using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obsidian.Lexing
{
    internal static class TokenTypesExtensions
    {
        internal static bool IsOpeningBrace(this TokenType tokenType)
        {
            return tokenType switch
            {
                TokenType.CurlyBrace_Open => true,
                TokenType.SquareBrace_Open => true,
                TokenType.Paren_Open => true,
                _ => false,
            };
        }
        internal static bool IsClosingBrace(this TokenType tokenType)
        {
            return tokenType switch
            {
                TokenType.CurlyBrace_Close => true,
                TokenType.SquareBrace_Close => true,
                TokenType.Paren_Close => true,
                _ => false,
            };
        }
        internal static bool IsMatchingBrace(this TokenType firstTokenType, TokenType secondTokenType)
        {
            return firstTokenType switch
            {
                TokenType.CurlyBrace_Open => secondTokenType == TokenType.CurlyBrace_Close,
                TokenType.CurlyBrace_Close => secondTokenType == TokenType.CurlyBrace_Open,
                TokenType.SquareBrace_Open => secondTokenType == TokenType.SquareBrace_Close,
                TokenType.SquareBrace_Close => secondTokenType == TokenType.SquareBrace_Open,
                TokenType.Paren_Open => secondTokenType == TokenType.Paren_Close,
                TokenType.Paren_Close => secondTokenType == TokenType.Paren_Open,
                _ => false,
            };
        }
    }
}
