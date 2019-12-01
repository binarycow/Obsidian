using System;
using System.Collections.Generic;
using System.Text;
using ExpressionParser.Lexing;
using ExpressionParser.Transforming.Nodes;

namespace ExpressionParser.Parsing
{
    public class LiteralNode : ASTNode
    {
        private LiteralNode(Token token, object? value) : base(token)
        {
            Value = value;
        }
        public object? Value { get; }


        public override TOutput Transform<TOutput>(INodeTransformVisitor<TOutput> visitor)
        {
            return visitor.Transform(this);
        }

        public static LiteralNode CreateIntegerLiteral(Token token)
        {
            if (int.TryParse(token.TextValue, out var intValue))
            {
                return new LiteralNode(token, intValue);
            }
            throw new NotImplementedException();
        }
        public static LiteralNode CreateFloatLiteral(Token token)
        {
            if (float.TryParse(token.TextValue, out var floatValue))
            {
                return new LiteralNode(token, floatValue);
            }
            throw new NotImplementedException();
        }
        public static LiteralNode CreateCharacterLiteral(Token token)
        {
            return new LiteralNode(token, token.TextValue[0]);
        }
        public static LiteralNode CreateStringLiteral(Token token)
        {
            return new LiteralNode(token, token.TextValue);
        }
        public static LiteralNode CreateNull()
        {
            return new LiteralNode(new Token(TokenType.NullLiteral, "null"), null);
        }
    }
}
