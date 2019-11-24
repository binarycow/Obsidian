using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obsidian.Lexing
{
    public static class TokenTypesExtensions
    {
        public static bool IsOpeningBrace(this TokenTypes tokenType)
        {
            return tokenType switch
            {
                TokenTypes.CurlyBrace_Open => true,
                TokenTypes.SquareBrace_Open => true,
                TokenTypes.Paren_Open => true,
                _ => false,
            };
        }
        public static bool IsClosingBrace(this TokenTypes tokenType)
        {
            return tokenType switch
            {
                TokenTypes.CurlyBrace_Close => true,
                TokenTypes.SquareBrace_Close => true,
                TokenTypes.Paren_Close => true,
                _ => false,
            };
        }
        public static bool IsMatchingBrace(this TokenTypes firstTokenType, TokenTypes secondTokenType)
        {
            return firstTokenType switch
            {
                TokenTypes.CurlyBrace_Open => secondTokenType == TokenTypes.CurlyBrace_Close,
                TokenTypes.CurlyBrace_Close => secondTokenType == TokenTypes.CurlyBrace_Open,
                TokenTypes.SquareBrace_Open => secondTokenType == TokenTypes.SquareBrace_Close,
                TokenTypes.SquareBrace_Close => secondTokenType == TokenTypes.SquareBrace_Open,
                TokenTypes.Paren_Open => secondTokenType == TokenTypes.Paren_Close,
                TokenTypes.Paren_Close => secondTokenType == TokenTypes.Paren_Open,
                _ => false,
            };
        }
    }
}
