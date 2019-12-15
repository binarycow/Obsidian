using System;
using System.Collections.Generic;
using System.Text;
using ExpressionParser.Lexing;
using ExpressionParser.Transforming.Nodes;
using System.Diagnostics;

namespace ExpressionParser.Parsing
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class IdentifierNode : ASTNode
    {
        internal IdentifierNode(Token token) : base(token)
        {
        }

        internal override string DebuggerDisplay => TextValue;

        public override TOutput Transform<TOutput>(INodeTransformVisitor<TOutput> visitor)
        {
            return visitor.Transform(this);
        }
    }
}
