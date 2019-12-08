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
        ParenOpen,
        ParenClose,
        SquareBraceOpen,
        SquareBraceClose,
        CurlyBraceOpen,
        CurlyBraceClose,
        SingleQuote,
        DoubleQuote,
    }
}
