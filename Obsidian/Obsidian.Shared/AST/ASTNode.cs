using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obsidian.Lexing;
using Obsidian.CommentRemover;
using Obsidian.Parsing;
using Obsidian.Transforming;
using Obsidian.WhiteSpaceControl;

namespace Obsidian.AST
{
    public abstract class ASTNode : ITransformable, IForceTransformable
    {
        public ASTNode(IEnumerable<ParsingNode> parsingNodes)
        {
            ParsingNodes = parsingNodes.ToArray();
        }
        public ASTNode(ParsingNode parsingNode) : this(Enumerable.Repeat(parsingNode, 1))
        {
        }

        public ParsingNode[] ParsingNodes { get; }


        public override string ToString()
        {
            return string.Join(string.Empty, ParsingNodes.SelectMany(x => x.ToString()));
        }

        public string ToString(bool debug)
        {
            return debug ? ToString().WhiteSpaceEscape() : ToString();
        }

        public abstract TOutput Transform<TOutput>(ITransformVisitor<TOutput> visitor);
        public abstract TOutput Transform<TOutput>(IForceTransformVisitor<TOutput> visitor, bool force);



        public static ASTNode GetTemplateNode(JinjaEnvironment environment, string templateText)
        {

            var whiteSpaceCounter = new WhiteSpaceCounterVisitor();
            var lexer = new Lexer(environment);
            var tokens = lexer.Tokenize(templateText).ToArray();
            var parsed = Parser.Parse(tokens).ToArray();
            var environmentTrimmed = EnvironmentTrimming.EnvironmentTrim(parsed, environment.Settings).ToArray();
            ASTNode templateNode = ASTGenerator.ParseTemplate(environmentTrimmed);
            templateNode = templateNode.Transform(CommentRemoverTransformer.Instance);
            templateNode = WhiteSpaceController.ControlWhiteSpace(templateNode);
            //templateNode = templateNode.Transform(TemplateContainerAssembler.Instance);

            return templateNode;
        }
    }
}
