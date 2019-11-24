using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExpressionParser.Lexing;
using ExpressionParser.Transforming.Nodes;

namespace ExpressionParser.Parsing
{
    public abstract class ASTNode : ITransformableNode
    {
        public ASTNode(IEnumerable<Token> tokens)
        {
            TextValue = string.Concat(tokens.Select(token => token.TextValue));
            Tokens = tokens.ToArrayWithoutInstantiation();
        }
        public ASTNode(Token token)
        {
            TextValue = token.TextValue;
            Tokens = new[] { token };
        }

        public string TextValue { get; }
        
        public Token[] Tokens { get; }

        public abstract TOutput Transform<TOutput>(INodeTransformVisitor<TOutput> visitor);
    }
}
