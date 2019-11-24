using System;
using System.Collections.Generic;
using System.Text;
using ExpressionParser.Lexing;
using ExpressionParser.Transforming.Nodes;

namespace ExpressionParser.Parsing
{
    public class IdentifierNode : ASTNode
    {
        public IdentifierNode(Token token) : base(token)
        {
        }
        public override TOutput Transform<TOutput>(INodeTransformVisitor<TOutput> visitor)
        {
            return visitor.Transform(this);
        }
    }
}
