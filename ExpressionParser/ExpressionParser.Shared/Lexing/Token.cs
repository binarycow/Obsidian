using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ExpressionParser.Lexing
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class Token
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
        }


        public Token(TokenType tokenType, TokenType? secondaryTokenType, StringBuilder stringBuilder) : this(tokenType, stringBuilder)
        {
            SecondaryTokenType = secondaryTokenType;
        }
        public Token(TokenType tokenType, TokenType? secondaryTokenType, IEnumerable<char> characters) : this(tokenType, characters)
        {
            SecondaryTokenType = secondaryTokenType;
        }
        public Token(TokenType tokenType, TokenType? secondaryTokenType, char @char) : this(tokenType, @char)
        {
            SecondaryTokenType = secondaryTokenType;
        }
        public Token(TokenType tokenType, TokenType? secondaryTokenType, string textValue) : this(tokenType, textValue)
        {
            SecondaryTokenType = secondaryTokenType;
        }


        public TokenType TokenType { get; }
        public TokenType? SecondaryTokenType { get; } = null;
        public string TextValue { get; }
    }
}
