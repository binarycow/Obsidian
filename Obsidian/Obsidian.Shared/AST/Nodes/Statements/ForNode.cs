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

            if (ForParser.StartBlock.TryParse(enumerator.Current, out var accumulations) == false)
            {
                return false;
            }
            var primaryStartParsingNode = enumerator.Current;

            if (accumulations.TryGetValue(ForParser.ForState.VariableNames, out var variableNames) == false || variableNames.Length == 0)
            {
                throw new NotImplementedException();
            }
            if (accumulations.TryGetValue(ForParser.ForState.Expression, out var exprArray) == false || exprArray.Length == 0)
            {
                throw new NotImplementedException();
            }
            var expression = exprArray[0];

            enumerator.MoveNext();
            var primaryBlockChildren = ASTGenerator.ParseUntilFailure(enumerator).ToArray();

            ContainerNode primaryBlock;

            if(ForParser.ElseBlock.TryParse(enumerator.Current))
            {
                primaryBlock = new ContainerNode(primaryStartParsingNode, primaryBlockChildren, null);
                var elseStartParsingNode = enumerator.Current;
                enumerator.MoveNext();
                var elseBlockChildren = ASTGenerator.ParseUntilFailure(enumerator).ToArray();
                if (ForParser.EndBlock.TryParse(enumerator.Current) == false)
                {
                    throw new NotImplementedException();
                }
                elseBlock = new ContainerNode(elseStartParsingNode, elseBlockChildren, null);
                parsedNode = new ForNode(primaryBlock, elseBlock, variableNames, ExpressionNode.FromString(expression), enumerator.Current);
                return true;
            }
            else
            {
                if (ForParser.EndBlock.TryParse(enumerator.Current) == false)
                {
                    throw new NotImplementedException();
                }
                primaryBlock = new ContainerNode(primaryStartParsingNode, primaryBlockChildren, null);
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
