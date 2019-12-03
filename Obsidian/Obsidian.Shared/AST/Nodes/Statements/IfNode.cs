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
using Obsidian.WhiteSpaceControl;

namespace Obsidian.AST.Nodes.Statements
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class IfNode : StatementNode, IWhiteSpaceControlling
    {
        public IfNode(ParsingNode? startParsingNode, IEnumerable<ConditionalNode> conditions, ParsingNode? endParsingNode, WhiteSpaceControlSet? whiteSpace = null)
            : base(startParsingNode, conditions, endParsingNode)
        {
            Conditions = conditions.ToArrayWithoutInstantiation();
            WhiteSpaceControl = whiteSpace ?? new WhiteSpaceControlSet();
        }

        public ConditionalNode[] Conditions { get; }
        private string DebuggerDisplay => $"{nameof(IfNode)}";

        public WhiteSpaceControlSet WhiteSpaceControl { get; }

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
        public static bool TryParseIf(Lexer lexer, ILookaroundEnumerator<ParsingNode> enumerator, [NotNullWhen(true)]out ASTNode? parsedNode)
        {
            WhiteSpaceMode thisConditionEnd = WhiteSpaceMode.Default;
            WhiteSpaceMode nextConditionStart = WhiteSpaceMode.Default;
            var conditions = new Queue<ConditionalNode>();
            parsedNode = default;


            if(IfParser.StartBlock.TryParse(enumerator.Current, out var outsideStart, out var thisConditionStart) == false)
            {
                return false;
            }
            var startParsingNode = enumerator.Current;
            if(IfParser.StartBlock.TryGetAccumulation(IfParser.IfState.Expression, 0, out var previousBlockExpression) == false)
            {
                throw new NotImplementedException();
            }
            enumerator.MoveNext();
            var blockChildren = ASTGenerator.ParseUntilFailure(lexer, enumerator).ToArray();


            while(IfParser.ElseIfBlock.TryParse(enumerator.Current, out thisConditionEnd, out nextConditionStart))
            {
                conditions.Enqueue(new ConditionalNode(startParsingNode, ExpressionNode.FromString(previousBlockExpression), blockChildren, null,
                    new WhiteSpaceControlSet(thisConditionStart, thisConditionEnd)
                ));
                if (IfParser.ElseIfBlock.TryGetAccumulation(IfParser.IfState.Expression, 0, out previousBlockExpression) == false)
                {
                    throw new NotImplementedException();
                }
                enumerator.MoveNext();
                blockChildren = ASTGenerator.ParseUntilFailure(lexer, enumerator).ToArray();
                thisConditionStart = nextConditionStart;
            }

            if(IfParser.ElseBlock.TryParse(enumerator.Current, out thisConditionEnd, out nextConditionStart))
            {
                startParsingNode = enumerator.Current;
                conditions.Enqueue(new ConditionalNode(startParsingNode, ExpressionNode.FromString(previousBlockExpression), blockChildren, null,
                    new WhiteSpaceControlSet(thisConditionStart, thisConditionEnd)
                ));
                previousBlockExpression = JinjaEnvironment.TRUE;
                enumerator.MoveNext();
                blockChildren = ASTGenerator.ParseUntilFailure(lexer, enumerator).ToArray();
                thisConditionStart = nextConditionStart;
            }

            if(IfParser.EndBlock.TryParse(enumerator.Current, out thisConditionEnd, out var outsideEnd) == false)
            {
                throw new NotImplementedException();
            }
            conditions.Enqueue(new ConditionalNode(startParsingNode, ExpressionNode.FromString(previousBlockExpression), blockChildren, null,
                new WhiteSpaceControlSet(thisConditionStart, thisConditionEnd)
            ));

            parsedNode = new IfNode(null, conditions, enumerator.Current,
                new WhiteSpaceControlSet(outsideStart, outsideEnd)
            );
            return true;
        }
    }
}
