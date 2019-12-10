using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Collections;
using Obsidian;

namespace Obsidian.Lexing
{
    internal class Lexer
    {
        internal Lexer(JinjaEnvironment environment)
        {
            Environment = environment;
            _Delegates = new TryDelegate[]
            {
                TryNewLine,
                TryWhiteSpace,
                TryKeyword,
                TrySingleChar,
            };
            _KeywordLookups.Add(environment.Settings.BlockStartString.ToCharArray(), TokenType.StatementStart);
            _KeywordLookups.Add(environment.Settings.BlockEndString.ToCharArray(), TokenType.StatementEnd);
            _KeywordLookups.Add(environment.Settings.VariableStartString.ToCharArray(), TokenType.ExpressionStart);
            _KeywordLookups.Add(environment.Settings.VariableEndString.ToCharArray(), TokenType.ExpressionEnd);
            _KeywordLookups.Add(environment.Settings.CommentStartString.ToCharArray(), TokenType.CommentStart);
            _KeywordLookups.Add(environment.Settings.CommentEndString.ToCharArray(), TokenType.CommentEnd);
            if (environment.Settings.LineCommentPrefix != null)
            {
                _KeywordLookups.Add(environment.Settings.LineCommentPrefix.ToCharArray(), TokenType.LineComment);
            }
            if (environment.Settings.LineStatementPrefix != null)
            {
                _KeywordLookups.Add(environment.Settings.LineStatementPrefix.ToCharArray(), TokenType.LineStatement);
            }
        }
        internal JinjaEnvironment Environment { get; }


        private delegate bool TryDelegate(ILookaroundEnumerator<char> enumerator, out Token? token);
        private readonly TryDelegate[] _Delegates = Array.Empty<TryDelegate>();

        private readonly Dictionary<char, Func<Token>> _SingleChar = new Dictionary<char, Func<Token>>
        {
            {',', () => Token.Comma },
            {':', () => Token.Colon },
            {'[', () => Token.SquareBrace_Open },
            {']', () => Token.SquareBrace_Close },
            {'{', () => Token.CurlyBrace_Open },
            {'}', () => Token.CurlyBrace_Close },
            {'(', () => Token.Paren_Open },
            {')', () => Token.Paren_Close },
            {'+', () => Token.Plus },
            {'-', () => Token.Minus },
            {'=', () => Token.Equal },
        };
        private readonly Dictionary<char[], TokenType> _KeywordLookups = new Dictionary<char[], TokenType>
        {
            { "for".ToCharArray(), TokenType.Keyword_For },
            { "endfor".ToCharArray(), TokenType.Keyword_EndFor },
            { "in".ToCharArray(), TokenType.Keyword_In },
            { "if".ToCharArray(), TokenType.Keyword_If },
            { "else".ToCharArray(), TokenType.Keyword_Else },
            { "elif".ToCharArray(), TokenType.Keyword_Elif },
            { "endif".ToCharArray(), TokenType.Keyword_Endif },
            { "block".ToCharArray(), TokenType.Keyword_Block },
            { "endblock".ToCharArray(), TokenType.Keyword_EndBlock },
            { "extends".ToCharArray(), TokenType.Keyword_Extends },
            { "raw".ToCharArray(), TokenType.Keyword_Raw },
            { "endraw".ToCharArray(), TokenType.Keyword_EndRaw },
            { "macro".ToCharArray(), TokenType.Keyword_Macro },
            { "endmacro".ToCharArray(), TokenType.Keyword_EndMacro },
            { "call".ToCharArray(), TokenType.Keyword_Call },
            { "endcall".ToCharArray(), TokenType.Keyword_EndCall },
            { "filter".ToCharArray(), TokenType.Keyword_Filter },
            { "endfilter".ToCharArray(), TokenType.Keyword_EndFilter },
            { "set".ToCharArray(), TokenType.Keyword_Set },
            { "endset".ToCharArray(), TokenType.Keyword_EndSet },
        };
        private bool TryKeyword(ILookaroundEnumerator<char> enumerator, out Token? token)
        {
            token = default;
            var possibleKeywords = _KeywordLookups.Keys.OrderByDescending(keyword => keyword.Length);
            foreach (var possibleKeyword in possibleKeywords)
            {
                if (possibleKeyword[0] != enumerator.Current) continue;
                var valid = true;
                for (int index = 1; index < possibleKeyword.Length; ++index)
                {
                    if(enumerator.TryGetNext(out var nextChar, index) == false || nextChar != possibleKeyword[index])
                    {
                        valid = false;
                        break;
                    }
                }
                if (valid)
                {
                    for (var i = 0; i < possibleKeyword.Length - 1; ++i)
                    {
                        enumerator.MoveNext();
                    }
                    token = new Token(_KeywordLookups[possibleKeyword], new string(possibleKeyword));
                    return true;
                }
            }
            return false;
        }

        internal IEnumerable<Token> Tokenize(IEnumerable<char> source)
        {
            var maximumLength = _KeywordLookups.Keys.Max(keyword => keyword.Length);
            using var enumerator = LookaroundEnumeratorFactory.CreateLookaroundEnumerator(source, (byte)maximumLength);

            var currentUnknown = new Queue<char>();
            while (enumerator.MoveNext())
            {
                var token = _Delegates.Select(del =>
                {
                    var Success = del(enumerator, out var Token);
                    return new
                    {
                        Success,
                        Token
                    };
                }).FirstOrDefault(x => x.Success)?.Token;
                if (token != default)
                {
                    if (currentUnknown.Count != 0)
                    {
                        yield return new Token(TokenType.Unknown, new string(currentUnknown.ToArray()));
                        currentUnknown.Clear();
                    }
                    yield return token;
                }
                else
                {
                    currentUnknown.Enqueue(enumerator.Current);
                }
            }
            if (currentUnknown.Count != 0)
            {
                yield return new Token(TokenType.Unknown, new string(currentUnknown.ToArray()));
            }
        }

        private bool TrySingleChar(ILookaroundEnumerator<char> enumerator, out Token? token)
        {
            _SingleChar.TryGetValue(enumerator.Current, out var tokenFunc);
            token = tokenFunc?.Invoke();
            return token != default;
        }
        private bool TryNewLine(ILookaroundEnumerator<char> enumerator, out Token? token)
        {
            token = default;
            switch (enumerator.Current)
            {
                case '\r':
                    if(enumerator.TryGetNext(out var nextChar) && nextChar == '\n')
                    {
                        enumerator.MoveNext();
                        token = Token.NewLine;
                        return true;
                    }
                    break;
                case '\n':
                    token = Token.NewLine;
                    return true;
            }
            return false;
        }
        private bool TryWhiteSpace(ILookaroundEnumerator<char> enumerator, out Token? token)
        {
            token = default;
            if (enumerator.Current.IsWhiteSpace() == false)
            {
                return false;
            }
            var queue = new Queue<char>();
            queue.Enqueue(enumerator.Current);

            while(enumerator.TryGetNext(out var nextChar) && nextChar.IsWhiteSpace() && nextChar.IsNotNewLine())
            {
                var result = enumerator.MoveNext();
                queue.Enqueue(enumerator.Current);
                if (result == false)
                {
                    break;
                }
            }
            token = new Token(TokenType.WhiteSpace, queue);
            return true;
        }

    }
}
