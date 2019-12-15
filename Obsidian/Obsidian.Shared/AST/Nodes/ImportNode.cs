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
        public ImportNode(ParsingNode parsingNode, string fromDef, ExpressionParser.Parsing.ASTNode importDef, string asDef) : base(parsingNode)
        {
            From = fromDef;
            Import = importDef;
            As = asDef;
        }

        public string From { get; }
        public ExpressionParser.Parsing.ASTNode Import { get; }
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
            if (ImportParser.Parser.TryParse(enumerator.Current) == false)
            {
                return false;
            }
            _ = ImportParser.Parser.TryGetAccumulation(ImportParser.ImportState.FromDefinition, 0, out var fromDef);
            if (ImportParser.Parser.TryGetAccumulation(ImportParser.ImportState.ImportDefinition, 0, out var importDef) == false)
            {
                throw new NotImplementedException();
            }
            if (ImportParser.Parser.TryGetAccumulation(ImportParser.ImportState.AsDefinition, 0, out var asDef) == false)
            {
                throw new NotImplementedException();
            }

            var importNode = environment.Evaluation.Parse(importDef);

            parsedNode = new ImportNode(enumerator.Current, fromDef, importNode, asDef);
            return true;
        }
    }
}
