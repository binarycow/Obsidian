using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Common.Collections;
using Obsidian.AST.NodeParsers;
using Obsidian.Lexing;
using Obsidian.Parsing;
using Obsidian.Transforming;
using Obsidian.WhiteSpaceControl;

namespace Obsidian.AST.Nodes.Statements
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class ForNode : StatementNode, IWhiteSpaceControlling
    {
        internal ForNode(ContainerNode primaryBlock, ContainerNode? elseBlock,
            string[] variableNames, ExpressionNode expression, ExpressionNode? filter, bool recursive, ParsingNode? endParsingNode, WhiteSpaceControlSet? whiteSpace = null)
            : base(
                  startParsingNode: null,
                  children: primaryBlock.YieldOne().Concat(elseBlock?.YieldOne() ?? Enumerable.Empty<ContainerNode>()),
                  endParsingNode: endParsingNode
            )
        {
            PrimaryBlock = primaryBlock;
            ElseBlock = elseBlock;
            VariableNames = variableNames;
            Expression = expression;
            Filter = filter;
            WhiteSpaceControl = whiteSpace ?? new WhiteSpaceControlSet();
            Recursive = recursive;
        }
        internal ForNode(ContainerNode primaryBlock, ContainerNode? elseBlock,
            string[] variableNames, object? expression, ExpressionNode? filter, bool recursive, ParsingNode? endParsingNode, WhiteSpaceControlSet? whiteSpace = null)
            : base(
                  startParsingNode: null,
                  children: primaryBlock.YieldOne().Concat(elseBlock?.YieldOne() ?? Enumerable.Empty<ContainerNode>()),
                  endParsingNode: endParsingNode
            )
        {
            PrimaryBlock = primaryBlock;
            ElseBlock = elseBlock;
            VariableNames = variableNames;
            Filter = filter;
            WhiteSpaceControl = whiteSpace ?? new WhiteSpaceControlSet();
            Recursive = recursive;
            AlreadyEvaluatedObject = expression;
        }

        public bool Recursive { get; }
        internal ContainerNode PrimaryBlock { get; }
        internal ContainerNode? ElseBlock { get; }
        internal string[] VariableNames { get; }
        internal ExpressionNode? Expression { get; }
        internal object? AlreadyEvaluatedObject { get; }
        internal ExpressionNode? Filter { get; }

        public WhiteSpaceControlSet WhiteSpaceControl { get; }

        private string DebuggerDisplay => $"{nameof(ForNode)} : Variables: \"{string.Join(", ", VariableNames)}\" Expression: \"{Expression}\"";

        internal static bool TryParseFor(JinjaEnvironment environment, Lexer lexer, ILookaroundEnumerator<ParsingNode> enumerator, [NotNullWhen(true)]out ASTNode? parsedNode)
        {
            ContainerNode? elseBlock = null;
            ExpressionNode? filterNode = null;
            parsedNode = default;

            if (ForParser.StartBlock.TryParse(enumerator.Current, out var outsideStart, out var primaryInsideStart) == false)
            {
                return false;
            }
            var primaryStartParsingNode = enumerator.Current;
            if (ForParser.StartBlock.TryGetAccumulations(ForParser.ForState.VariableNames, out var variableNames) == false || variableNames.Length == 0)
            {
                throw new NotImplementedException();
            }
            if (ForParser.StartBlock.TryGetAccumulation(ForParser.ForState.Expression, 0, out var expression) == false)
            {
                throw new NotImplementedException();
            }
            if(ForParser.StartBlock.TryGetAccumulation(ForParser.ForState.Filter, 0, out string? filter))
            {
                filterNode = ExpressionNode.FromString(environment, filter);
            }
            ForParser.StartBlock.TryGetVariable<bool>("recursive", out var recursive);
            enumerator.MoveNext();
            var primaryBlockChildren = ASTGenerator.ParseUntilFailure(environment, lexer, enumerator).ToArray();

            ContainerNode primaryBlock;

            if(ForParser.ElseBlock.TryParse(enumerator.Current, out var primaryInsideEnd, out var elseInsideStart))
            {
                var elseStartParsingNode = enumerator.Current;
                enumerator.MoveNext();
                var elseBlockChildren = ASTGenerator.ParseUntilFailure(environment, lexer, enumerator).ToArray();
                if (ForParser.EndBlock.TryParse(enumerator.Current, out var elseInsideEnd, out var outsideEnd) == false)
                {
                    throw new NotImplementedException();
                }

                primaryBlock = new ContainerNode(primaryStartParsingNode, primaryBlockChildren, null, 
                    new WhiteSpaceControlSet(primaryInsideStart, primaryInsideEnd));
                elseBlock = new ContainerNode(elseStartParsingNode, elseBlockChildren, null,
                    new WhiteSpaceControlSet(elseInsideStart, elseInsideEnd));
                parsedNode = new ForNode(primaryBlock, elseBlock, variableNames, ExpressionNode.FromString(environment, expression),
                    filterNode, recursive, enumerator.Current, new WhiteSpaceControlSet(outsideStart, outsideEnd));
                return true;
            }
            else
            {
                if (ForParser.EndBlock.TryParse(enumerator.Current, out primaryInsideEnd, out var outsideEnd) == false)
                {
                    throw new NotImplementedException();
                }
                primaryBlock = new ContainerNode(primaryStartParsingNode, primaryBlockChildren, null,
                    new WhiteSpaceControlSet(primaryInsideStart, primaryInsideEnd));
                parsedNode = new ForNode(primaryBlock, elseBlock, variableNames, ExpressionNode.FromString(environment, expression),
                    filterNode, recursive, enumerator.Current, new WhiteSpaceControlSet(outsideStart, outsideEnd));
                return true;
            }
        }

        public override void Transform(IManualWhiteSpaceTransformVisitor visitor, bool inner = false)
        {
            visitor.Transform(this, inner);
        }

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
    }
}
