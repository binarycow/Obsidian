using System;
using System.Collections.Generic;
using System.Text;
using ExpressionParser.Lexing;
using ExpressionParser.Transforming.Nodes;
using System.Diagnostics;



namespace ExpressionParser.Parsing
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class LiteralNode : ASTNode
    {
        private LiteralNode(Token token, object? value) : base(token)
        {
            Value = value;
        }
        internal object? Value { get; }

        internal override string DebuggerDisplay => $"{Value}";

        public override TOutput Transform<TOutput>(INodeTransformVisitor<TOutput> visitor)
        {
            return visitor.Transform(this);
        }

        internal static LiteralNode CreateIntegerLiteral(Token token)
        {
            if (int.TryParse(token.TextValue, out var intValue))
            {
                return new LiteralNode(token, intValue);
            }
            throw new NotImplementedException();
        }
        internal static LiteralNode CreateFloatLiteral(Token token)
        {
            if (float.TryParse(token.TextValue, out var floatValue))
            {
                return new LiteralNode(token, floatValue);
            }
            throw new NotImplementedException();
        }
        internal static LiteralNode CreateCharacterLiteral(Token token)
        {
            return new LiteralNode(token, token.TextValue[0]);
        }
        internal static LiteralNode CreateStringLiteral(Token token)
        {
            return new LiteralNode(token, token.TextValue);
        }
        internal static LiteralNode CreateNull()
        {
            return new LiteralNode(new Token(TokenType.NullLiteral, null, "null"), null);
        }
    }
}
