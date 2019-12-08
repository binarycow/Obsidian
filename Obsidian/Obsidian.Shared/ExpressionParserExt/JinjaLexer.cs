using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using Common;
using Common.Collections;
using ExpressionParser;
using ExpressionParser.Exceptions;
using ExpressionParser.Lexing;

namespace Obsidian.ExpressionParserExt
{
    internal class JinjaLexer : Lexer
    {
        internal JinjaLexer(JinjaLanguageDefinition languageDefinition) : base(languageDefinition)
        {

        }

        internal override bool TryReadCharacterLiteral(ILookaroundEnumerator<char> enumerator, [NotNullWhen(true)] out Token? token)
        {
            token = default;
            return false;
        }

        internal override bool TryReadStringLiteral(ILookaroundEnumerator<char> enumerator, [NotNullWhen(true)] out Token? token)
        {
            token = default;
            if(enumerator.Current != '"' && enumerator.Current != '\'')
            {
                return false;
            }
            using var checkoutRecord = StringBuilderPool.Instance.Checkout();
            var stringBuilder = checkoutRecord.CheckedOutObject;

            var quoteChar = enumerator.Current;
            while(enumerator.MoveNext() && enumerator.Current != quoteChar)
            {
                if(enumerator.Current == '\\')
                {
                    if(enumerator.TryGetNext(out var nextChar) == false)
                    {
                        throw new LexingException(ExpressionParserStrings.ResourceManager.GetString("LexerError_UnrecognizedEscape", CultureInfo.InvariantCulture));
                    }
                    if(nextChar.IsValidEscapedChar() == false)
                    {
                        throw new LexingException(ExpressionParserStrings.ResourceManager.GetString("LexerError_UnrecognizedEscape", CultureInfo.InvariantCulture));
                    }
                    enumerator.MoveNext(); //Eat the backslash
                    stringBuilder.Append(enumerator.Current.Escape());
                    continue;
                }
                stringBuilder.Append(enumerator.Current);
            }
            if(enumerator.State == EnumeratorState.Complete)
            {
                throw new LexingException(ExpressionParserStrings.ResourceManager.GetString("LexerError_NewlineInConstant", CultureInfo.InvariantCulture));
            }
            if(enumerator.Current != quoteChar)
            {
                throw new LexingException($"Expected {quoteChar} : Encountered {enumerator.Current}");
            }
            token = new Token(TokenType.StringLiteral, null, stringBuilder);
            return true;
        }
    }
}
