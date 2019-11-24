using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obsidian.Lexing
{
    public enum TokenTypes
    {
        Unknown = 0,
        StatementStart,
        StatementEnd,
        ExpressionStart,
        ExpressionEnd,
        CommentStart,
        CommentEnd,
        LineStatement,
        LineComment,
        NewLine,
        WhiteSpace,
        Minus,
        Plus,
        Keyword_For,
        Comma,
        Keyword_In,
        Keyword_EndFor,
        Keyword_If,
        Keyword_Else,
        Keyword_Elif,
        Keyword_Endif,
        Colon,
        SquareBrace_Open,
        Paren_Open,
        SquareBrace_Close,
        Paren_Close,
        CurlyBrace_Open,
        CurlyBrace_Close,
    }
}
