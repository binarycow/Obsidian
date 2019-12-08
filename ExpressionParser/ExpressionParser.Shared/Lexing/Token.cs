using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ExpressionParser.Lexing
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal struct Token : IEquatable<Token>
    {
        private string DebuggerDisplay => $"{nameof(Token)} : {TokenType} : '{TextValue}'";
        private Token(TokenType tokenType, StringBuilder stringBuilder) : this(tokenType, stringBuilder.ToString())
        {
        }
        private Token(TokenType tokenType, IEnumerable<char> characters) : this(tokenType, new string(characters.ToArrayWithoutInstantiation()))
        {

        }
        private Token(TokenType tokenType, char @char) : this(tokenType, new string(@char, 1))
        {

        }
        private Token(TokenType tokenType, string textValue)
        {
            TokenType = tokenType;
            TextValue = textValue;
            SecondaryTokenType = null;
        }


        internal Token(TokenType tokenType, TokenType? secondaryTokenType, StringBuilder stringBuilder) : this(tokenType, stringBuilder)
        {
            SecondaryTokenType = secondaryTokenType;
        }
        internal Token(TokenType tokenType, TokenType? secondaryTokenType, IEnumerable<char> characters) : this(tokenType, characters)
        {
            SecondaryTokenType = secondaryTokenType;
        }
        internal Token(TokenType tokenType, TokenType? secondaryTokenType, char tokenCharacter) : this(tokenType, tokenCharacter)
        {
            SecondaryTokenType = secondaryTokenType;
        }
        internal Token(TokenType tokenType, TokenType? secondaryTokenType, string textValue) : this(tokenType, textValue)
        {
            SecondaryTokenType = secondaryTokenType;
        }


        internal TokenType TokenType { get; }
        internal TokenType? SecondaryTokenType { get; }
        internal string TextValue { get; }






        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            if (!(obj is Token token)) return false;
            return Equals(token);
        }

        public bool Equals(Token other)
        {
            return other.TokenType == TokenType &&
                other.SecondaryTokenType == SecondaryTokenType &&
                other.TextValue == TextValue;
        }


        public override int GetHashCode()
        {
            unchecked
            {
                int hash = (int)2166136261;
                hash = (hash * 16777619) ^ TokenType.GetHashCode();
                hash = (hash * 16777619) ^ SecondaryTokenType?.GetHashCode() ?? TokenType.GetHashCode();
                hash = (hash * 16777619) ^ TextValue.GetHashCode();
                return hash;
            }
        }

        public static bool operator ==(Token left, Token right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Token left, Token right)
        {
            return !(left == right);
        }
    }
}
