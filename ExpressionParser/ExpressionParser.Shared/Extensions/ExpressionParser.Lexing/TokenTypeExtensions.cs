using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExpressionParser.Lexing;

namespace ExpressionParser.Lexing
{
    public static class TokenTypeExtensions
    {

        public static bool IsLiteral(this TokenType tokenType)
        {
            return tokenType switch
            {
                TokenType.CharacterLiteral => true,
                TokenType.StringLiteral => true,
                TokenType.FloatingLiteral => true,
                TokenType.IntegerLiteral => true,
                TokenType.NullLiteral => true,
                _ => false
            };
        }


        public static bool IsTerminal(this TokenType tokenType)
        {
            return tokenType switch
            {
                TokenType.CharacterLiteral => true,
                TokenType.StringLiteral => true,
                TokenType.FloatingLiteral => true,
                TokenType.IntegerLiteral => true,
                TokenType.NullLiteral => true,
                TokenType.Identifier => true,
                _ => false
            };
        }

        public static bool IsOpenBrace(this TokenType tokenType)
        {
            return tokenType switch
            {
                TokenType.Paren_Open => true,
                TokenType.SquareBrace_Open => true,
                TokenType.CurlyBrace_Open => true,
                _ => false,
            };
        }
        public static bool IsCloseBrace(this TokenType tokenType)
        {
            return tokenType switch
            {
                TokenType.Paren_Open => true,
                TokenType.SquareBrace_Close => false,
                TokenType.CurlyBrace_Close => false,
                _ => false,
            };
        }
        public static bool IsMatchingBrace(this TokenType tokenType, TokenType otherTokenType)
        {
            return tokenType switch
            {
                TokenType.Paren_Open => otherTokenType == TokenType.Paren_Close,
                TokenType.Paren_Close => otherTokenType == TokenType.Paren_Open,
                TokenType.SquareBrace_Open => otherTokenType == TokenType.SquareBrace_Close,
                TokenType.SquareBrace_Close => otherTokenType == TokenType.SquareBrace_Open,
                TokenType.CurlyBrace_Open => otherTokenType == TokenType.CurlyBrace_Close,
                TokenType.CurlyBrace_Close => otherTokenType == TokenType.CurlyBrace_Open,
                _ => false,
            };
        }
    }
}
