using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obsidian.Lexing
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    internal enum TokenType
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
        Keyword_Raw,
        Keyword_EndRaw,
        Keyword_Macro,
        Keyword_EndMacro,
        Colon,
        SquareBrace_Open,
        Paren_Open,
        SquareBrace_Close,
        Paren_Close,
        CurlyBrace_Open,
        CurlyBrace_Close,
        Keyword_Block,
        Keyword_EndBlock,
        Keyword_Extends,
        Keyword_Call,
        Keyword_EndCall,
        Keyword_Filter,
        Keyword_EndFilter,
        Keyword_Set,
        Keyword_EndSet,
        Equal,
        Keyword_Include,
        Keyword_Ignore,
        Keyword_Missing,
        Keyword_With,
        Keyword_Without,
        Keyword_Context,
    }
}
