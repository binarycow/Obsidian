using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Common.Collections;
using Obsidian.AST.NodeParsers;
using Obsidian.Lexing;
using Obsidian.Parsing;
using Obsidian.Transforming;

namespace Obsidian.AST.Nodes
{
    internal class ImportNode : ASTNode
    {
        public ImportNode(ParsingNode parsingNode, ExpressionParser.Parsing.ASTNode template, string asDef) : base(parsingNode)
        {
            Template = template;
            As = asDef;
        }

        public ExpressionParser.Parsing.ASTNode Template { get; }
        public string As { get; }

        public override TOutput Transform<TOutput>(ITransformVisitor<TOutput> visitor)
        {
            return visitor.Transform(this);
        }

        public override TOutput Transform<TOutput>(IForceTransformVisitor<TOutput> visitor, bool force)
        {
            return visitor.Transform(this, force);
        }

        public override void Transform(ITransformVisitor visitor)
        {
            visitor.Transform(this);
        }

        public override void Transform(IManualWhiteSpaceTransformVisitor visitor, bool inner = false)
        {
            visitor.Transform(this, inner);
        }

        internal static bool TryParseImport(JinjaEnvironment environment, Lexer lexer, ILookaroundEnumerator<ParsingNode> enumerator, [NotNullWhen(true)]out ASTNode? parsedNode)
        {
            parsedNode = default;
            if (ImportParser.ParserImport.TryParse(enumerator.Current) == false)
            {
                return false;
            }
            if (ImportParser.ParserImport.TryGetAccumulation(ImportParser.ImportState.Template, 0, out var template) == false)
            {
                throw new NotImplementedException();
            }
            if (ImportParser.ParserImport.TryGetAccumulation(ImportParser.ImportState.As, 0, out var asText) == false)
            {
                throw new NotImplementedException();
            }
            var templateNode = environment.Evaluation.Parse(template);
            parsedNode = new ImportNode(enumerator.Current, templateNode, asText);
            return true;
        }
    }
}
