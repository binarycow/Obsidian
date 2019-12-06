using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExpressionParser.Lexing;
using ExpressionParser.Transforming.Nodes;
using System.Diagnostics;
namespace ExpressionParser.Parsing
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class DictionaryItemNode : ASTNode
    {
        public DictionaryItemNode(ASTNode key, ASTNode value) : base(key.Tokens.Concat(value.Tokens))
        {
            Key = key;
            Value = value;
        }

        public ASTNode Key { get; }
        public ASTNode Value { get; }

        public override string DebuggerDisplay => $"({Key}, {Value})";
        public override TOutput Transform<TOutput>(INodeTransformVisitor<TOutput> visitor)
        {
            return visitor.Transform(this);
        }
    }
}
