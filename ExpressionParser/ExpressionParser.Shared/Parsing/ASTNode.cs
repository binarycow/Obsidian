using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ExpressionParser.Lexing;
using ExpressionParser.Transforming.Nodes;

namespace ExpressionParser.Parsing
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal abstract class ASTNode : ITransformableNode
    {
        internal ASTNode(IEnumerable<Token> tokens)
        {
            TextValue = string.Concat(tokens.Select(token => token.TextValue));
            Tokens = tokens.ToArrayWithoutInstantiation();
        }
        internal ASTNode(Token token)
        {
            TextValue = token.TextValue;
            Tokens = new[] { token };
        }

        internal void SetParent(ASTNode parent)
        {
            if (Parent != default) throw new InvalidOperationException();
            Parent = parent;
        }

        internal ASTNode? Parent { get; set; }
        internal string TextValue { get; }
        
        internal Token[] Tokens { get; }

        public abstract TOutput Transform<TOutput>(INodeTransformVisitor<TOutput> visitor);

        internal abstract string DebuggerDisplay { get; }
    }
}
