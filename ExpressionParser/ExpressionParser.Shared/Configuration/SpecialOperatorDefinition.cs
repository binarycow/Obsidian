using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using ExpressionParser.Lexing;

namespace ExpressionParser.Configuration
{
    internal class SpecialOperatorDefinition : OperatorDefinition
    {
        private SpecialOperatorDefinition(string text, TokenType? secondaryTokenType, int precedence, SpecialOperatorType operatorType, 
            TokenType? argumentSeperator = null, TokenType? endingText = null, int minArguments = 0, int maxArguments = 0) 
            : base(text, secondaryTokenType, precedence, OperandCount.Binary)
        {
            OperatorType = operatorType;
            EndingToken = endingText;
            MinimumArguments = minArguments;
            MaximumArguments = maxArguments;
            ArgumentSeperator = argumentSeperator;
        }

        internal TokenType? ArgumentSeperator { get; }
        internal TokenType? EndingToken { get; }
        internal int MinimumArguments { get; }
        internal int MaximumArguments { get; }

        internal SpecialOperatorType OperatorType { get; }

        internal static SpecialOperatorDefinition Create(string text, TokenType? secondaryTokenType, int precedence, SpecialOperatorType operatorType, TokenType argumentSeperator,
            TokenType endingText, int minArguments, int maxArguments = int.MaxValue)
        {
            if (minArguments < 0) throw new ArgumentOutOfRangeException(nameof(minArguments), minArguments,
                string.Format(
                    CultureInfo.InvariantCulture,
                    ExpressionParserStrings.ResourceManager.GetString("OperatorError_MinArgsInvalid", CultureInfo.InvariantCulture),
                    nameof(minArguments)
                )
            );

            if (maxArguments < minArguments) throw new ArgumentOutOfRangeException(nameof(maxArguments), maxArguments,
                string.Format(
                    CultureInfo.InvariantCulture,
                    ExpressionParserStrings.ResourceManager.GetString("OperatorError_MaxArgsInvalid", CultureInfo.InvariantCulture),
                    nameof(maxArguments), nameof(minArguments)
                )
            );
            return new SpecialOperatorDefinition(text, secondaryTokenType, precedence, operatorType, argumentSeperator, endingText, minArguments, maxArguments);
        }

        internal static SpecialOperatorDefinition Create(string text, int precedence, SpecialOperatorType operatorType, TokenType argumentSeperator,
            TokenType endingText, int minArguments, int maxArguments = int.MaxValue)
        {
            return Create(text, null, precedence, operatorType, argumentSeperator, endingText, minArguments, maxArguments);
        }

        internal static SpecialOperatorDefinition Create(string text, int precedence, SpecialOperatorType operatorType)
        {
            return new SpecialOperatorDefinition(text, null, precedence, operatorType);
        }
    }
}
