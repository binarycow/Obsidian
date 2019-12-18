using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Obsidian.Lexing;
using Obsidian.Parsing;
using Obsidian.Transforming;
using Obsidian.WhiteSpaceControl;

namespace Obsidian.AST
{
    internal abstract class ASTNode : DynamicObject, ITransformable, IForceTransformable
    {
        private static int _NodeID = 0;

        internal ASTNode(ParsingNode parsingNode)
        {
            ParsingNodes = new ParsingNode[] { parsingNode };
            StartParsingNode = default;
            EndParsingNode = default;
            NodeID = _NodeID++;
        }
        internal ASTNode(ParsingNode? startingParsingNode, IEnumerable<ParsingNode> contentParsingNodes, ParsingNode? endingParsingNode)
        {
            ParsingNodes =
                (startingParsingNode?.YieldOne() ?? Enumerable.Empty<ParsingNode>())
                .Concat(contentParsingNodes)
                .Concat(endingParsingNode?.YieldOne() ?? Enumerable.Empty<ParsingNode>())
                .ToArray();
            StartParsingNode = startingParsingNode;
            EndParsingNode = endingParsingNode;
            NodeID = _NodeID++;
        }

        internal ParsingNode[] ParsingNodes { get; }
        internal ParsingNode? StartParsingNode { get; }
        internal ParsingNode? EndParsingNode { get; }
        internal int NodeID { get; }


        public override string ToString()
        {
            return string.Join(string.Empty, ParsingNodes.SelectMany(x => x.ToString()));
        }

        internal string ToString(bool debug)
        {
            return debug ? ToString().WhiteSpaceEscape() : ToString();
        }

        internal virtual void ToOriginalText(StringBuilder stringBuilder)
        {
            foreach (var node in ParsingNodes)
            {
                node.ToOriginalText(stringBuilder);
            }
        }
        internal string OriginalText
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

        public abstract void Transform(IManualWhiteSpaceTransformVisitor visitor, bool inner = false);


        internal static ASTNode GetTemplateNode(JinjaEnvironment environment, string templateText)
        {
            var lexer = new Lexer(environment);
            var tokens = lexer.Tokenize(templateText).ToArray();
            var parsed = Parser.Parse(tokens).ToArray();
            var outputCombined = OutputCombiner.CombineOutput(parsed);
            ASTNode templateNode = ASTGenerator.ParseTemplate(environment, lexer, outputCombined);
            //templateNode = templateNode.Transform(CommentRemoverTransformer.Instance);
            templateNode = WhiteSpaceController.ControlWhiteSpace(environment, templateNode);

            return templateNode;
        }

#if DEBUG
        internal static string CheckOriginalText(JinjaEnvironment environment, string templateText)
        {
            var lexer = new Lexer(environment);
            var tokens = lexer.Tokenize(templateText).ToArray();
            var parsed = Parser.Parse(tokens).ToArray();
            var outputCombined = OutputCombiner.CombineOutput(parsed);
            ASTNode templateNode = ASTGenerator.ParseTemplate(environment, lexer, outputCombined);

            var stringBuilder = new StringBuilder();
            templateNode.ToOriginalText(stringBuilder);
            return stringBuilder.ToString();
        }
#endif
    }
}
