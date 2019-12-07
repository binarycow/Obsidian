using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExpressionParser.Configuration;
using ExpressionParser.Lexing;

namespace ExpressionParser
{
    internal static class LanguageDefinition
    {
        public static readonly Lazy<CSharpLanguageDefinition> _CSharp = new Lazy<CSharpLanguageDefinition>();
        public static ILanguageDefinition CSharp => _CSharp.Value;


        public static IDictionary<char, TokenType> StandardSingleCharacterTokens => new Dictionary<char, TokenType>
        {
            { '(', TokenType.ParenOpen },
            { ')', TokenType.ParenClose },
            { '[', TokenType.SquareBraceOpen },
            { ']', TokenType.SquareBraceClose },
            { '{', TokenType.CurlyBraceOpen },
            { '}', TokenType.CurlyBraceClose },
            { ',', TokenType.Comma },
            { ':', TokenType.Colon },
            { '\'', TokenType.SingleQuote },
            { '"', TokenType.DoubleQuote },
        };
    }
}
