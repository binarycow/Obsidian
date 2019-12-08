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
#pragma warning disable CA1812 // Avoid uninstantiated internal classes
    internal class DictionaryItemNode : ASTNode
#pragma warning restore CA1812 // Avoid uninstantiated internal classes
    {
        internal DictionaryItemNode(ASTNode key, ASTNode value) : base(key.Tokens.Concat(value.Tokens))
        {
            Key = key;
            Value = value;
        }

        internal ASTNode Key { get; }
        internal ASTNode Value { get; }

        internal override string DebuggerDisplay => $"({Key}, {Value})";
        public override TOutput Transform<TOutput>(INodeTransformVisitor<TOutput> visitor)
        {
            return visitor.Transform(this);
        }
    }
}
