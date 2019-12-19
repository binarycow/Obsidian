using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Common.Collections;
using Obsidian.AST.NodeParsers;
using Obsidian.Lexing;
using Obsidian.Parsing;
using Obsidian.Transforming;

namespace Obsidian.AST.Nodes.Statements
{
    internal class FromNode : ASTNode
    {
        public FromNode(ParsingNode parsingNode, ExpressionParser.Parsing.ASTNode template, IEnumerable<(string MacroName, string AliasName)> imports) : base(parsingNode)
        {
            Template = template;
            Imports = imports.ToArrayWithoutInstantiation();
        }

        public ExpressionParser.Parsing.ASTNode Template { get; }
        public IEnumerable<(string MacroName, string AliasName)> Imports { get; }

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

        internal static bool TryParseFrom(JinjaEnvironment environment, Lexer lexer, ILookaroundEnumerator<ParsingNode> enumerator, [NotNullWhen(true)]out ASTNode? parsedNode)
        {
            parsedNode = default;
            if (FromParser.Parser.TryParse(enumerator.Current) == false)
            {
                return false;
            }
            if (FromParser.Parser.TryGetAccumulation(FromParser.FromState.Template, 0, out var template) == false)
            {
                throw new NotImplementedException();
            }
            if (FromParser.Parser.TryGetAccumulations(FromParser.FromState.Import, out var importAccumulations) == false)
            {
                throw new NotImplementedException();
            }

            var imports = new List<(string MacroName, string AliasName)>();
            foreach(var importString in importAccumulations)
            {
                if(FromParser.AsParser.TryParse(lexer, importString) == false)
                {
                    throw new NotImplementedException();
                }
                if(FromParser.AsParser.TryGetAccumulation(FromParser.AsState.MacroName, 0, out var macroName) == false)
                {
                    throw new NotImplementedException();
                }
                if(FromParser.AsParser.TryGetAccumulation(FromParser.AsState.AliasName, 0, out var aliasName) == false)
                {
                    aliasName = macroName;
                }
                imports.Add((MacroName: macroName, AliasName: aliasName));
            }

            var templateNode = environment.Evaluation.Parse(template);
            parsedNode = new FromNode(enumerator.Current, templateNode, imports);
            return true;
        }

    }
}
