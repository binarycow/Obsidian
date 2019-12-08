using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExpressionParser.Operators;
using ExpressionParser.Transforming.Nodes;
using System.Diagnostics;

namespace ExpressionParser.Parsing
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class BinaryASTNode : ASTNode
    {
        internal BinaryASTNode(ASTNode left, Operator @operator, ASTNode right) : base(left.Tokens.Concat(@operator.Token).Concat(right.Tokens))
        {
            Left = left;
            Operator = @operator;
            Right = right;
        }
        internal ASTNode Left { get; }
        internal Operator Operator { get;  }
        internal ASTNode Right { get; }

        internal override string DebuggerDisplay => $"{Left.DebuggerDisplay} {Operator.DebuggerDisplay} {Right.DebuggerDisplay}";

        public override TOutput Transform<TOutput>(INodeTransformVisitor<TOutput> visitor)
        {
            return visitor.Transform(this);
        }
    }
}
