using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obsidian.Lexing;
using Obsidian.Parsing;
using Obsidian.Transforming;
using Obsidian.WhiteSpaceControl;

namespace Obsidian.AST
{
    public abstract class ASTNode : ITransformable, IForceTransformable
    {
        public ASTNode(ParsingNode parsingNode)
        {
            ParsingNodes = new ParsingNode[] { parsingNode };
            StartParsingNode = default;
            EndParsingNode = default;
        }
        public ASTNode(ParsingNode? startingParsingNode, IEnumerable<ParsingNode> contentParsingNodes, ParsingNode? endingParsingNode)
        {
            ParsingNodes =
                (startingParsingNode?.YieldOne() ?? Enumerable.Empty<ParsingNode>())
                .Concat(contentParsingNodes)
                .Concat(endingParsingNode?.YieldOne() ?? Enumerable.Empty<ParsingNode>())
                .ToArray();
            StartParsingNode = startingParsingNode;
            EndParsingNode = endingParsingNode;
        }

        public ParsingNode[] ParsingNodes { get; }
        public ParsingNode? StartParsingNode { get; }
        public ParsingNode? EndParsingNode { get; }


        public override string ToString()
        {
            return string.Join(string.Empty, ParsingNodes.SelectMany(x => x.ToString()));
        }

        public string ToString(bool debug)
        {
            return debug ? ToString().WhiteSpaceEscape() : ToString();
        }

        public virtual void ToOriginalText(StringBuilder stringBuilder)
        {
            foreach (var node in ParsingNodes)
            {
                node.ToOriginalText(stringBuilder);
            }
        }
        public string OriginalText
        {
            get
            {
                var sb = new StringBuilder();
                ToOriginalText(sb);
                return sb.ToString();
            }
        }


        public abstract TOutput Transform<TOutput>(ITransformVisitor<TOutput> visitor);
        public abstract TOutput Transform<TOutput>(IForceTransformVisitor<TOutput> visitor, bool force);

        public abstract void Transform(ITransformVisitor visitor);



        public static ASTNode GetTemplateNode(JinjaEnvironment environment, string templateText)
        {

            var whiteSpaceCounter = new WhiteSpaceCounterVisitor();
            var lexer = new Lexer(environment);
            var tokens = lexer.Tokenize(templateText).ToArray();
            var parsed = Parser.Parse(tokens).ToArray();
            ASTNode templateNode = ASTGenerator.ParseTemplate(parsed);
            //templateNode = templateNode.Transform(CommentRemoverTransformer.Instance);
            templateNode = WhiteSpaceController.ControlWhiteSpace(environment, templateNode);

            return templateNode;
        }

#if DEBUG
        public static string CheckOriginalText(JinjaEnvironment environment, string templateText)
        {
            var whiteSpaceCounter = new WhiteSpaceCounterVisitor();
            var lexer = new Lexer(environment);
            var tokens = lexer.Tokenize(templateText).ToArray();
            var parsed = Parser.Parse(tokens).ToArray();
            ASTNode templateNode = ASTGenerator.ParseTemplate(parsed);

            var stringBuilder = new StringBuilder();
            templateNode.ToOriginalText(stringBuilder);
            return stringBuilder.ToString();
        }
#endif
    }
}
