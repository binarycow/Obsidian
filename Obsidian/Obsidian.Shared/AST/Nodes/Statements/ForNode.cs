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
    public class ForNode : StatementNode
    {
        public ForNode(ContainerNode primaryBlock, ContainerNode? elseBlock,
            string[] variableNames, ExpressionNode expression, ParsingNode endParsingNode)
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
        }

        public ContainerNode PrimaryBlock { get; }
        public ContainerNode? ElseBlock { get; }
        public string[] VariableNames { get; }
        public ExpressionNode Expression { get; }

        private string DebuggerDisplay => $"{nameof(ForNode)} : Variables: \"{string.Join(", ", VariableNames)}\" Expression: \"{Expression}\"";

        internal static bool TryParseFor(ILookaroundEnumerator<ParsingNode> enumerator, [NotNullWhen(true)]out ASTNode? parsedNode)
        {
            ContainerNode? elseBlock = null;
            parsedNode = default;

            if (ForParser.StartBlock.TryParse(enumerator.Current, out var outsideStartWhiteSpace, out var primaryBlockInsideStartWhiteSpace) == false)
            {
                return false;
            }
            var primaryStartParsingNode = enumerator.Current;
            if (ForParser.StartBlock.TryGetAccumulations(ForParser.ForState.VariableNames, out var variableNames) == false || variableNames.Length == 0)
            {
                throw new NotImplementedException();
            }
            if (ForParser.StartBlock.TryGetAccumulation(ForParser.ForState.Expression, 0, out var expression) == false || variableNames.Length == 0)
            {
                throw new NotImplementedException();
            }
            enumerator.MoveNext();
            var primaryBlockChildren = ASTGenerator.ParseUntilFailure(enumerator).ToArray();

            ContainerNode primaryBlock;

            if(ForParser.ElseBlock.TryParse(enumerator.Current, out var primaryBlockInsideEndWhiteSpace, out var elseBlockInsideStartWhiteSpace))
            {
                var elseStartParsingNode = enumerator.Current;
                enumerator.MoveNext();
                var elseBlockChildren = ASTGenerator.ParseUntilFailure(enumerator).ToArray();
                if (ForParser.EndBlock.TryParse(enumerator.Current, out var elseBlockInsideEndWhiteSpace, out var outsideEndWhiteSpace) == false)
                {
                    throw new NotImplementedException();
                }


                primaryBlock = new ContainerNode(primaryStartParsingNode, primaryBlockChildren, null, 
                    new WhiteSpaceControlSet(outsideStartWhiteSpace, outsideEndWhiteSpace, primaryBlockInsideStartWhiteSpace, primaryBlockInsideEndWhiteSpace));
                elseBlock = new ContainerNode(elseStartParsingNode, elseBlockChildren, null,
                    new WhiteSpaceControlSet(outsideStartWhiteSpace, outsideEndWhiteSpace, elseBlockInsideStartWhiteSpace, elseBlockInsideEndWhiteSpace));
                parsedNode = new ForNode(primaryBlock, elseBlock, variableNames, ExpressionNode.FromString(expression), enumerator.Current);
                return true;
            }
            else
            {
                if (ForParser.EndBlock.TryParse(enumerator.Current, out primaryBlockInsideEndWhiteSpace, out var outsideEndWhiteSpace) == false)
                {
                    throw new NotImplementedException();
                }
                primaryBlock = new ContainerNode(primaryStartParsingNode, primaryBlockChildren, null,
                    new WhiteSpaceControlSet(outsideStartWhiteSpace, outsideEndWhiteSpace, primaryBlockInsideStartWhiteSpace, primaryBlockInsideEndWhiteSpace));
                parsedNode = new ForNode(primaryBlock, elseBlock, variableNames, ExpressionNode.FromString(expression), enumerator.Current);
                return true;
            }
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
