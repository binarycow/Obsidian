using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using Common;
using Common.Collections;
using Common.LookaroundEnumerator;
using ExpressionParser.Configuration;
using ExpressionParser.Exceptions;

namespace ExpressionParser.Lexing
{
    internal class Lexer
    {
        private const int _MIN_LOOKAHEAD_COUNT = 1;

        internal Lexer(ILanguageDefinition languageDefinition)
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

            _LookaheadCount = (byte)Math.Max(_Operators.Keys.Max(arr => arr.Length), _MIN_LOOKAHEAD_COUNT);
        }
        internal ILanguageDefinition LanguageDefinition { get; }

        private readonly TryReadDelegate[] _Delegates;
        protected virtual TryReadDelegate[] Delegates => _Delegates;

        private readonly Dictionary<char[], OperatorDefinition> _Operators;
        private readonly byte _LookaheadCount;


        internal delegate bool TryReadDelegate(ILookaroundEnumerator<char> enumerator, [NotNullWhen(true)]out Token? token);


        internal IEnumerable<Token> Tokenize(IEnumerable<char> inputText)
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
                if(foundToken == null)
                {
                    hasUnknown = true;
                    currentUnknownChars.Append(enumerator.Current);
                }
                else
                {
                    if(hasUnknown)
                    {
                        yield return new Token(TokenType.Unknown, null, currentUnknownChars);
                        currentUnknownChars.Clear();
                        hasUnknown = false;
                    }
                    yield return foundToken.Value;
                }
            }
            if (hasUnknown)
            {
                yield return new Token(TokenType.Unknown, null, currentUnknownChars);
            }
        }
        internal virtual bool TryReadSingleChar(ILookaroundEnumerator<char> enumerator, [NotNullWhen(true)]out Token? token)
        {
            token = LanguageDefinition.SingleCharTokens.TryGetValue(enumerator.Current, out var tokenType) ?
                new Token(tokenType, null, enumerator.Current) :
                default;
            return token != default;
        }
        internal virtual bool TryReadWhiteSpace(ILookaroundEnumerator<char> enumerator, [NotNullWhen(true)]out Token? token)
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
            token = new Token(TokenType.WhiteSpace, null, stringBuilder);
            return true;
        }

        internal virtual bool TryReadOperator(ILookaroundEnumerator<char> enumerator, [NotNullWhen(true)]out Token? token)
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
            token = new Token(TokenType.Operator, _Operators[possibleOperators[0]].SecondaryTokenType, _Operators[possibleOperators[0]].Text);
            enumerator.MoveNext(possibleOperators[0].Length - 1);
            return true;
        }
        internal virtual bool TryReadIdentifier(ILookaroundEnumerator<char> enumerator, [NotNullWhen(true)]out Token? token)
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
            token = new Token(TokenType.Identifier, null, stringBuilder);
            return true;
        }

        internal virtual bool TryReadStringLiteral(ILookaroundEnumerator<char> enumerator, [NotNullWhen(true)]out Token? token)
        {
            token = default;
            if(enumerator.Current != '"')
            {
                return false;
            }
            throw new NotImplementedException();
        }
        internal virtual bool TryReadCharacterLiteral(ILookaroundEnumerator<char> enumerator, [NotNullWhen(true)]out Token? token)
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
                throw new LexingException(ExpressionParserStrings.ResourceManager.GetString("LexerError_NewlineInConstant", CultureInfo.InvariantCulture));
            }
            if(enumerator.Current == '\'')
            {
                throw new LexingException(ExpressionParserStrings.ResourceManager.GetString("LexerError_EmptyCharacterLiteral", CultureInfo.InvariantCulture));
            }
            if(enumerator.Current == '\\')
            {
                if(nextChar.IsValidEscapedChar() == false)
                {
                    throw new LexingException(ExpressionParserStrings.ResourceManager.GetString("LexerError_UnrecognizedEscape", CultureInfo.InvariantCulture));
                }
                enumerator.MoveNext(); // Pass the backslash
                escaped = true;
            }
            if(enumerator.TryGetNext(out nextChar) == false)
            {
                throw new LexingException(ExpressionParserStrings.ResourceManager.GetString("LexerError_NewlineInConstant", CultureInfo.InvariantCulture));
            }
            if(nextChar != '\'')
            {
                throw new LexingException(ExpressionParserStrings.ResourceManager.GetString("LexerError_TooManyCharsInCharLiteral", CultureInfo.InvariantCulture));
            }
            token = new Token(TokenType.CharacterLiteral, null, escaped ? enumerator.Current.Escape() : enumerator.Current);
            enumerator.MoveNext(); //Move to quote
            return true;
        }


        internal virtual bool TryReadNumericLiteral(ILookaroundEnumerator<char> enumerator, [NotNullWhen(true)]out Token? token)
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

            token = new Token(hasDecimal ? TokenType.FloatingLiteral : TokenType.IntegerLiteral, null, stringBuilder);
            return true;
        }


        internal bool? TryReadOnlyOneToken(IEnumerable<char> inputText, [NotNullWhen(true)]out Token? firstToken)
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
