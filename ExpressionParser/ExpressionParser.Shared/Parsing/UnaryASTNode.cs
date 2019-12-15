using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ExpressionParser.Operators;
using ExpressionParser.Transforming.Nodes;

namespace ExpressionParser.Parsing
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class UnaryASTNode : ASTNode
    {
        internal UnaryASTNode(Operator @operator, ASTNode right) : base(@operator.Token.YieldOne().Concat(right.Tokens))
        {
            Operator = @operator;
            Right = right;
        }
        internal Operator Operator { get; }
        internal ASTNode Right { get; }

        internal override string DebuggerDisplay => $"{Operator.DebuggerDisplay} {Right.DebuggerDisplay}";

        public override TOutput Transform<TOutput>(INodeTransformVisitor<TOutput> visitor)
        {
            return visitor.Transform(this);
        }
    }
}
