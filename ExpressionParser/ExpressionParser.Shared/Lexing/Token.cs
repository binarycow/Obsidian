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
        public Token(TokenType tokenType, StringBuilder stringBuilder) : this(tokenType, stringBuilder.ToString())
        {
        }
        public Token(TokenType tokenType, IEnumerable<char> characters) : this(tokenType, new string(characters.ToArrayWithoutInstantiation()))
        {

        }
        public Token(TokenType tokenType, char @char) : this(tokenType, new string(@char, 1))
        {

        }
        public Token(TokenType tokenType, string textValue)
        {
            TokenType = tokenType;
            TextValue = textValue;
        }

        public TokenType TokenType { get; }
        public string TextValue { get; }
    }
}
