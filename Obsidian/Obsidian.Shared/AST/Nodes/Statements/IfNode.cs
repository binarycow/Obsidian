using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Common.Collections;
using Obsidian.AST.NodeParsers;
using Obsidian.AST.Nodes.MiscNodes;
using Obsidian.Lexing;
using Obsidian.Parsing;
using Obsidian.Transforming;

namespace Obsidian.AST.Nodes.Statements
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class IfNode : StatementNode
    {
        public IfNode(ParsingNode? startParsingNode, IEnumerable<ConditionalNode> conditions, ParsingNode? endParsingNode)
            : base(startParsingNode, conditions, endParsingNode)
        {
            Conditions = conditions.ToArrayWithoutInstantiation();
        }

        public ConditionalNode[] Conditions { get; }
        private string DebuggerDisplay => $"{nameof(IfNode)}";

        public override TOutput Transform<TOutput>(ITransformVisitor<TOutput> visitor)
        {
            return visitor.Transform(this);
        }

        public override void Transform(ITransformVisitor visitor)
        {
            visitor.Transform(this);
        }
        public override TOutput Transform<TOutput>(IForceTransformVisitor<TOutput> visitor, bool force)
        {
            return visitor.Transform(this, force);
        }
        public static bool TryParseIf(ILookaroundEnumerator<ParsingNode> enumerator, [NotNullWhen(true)]out ASTNode? parsedNode)
        {
            var conditions = new Queue<ConditionalNode>();
            parsedNode = default;


            if(IfParser.StartBlock.TryParse(enumerator.Current) == false)
            {
                return false;
            }
            var startParsingNode = enumerator.Current;
            if(IfParser.StartBlock.TryGetAccumulation(IfParser.IfState.Expression, 0, out var previousBlockExpression) == false)
            {
                throw new NotImplementedException();
            }
            enumerator.MoveNext();
            var blockChildren = ASTGenerator.ParseUntilFailure(enumerator).ToArray();

            while(IfParser.ElseIfBlock.TryParse(enumerator.Current))
            {
                conditions.Enqueue(new ConditionalNode(startParsingNode, ExpressionNode.FromString(previousBlockExpression), blockChildren, null));
                if (IfParser.ElseIfBlock.TryGetAccumulation(IfParser.IfState.Expression, 0, out previousBlockExpression) == false)
                {
                    throw new NotImplementedException();
                }
                enumerator.MoveNext();
                blockChildren = ASTGenerator.ParseUntilFailure(enumerator).ToArray();
            }

            if(IfParser.ElseBlock.TryParse(enumerator.Current))
            {
                startParsingNode = enumerator.Current;
                conditions.Enqueue(new ConditionalNode(startParsingNode, ExpressionNode.FromString(previousBlockExpression), blockChildren, null));
                previousBlockExpression = JinjaEnvironment.TRUE;
                enumerator.MoveNext();
                blockChildren = ASTGenerator.ParseUntilFailure(enumerator).ToArray();
            }

            if(IfParser.EndBlock.TryParse(enumerator.Current) == false)
            {
                throw new NotImplementedException();
            }
            conditions.Enqueue(new ConditionalNode(startParsingNode, ExpressionNode.FromString(previousBlockExpression), blockChildren, null));

            parsedNode = new IfNode(null, conditions, enumerator.Current);
            return true;
        }
    }
}
