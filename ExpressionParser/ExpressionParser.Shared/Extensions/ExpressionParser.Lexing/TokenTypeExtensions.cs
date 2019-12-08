using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExpressionParser.Lexing;

namespace ExpressionParser.Lexing
{
    internal static class TokenTypeExtensions
    {

        internal static bool IsLiteral(this TokenType tokenType)
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


        internal static bool IsTerminal(this TokenType tokenType)
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

        internal static bool IsOpenBrace(this TokenType tokenType)
        {
            return tokenType switch
            {
                TokenType.ParenOpen => true,
                TokenType.SquareBraceOpen => true,
                TokenType.CurlyBraceOpen => true,
                _ => false,
            };
        }
        internal static bool IsCloseBrace(this TokenType tokenType)
        {
            return tokenType switch
            {
                TokenType.ParenOpen => true,
                TokenType.SquareBraceClose => false,
                TokenType.CurlyBraceClose => false,
                _ => false,
            };
        }
        internal static bool IsMatchingBrace(this TokenType tokenType, TokenType otherTokenType)
        {
            return tokenType switch
            {
                TokenType.ParenOpen => otherTokenType == TokenType.ParenClose,
                TokenType.ParenClose => otherTokenType == TokenType.ParenOpen,
                TokenType.SquareBraceOpen => otherTokenType == TokenType.SquareBraceClose,
                TokenType.SquareBraceClose => otherTokenType == TokenType.SquareBraceOpen,
                TokenType.CurlyBraceOpen => otherTokenType == TokenType.CurlyBraceClose,
                TokenType.CurlyBraceClose => otherTokenType == TokenType.CurlyBraceOpen,
                _ => false,
            };
        }
        internal static bool IsMatchingBrace(this TokenType tokenType, TokenType? otherTokenType)
        {
            return otherTokenType.HasValue && tokenType.IsMatchingBrace(otherTokenType.Value);
        }

    }
}
