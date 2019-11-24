using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Common;
using Common.Collections;
using ExpressionParser.Exceptions;
using ExpressionParser.Lexing;

namespace Obsidian.ExpressionParserExt
{
    public class JinjaLexer : Lexer
    {
        public JinjaLexer(JinjaLanguageDefinition languageDefinition) : base(languageDefinition)
        {

        }

        public override bool TryReadCharacterLiteral(ILookaroundEnumerator<char> enumerator, [NotNullWhen(true)] out Token? token)
        {
            token = default;
            return false;
        }

        public override bool TryReadStringLiteral(ILookaroundEnumerator<char> enumerator, [NotNullWhen(true)] out Token? token)
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
                        throw new LexingException("Unrecognized escape sequence");
                    }
                    if(nextChar.IsValidEscapedChar() == false)
                    {
                        throw new LexingException("Unrecognized escape sequence");
                    }
                    enumerator.MoveNext(); //Eat the backslash
                    stringBuilder.Append(enumerator.Current.Escape());
                    continue;
                }
                stringBuilder.Append(enumerator.Current);
            }
            if(enumerator.State == EnumeratorState.Complete)
            {
                throw new LexingException("Newline in constant");
            }
            if(enumerator.Current != quoteChar)
            {
                throw new LexingException($"Expected {quoteChar} : Encountered {enumerator.Current}");
            }
            token = new Token(TokenType.StringLiteral, stringBuilder);
            return true;
        }
    }
}
