using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Collections;
using Obsidian;

namespace Obsidian.Lexing
{
    public class Lexer
    {
        public Lexer(JinjaEnvironment environment)
        {
            Environment = environment;
            _Delegates = new TryDelegate[]
            {
                TryNewLine,
                TryWhiteSpace,
                TryKeyword,
                TrySingleChar,
            };
            _KeywordLookups.Add(environment.Settings.BlockStartString.ToCharArray(), TokenTypes.StatementStart);
            _KeywordLookups.Add(environment.Settings.BlockEndString.ToCharArray(), TokenTypes.StatementEnd);
            _KeywordLookups.Add(environment.Settings.VariableStartString.ToCharArray(), TokenTypes.ExpressionStart);
            _KeywordLookups.Add(environment.Settings.VariableEndString.ToCharArray(), TokenTypes.ExpressionEnd);
            _KeywordLookups.Add(environment.Settings.CommentStartString.ToCharArray(), TokenTypes.CommentStart);
            _KeywordLookups.Add(environment.Settings.CommentEndString.ToCharArray(), TokenTypes.CommentEnd);
            if (environment.Settings.LineCommentPrefix != null)
            {
                _KeywordLookups.Add(environment.Settings.LineCommentPrefix.ToCharArray(), TokenTypes.LineComment);
            }
            if (environment.Settings.LineStatementPrefix != null)
            {
                _KeywordLookups.Add(environment.Settings.LineStatementPrefix.ToCharArray(), TokenTypes.LineStatement);
            }
        }
        public JinjaEnvironment Environment { get; }


        private delegate bool TryDelegate(ILookaroundEnumerator<char> enumerator, out Token? token);
        private TryDelegate[] _Delegates = new TryDelegate[] { };

        private Dictionary<char, Func<Token>> _SingleChar = new Dictionary<char, Func<Token>>
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
        };
        private Dictionary<char[], TokenTypes> _KeywordLookups = new Dictionary<char[], TokenTypes>
        {
            { "for".ToCharArray(), TokenTypes.Keyword_For },
            { "endfor".ToCharArray(), TokenTypes.Keyword_EndFor },
            { "in".ToCharArray(), TokenTypes.Keyword_In },
            { "if".ToCharArray(), TokenTypes.Keyword_If },
            { "else".ToCharArray(), TokenTypes.Keyword_Else },
            { "elif".ToCharArray(), TokenTypes.Keyword_Elif },
            { "endif".ToCharArray(), TokenTypes.Keyword_Endif },
            { "block".ToCharArray(), TokenTypes.Keyword_Block },
            { "endblock".ToCharArray(), TokenTypes.Keyword_EndBlock },
            { "extends".ToCharArray(), TokenTypes.Keyword_Extends },
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
        public IEnumerable<Token> Tokenize(IEnumerable<char> source)
        {
            var maximumLength = _KeywordLookups.Keys.Max(keyword => keyword.Length);
            var enumerator = LookaroundEnumeratorFactory.CreateLookaroundEnumerator(source, (byte)maximumLength);
            return Tokenize(enumerator);
        }

        public IEnumerable<Token> Tokenize(ILookaroundEnumerator<char> enumerator)
        {
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
                        yield return new Token(TokenTypes.Unknown, new string(currentUnknown.ToArray()));
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
                yield return new Token(TokenTypes.Unknown, new string(currentUnknown.ToArray()));
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

            while(enumerator.TryGetNext(out var nextChar) && nextChar.IsWhiteSpace())
            {
                var result = enumerator.MoveNext();
                queue.Enqueue(enumerator.Current);
                if (result == false)
                {
                    break;
                }
            }
            token = new Token(TokenTypes.WhiteSpace, queue);
            return true;
        }

    }
}
