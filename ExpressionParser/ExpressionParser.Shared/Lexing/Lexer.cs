using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Common;
using Common.Collections;
using Common.LookaroundEnumerator;
using ExpressionParser.Configuration;
using ExpressionParser.Exceptions;

namespace ExpressionParser.Lexing
{
    public class Lexer
    {
        private const int MIN_LOOKAHEAD_COUNT = 1;

        public Lexer(ILanguageDefinition languageDefinition)
        {
            LanguageDefinition = languageDefinition;
            _Delegates = new TryReadDelegate[]
            {
                TryReadCharacterLiteral,
                TryReadStringLiteral,
                TryReadNumericLiteral,
                TryReadWhiteSpace,
                TryReadOperator,
                TryReadIdentifier,
                TryReadSingleChar,
            };
            _Operators = languageDefinition.Operators.ToDictionary(operatorDef => operatorDef.Text.ToCharArray());

            _LookaheadCount = (byte)Math.Max(_Operators.Keys.Max(arr => arr.Length), MIN_LOOKAHEAD_COUNT);
        }
        public ILanguageDefinition LanguageDefinition { get; }

        private TryReadDelegate[] _Delegates;
        protected virtual TryReadDelegate[] Delegates => _Delegates;

        private readonly Dictionary<char[], OperatorDefinition> _Operators;
        private readonly byte _LookaheadCount;


        public delegate bool TryReadDelegate(ILookaroundEnumerator<char> enumerator, [NotNullWhen(true)]out Token? token);


        public IEnumerable<Token> Tokenize(IEnumerable<char> inputText)
        {
            using var enumerator = LookaroundEnumeratorFactory.CreateLookaroundEnumerator(inputText, _LookaheadCount);
            var DEBUG = enumerator as EnumerableLookaroundEnumerator<char>;

            using var checkoutRecord_UnknownChars = StringBuilderPool.Instance.Checkout();
            var currentUnknownChars = checkoutRecord_UnknownChars.CheckedOutObject;
            var hasUnknown = false;
            while (enumerator.MoveNext())
            {
                var foundToken = Delegates.Select(del =>
                {
                    var Success = del(enumerator, out var Result);
                    return new { Success, Result };
                }).FirstOrDefault(res => res.Success)?.Result;
                if(foundToken == default)
                {
                    hasUnknown = true;
                    currentUnknownChars.Append(enumerator.Current);
                }
                else
                {
                    if(hasUnknown)
                    {
                        yield return new Token(TokenType.Unknown, currentUnknownChars);
                        currentUnknownChars.Clear();
                        hasUnknown = false;
                    }
                    yield return foundToken;
                }
            }
            if (hasUnknown)
            {
                yield return new Token(TokenType.Unknown, currentUnknownChars);
            }
        }
        public virtual bool TryReadSingleChar(ILookaroundEnumerator<char> enumerator, [NotNullWhen(true)]out Token? token)
        {
            token = LanguageDefinition.SingleCharTokens.TryGetValue(enumerator.Current, out var tokenType) ?
                new Token(tokenType, enumerator.Current) :
                default;
            return token != default;
        }
        public virtual bool TryReadWhiteSpace(ILookaroundEnumerator<char> enumerator, [NotNullWhen(true)]out Token? token)
        {
            token = default;
            if (enumerator.Current.IsWhiteSpace() == false)
            {
                return false;
            }
            using var checkoutRecord = StringBuilderPool.Instance.Checkout();
            var stringBuilder = checkoutRecord.CheckedOutObject;
            stringBuilder.Append(enumerator.Current);

            while (enumerator.TryGetNext(out var nextChar) && nextChar.IsWhiteSpace())
            {
                stringBuilder.Append(enumerator.MoveNextAndGetValue(out _));
            }
            token = new Token(TokenType.WhiteSpace, stringBuilder);
            return true;
        }

        public virtual bool TryReadOperator(ILookaroundEnumerator<char> enumerator, [NotNullWhen(true)]out Token? token)
        {
            token = default;
            var initialPossibleOperators = _Operators.Keys.Where(operatorArr => operatorArr[0] == enumerator.Current).ToArray();
            if (initialPossibleOperators.Length == 0)
            {
                return false;
            }

            List<char[]> possibleOperators = new List<char[]>();
            foreach(var op in initialPossibleOperators.OrderByDescending(x => x.Length))
            {
                if(op[op.Length - 1].IsLetter())
                {
                    // If the operator ends in a letter (like "is") - then the *NEXT* char after it should _NOT_ be a letter.
                    if(enumerator.TryGetNext(out var nextChar, op.Length) && nextChar.IsLetter())
                    {
                        continue; // Skip this operator - the next character is a letter, and can't properly terminate the operator
                    }
                }

                var valid = true;
                for (var index = op.Length - 1; index > 0; --index)
                {
                    if ((enumerator.TryGetNext(out var nextChar, index) == false) || nextChar != op[index])
                    {
                        valid = false;
                        break;
                    }
                }
                if (valid == false) continue;
                if (enumerator.Current != op[0]) continue;
                possibleOperators.Add(op);
            }
            if (possibleOperators.Count == 0) return false;
            possibleOperators = possibleOperators.Distinct(CharArrayEqualityComparer.Instance).ToList();
            if (possibleOperators.Count >= 2) throw new NotImplementedException();
            token = new Token(TokenType.Operator, _Operators[possibleOperators[0]].Text);
            enumerator.MoveNext(possibleOperators[0].Length - 1);
            return true;
        }
        public virtual bool TryReadIdentifier(ILookaroundEnumerator<char> enumerator, [NotNullWhen(true)]out Token? token)
        {
            token = default;
            if(enumerator.Current.IsLetter() == false && enumerator.Current != '_')
            {
                return false;
            }
            using var checkoutRecord = StringBuilderPool.Instance.Checkout();
            var stringBuilder = checkoutRecord.CheckedOutObject;
            stringBuilder.Append(enumerator.Current);
            while(enumerator.TryGetNext(out var nextChar) && nextChar.IsLetter() || nextChar.IsDigit() || nextChar == '_')
            {
                stringBuilder.Append(enumerator.MoveNextAndGetValue(out _));
            }
            token = new Token(TokenType.Identifier, stringBuilder);
            return true;
        }

        public virtual bool TryReadStringLiteral(ILookaroundEnumerator<char> enumerator, [NotNullWhen(true)]out Token? token)
        {
            token = default;
            if(enumerator.Current != '"')
            {
                return false;
            }
            throw new NotImplementedException();
        }
        public virtual bool TryReadCharacterLiteral(ILookaroundEnumerator<char> enumerator, [NotNullWhen(true)]out Token? token)
        {
            token = default;
            var escaped = false;
            if(enumerator.Current != '\'')
            {
                return false;
            }
            enumerator.MoveNext(); //Eat the quote.
            if(enumerator.TryGetNext(out var nextChar) == false)
            {
                throw new LexingException("Newline in constant");
            }
            if(enumerator.Current == '\'')
            {
                throw new LexingException("Empty character literal");
            }
            if(enumerator.Current == '\\')
            {
                if(nextChar.IsValidEscapedChar() == false)
                {
                    throw new LexingException("Unrecognized escape sequence");
                }
                enumerator.MoveNext(); // Pass the backslash
                escaped = true;
            }
            if(enumerator.TryGetNext(out nextChar) == false)
            {
                throw new LexingException("Newline in constant");
            }
            if(nextChar != '\'')
            {
                throw new LexingException("Too many characters in character literal");
            }
            token = new Token(TokenType.CharacterLiteral, escaped ? enumerator.Current.Escape() : enumerator.Current);
            enumerator.MoveNext(); //Move to quote
            return true;
        }


        public virtual bool TryReadNumericLiteral(ILookaroundEnumerator<char> enumerator, [NotNullWhen(true)]out Token? token)
        {
            token = default;
            if(enumerator.Current.IsDigit() == false)
            {
                return false;
            }
            using var checkoutRecord = StringBuilderPool.Instance.Checkout();
            var stringBuilder = checkoutRecord.CheckedOutObject;
            stringBuilder.Append(enumerator.Current);
            var hasDecimal = false;

            // Get the portion before the decimal point...
            while(enumerator.TryGetNext(out var nextChar) && nextChar.IsDigit())
            {
                stringBuilder.Append(enumerator.MoveNextAndGetValue(out _));
            }

            // Attempt to get the decimal point
            if(enumerator.TryGetNext(out var decimalPoint) && decimalPoint == '.' && 
                enumerator.TryGetNext(out var decimalDigit, 2) && decimalDigit.IsDigit())
            {
                hasDecimal = true;
                stringBuilder.Append(enumerator.MoveNextAndGetValue(out _));
            }

            // Get the portion after the decimal point...
            while (hasDecimal && enumerator.TryGetNext(out var nextChar) && nextChar.IsDigit())
            {
                stringBuilder.Append(enumerator.MoveNextAndGetValue(out _));
            }

            token = new Token(hasDecimal ? TokenType.FloatingLiteral : TokenType.IntegerLiteral, stringBuilder);
            return true;
        }


        public bool? TryReadOnlyOneToken(IEnumerable<char> inputText, [NotNullWhen(true)]out Token? firstToken)
        {
            firstToken = default;
            bool? result = null;
            using var enumerator = LookaroundEnumeratorFactory.CreateLookaroundEnumerator(inputText, _LookaheadCount);
            var continueLoop = true;
            while (continueLoop && enumerator.MoveNext())
            {
                foreach(var del in Delegates)
                {
                    if(del(enumerator, out firstToken))
                    {
                        continueLoop = false;
                        result = true;
                        break;
                    }
                }
            }
            if (firstToken != default) enumerator.MoveNext();
            if(enumerator.State != EnumeratorState.Complete)
            {
                result = false;
            }
            return result;
        }

    }
}
