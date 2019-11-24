using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExpressionParser.Configuration;
using ExpressionParser.Lexing;

namespace ExpressionParser
{
    public static class LanguageDefinition
    {
        public static readonly Lazy<CSharpLanguageDefinition> _CSharp = new Lazy<CSharpLanguageDefinition>();
        public static ILanguageDefinition CSharp => _CSharp.Value;


        public static IDictionary<char, TokenType> StandardSingleCharacterTokens => new Dictionary<char, TokenType>
        {
            { '(', TokenType.Paren_Open },
            { ')', TokenType.Paren_Close },
            { '[', TokenType.SquareBrace_Open },
            { ']', TokenType.SquareBrace_Close },
            { '{', TokenType.CurlyBrace_Open },
            { '}', TokenType.CurlyBrace_Close },
            { ',', TokenType.Comma },
            { ':', TokenType.Colon },
            { '\'', TokenType.SingleQuote },
            { '"', TokenType.DoubleQuote },
        };
    }
}
