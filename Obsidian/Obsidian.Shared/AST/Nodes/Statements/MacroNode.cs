using Common.Collections;
using Obsidian.AST.NodeParsers;
using Obsidian.AST.Nodes.MiscNodes;
using Obsidian.Lexing;
using Obsidian.Parsing;
using Obsidian.Transforming;
using Obsidian.WhiteSpaceControl;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Obsidian.AST.Nodes.Statements
{
    internal class MacroNode : ASTNode, IWhiteSpaceControlling
    {
        internal MacroNode(ParsingNode? startParsingNode, string macroText, ContainerNode contents, ParsingNode? endParsingNode, WhiteSpaceControlSet? whiteSpace = null)
            : base(startParsingNode, contents.ParsingNodes, endParsingNode)
        {
            Contents = contents;
            WhiteSpaceControl = whiteSpace ?? new WhiteSpaceControlSet();
            MacroText = macroText;
        }

        internal ContainerNode Contents { get; }
        internal string MacroText { get; }
        public WhiteSpaceControlSet WhiteSpaceControl { get; }

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

        internal static bool TryParseMacro(JinjaEnvironment environment, Lexer lexer, ILookaroundEnumerator<ParsingNode> enumerator, [NotNullWhen(true)]out ASTNode? parsedNode)
        {
            parsedNode = default;


            if (MacroParser.StartBlock.TryParse(enumerator.Current, out var outsideStart, out var insideStart) == false)
            {
                return false;
            }
            if (MacroParser.StartBlock.TryGetAccumulation(MacroParser.MacroState.MacroDefinition, 0, out var macroText) == false)
            {
                throw new NotImplementedException();
            }
            var startParsingNode = enumerator.Current;
            enumerator.MoveNext();
            var contents = ASTGenerator.ParseUntilFailure(environment, lexer, enumerator).ToArray();
            if (MacroParser.EndBlock.TryParse(enumerator.Current, out var insideEnd, out var outsideEnd) == false)
            {
                throw new NotImplementedException();
            }
            var endParsingNode = enumerator.Current;
            var contentsNode = new ContainerNode(null, contents, null, 
                new WhiteSpaceControlSet(insideStart, insideEnd)
            );

            parsedNode = new MacroNode(startParsingNode, macroText, contentsNode, endParsingNode,
                new WhiteSpaceControlSet(outsideStart, outsideEnd)
            );
            return true;
        }

    }

}
