using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Obsidian.Lexing
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    internal class Token
    {
        internal Token(TokenType tokenType, string value)
        {
            TokenType = tokenType;
            Value = value;
        }
        internal Token(TokenType tokenType, IEnumerable<char> value) : this(tokenType, new string(value.ToArray())) { }



        internal virtual void ToOriginalText(StringBuilder stringBuilder)
        {
            stringBuilder.Append(Value);
        }

        private string DebuggerDisplay => $"{nameof(Token)} {TokenType} : \"{Value.WhiteSpaceEscape()}\"";

        internal TokenType TokenType { get; }
        internal string Value { get; }

        private static readonly Lazy<Token> _NewLine = new Lazy<Token>(() => new Token(TokenType.NewLine, "\n"));
        private static readonly Lazy<Token> _StatementStart = new Lazy<Token>(() => new Token(TokenType.StatementStart, "{%"));
        private static readonly Lazy<Token> _StatementEnd = new Lazy<Token>(() => new Token(TokenType.StatementEnd, "%}"));
        private static readonly Lazy<Token> _ExpressionStart = new Lazy<Token>(() => new Token(TokenType.ExpressionStart, "{{"));
        private static readonly Lazy<Token> _ExpressionEnd = new Lazy<Token>(() => new Token(TokenType.ExpressionEnd, "}}"));
        private static readonly Lazy<Token> _CommentStart = new Lazy<Token>(() => new Token(TokenType.CommentStart, "{#"));
        private static readonly Lazy<Token> _CommentEnd = new Lazy<Token>(() => new Token(TokenType.CommentEnd, "#}"));
        private static readonly Lazy<Token> _Minus = new Lazy<Token>(() => new Token(TokenType.Minus, "-"));
        private static readonly Lazy<Token> _Plus = new Lazy<Token>(() => new Token(TokenType.Plus, "+"));
        private static readonly Lazy<Token> _Comma = new Lazy<Token>(() => new Token(TokenType.Comma, ","));
        private static readonly Lazy<Token> _Colon = new Lazy<Token>(() => new Token(TokenType.Colon, ":"));
        private static readonly Lazy<Token> _Equal = new Lazy<Token>(() => new Token(TokenType.Equal, "="));

        private static readonly Lazy<Token> _SquareBrace_Open = new Lazy<Token>(() => new Token(TokenType.SquareBrace_Open, "["));
        private static readonly Lazy<Token> _SquareBrace_Close = new Lazy<Token>(() => new Token(TokenType.SquareBrace_Close, "]"));
        private static readonly Lazy<Token> _CurlyBrace_Open = new Lazy<Token>(() => new Token(TokenType.CurlyBrace_Open, "{"));
        private static readonly Lazy<Token> _CurlyBrace_Close = new Lazy<Token>(() => new Token(TokenType.CurlyBrace_Close, "}"));
        private static readonly Lazy<Token> _Paren_Open = new Lazy<Token>(() => new Token(TokenType.Paren_Open, "("));
        private static readonly Lazy<Token> _Paren_Close = new Lazy<Token>(() => new Token(TokenType.Paren_Close, ")"));
        internal static Token NewLine { get { return _NewLine.Value; } }
        internal static Token StatementStart { get { return _StatementStart.Value; } }
        internal static Token StatementEnd { get { return _StatementEnd.Value; } }
        internal static Token ExpressionStart { get { return _ExpressionStart.Value; } }
        internal static Token ExpressionEnd { get { return _ExpressionEnd.Value; } }
        internal static Token CommentStart { get { return _CommentStart.Value; } }
        internal static Token CommentEnd { get { return _CommentEnd.Value; } }
        internal static Token Minus { get { return _Minus.Value; } }
        internal static Token Plus { get { return _Plus.Value; } }
        internal static Token Comma { get { return _Comma.Value; } }
        internal static Token Colon { get { return _Colon.Value; } }


        internal static Token SquareBrace_Open { get { return _SquareBrace_Open.Value; } }
        internal static Token SquareBrace_Close { get { return _SquareBrace_Close.Value; } }
        internal static Token CurlyBrace_Open { get { return _CurlyBrace_Open.Value; } }
        internal static Token CurlyBrace_Close { get { return _CurlyBrace_Close.Value; } }
        internal static Token Paren_Open { get { return _Paren_Open.Value; } }
        internal static Token Paren_Close { get { return _Paren_Close.Value; } }

        internal static Token Equal { get { return _Equal.Value; } }
    }
}
