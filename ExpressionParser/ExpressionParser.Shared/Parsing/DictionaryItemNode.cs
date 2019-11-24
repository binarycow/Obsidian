using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExpressionParser.Lexing;
using ExpressionParser.Transforming.Nodes;

namespace ExpressionParser.Parsing
{
    public class DictionaryItemNode : ASTNode
    {
        public DictionaryItemNode(ASTNode key, ASTNode value) : base(key.Tokens.Concat(value.Tokens))
        {
            Key = key;
            Value = value;
        }

        public ASTNode Key { get; }
        public ASTNode Value { get; }

        public override TOutput Transform<TOutput>(INodeTransformVisitor<TOutput> visitor)
        {
            return visitor.Transform(this);
        }
    }
}
