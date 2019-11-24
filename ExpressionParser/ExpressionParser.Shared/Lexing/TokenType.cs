using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressionParser.Lexing
{
    public enum TokenType
    {
        Unknown = 0,
        FloatingLiteral,
        IntegerLiteral,
        StringLiteral,
        CharacterLiteral,
        Identifier,
        WhiteSpace,
        Operator,
        NullLiteral,
        Comma,
        Colon,
        Paren_Open,
        Paren_Close,
        SquareBrace_Open,
        SquareBrace_Close,
        CurlyBrace_Open,
        CurlyBrace_Close,
        SingleQuote,
        DoubleQuote,
    }
}
