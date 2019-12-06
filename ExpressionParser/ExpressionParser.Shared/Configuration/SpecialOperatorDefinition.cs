using System;
using System.Collections.Generic;
using System.Text;
using ExpressionParser.Lexing;

namespace ExpressionParser.Configuration
{
    public class SpecialOperatorDefinition : OperatorDefinition
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

        public TokenType? ArgumentSeperator { get; }
        public TokenType? EndingToken { get; }
        public int MinimumArguments { get; }
        public int MaximumArguments { get; }

        public SpecialOperatorType OperatorType { get; }

        public static SpecialOperatorDefinition Create(string text, TokenType? secondaryTokenType, int precedence, SpecialOperatorType operatorType, TokenType argumentSeperator,
            TokenType endingText, int minArguments, int maxArguments = int.MaxValue)
        {
            if (minArguments < 0) throw new ArgumentOutOfRangeException(nameof(minArguments), minArguments,
                $"{nameof(minArguments)} must be greater than or equal to zero.");
            if (maxArguments < minArguments) throw new ArgumentOutOfRangeException(nameof(maxArguments), maxArguments,
                $"{nameof(maxArguments)} must be greater than or equal to {nameof(minArguments)}.");
            return new SpecialOperatorDefinition(text, secondaryTokenType, precedence, operatorType, argumentSeperator, endingText, minArguments, maxArguments);
        }

        public static SpecialOperatorDefinition Create(string text, int precedence, SpecialOperatorType operatorType, TokenType argumentSeperator,
            TokenType endingText, int minArguments, int maxArguments = int.MaxValue)
        {
            return Create(text, null, precedence, operatorType, argumentSeperator, endingText, minArguments, maxArguments);
        }

        public static SpecialOperatorDefinition Create(string text, int precedence, SpecialOperatorType operatorType)
        {
            return new SpecialOperatorDefinition(text, null, precedence, operatorType);
        }
    }
}
