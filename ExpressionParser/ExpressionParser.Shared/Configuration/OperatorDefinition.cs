using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using ExpressionParser.Lexing;

namespace ExpressionParser.Configuration
{
    public abstract class OperatorDefinition
    {
        internal OperatorDefinition(string text, int precedence, OperandCount operandCount)
        {
            Text = text;
            Precedence = precedence;
            OperandCount = operandCount;
        }

        internal OperatorDefinition(string text, TokenType? secondaryTokenType, int precedence, OperandCount operandCount) : this(text, precedence, operandCount)
        {
            SecondaryTokenType = secondaryTokenType;
        }

        public TokenType? SecondaryTokenType { get; }
        public string Text { get; }
        public int Precedence { get; }
        public OperandCount OperandCount { get; }

        public static OperatorDefinition CreateMemberAccess([Localizable(false)]string text, int precedence)
        {
            return SpecialOperatorDefinition.Create(text, precedence, SpecialOperatorType.PropertyAccess);
        }

        public static OperatorDefinition CreatePipeline([Localizable(false)]string text, int precedence)
        {
            return SpecialOperatorDefinition.Create(text, precedence, SpecialOperatorType.Pipeline);
        }

        public static OperatorDefinition CreateMethod([Localizable(false)]string startText, TokenType argSeperator, TokenType endText, int precedence)
        {
            return SpecialOperatorDefinition.Create(startText, precedence, SpecialOperatorType.MethodCall, argSeperator, endText, 0, int.MaxValue);
        }

        public static OperatorDefinition CreateIndex([Localizable(false)]string startText, TokenType argSeperator, TokenType endText, int precedence)
        {
            return SpecialOperatorDefinition.Create(startText, precedence, SpecialOperatorType.Index, argSeperator, endText, 1, int.MaxValue);
        }
        public static OperatorDefinition CreateMethod([Localizable(false)]string startText, TokenType secondaryTokenType, TokenType argSeperator, TokenType endText, int precedence)
        {
            return SpecialOperatorDefinition.Create(startText, secondaryTokenType, precedence, SpecialOperatorType.MethodCall, argSeperator, endText, 0, int.MaxValue);
        }

        public static OperatorDefinition CreateIndex([Localizable(false)]string startText, TokenType secondaryTokenType, TokenType argSeperator, TokenType endText, int precedence)
        {
            return SpecialOperatorDefinition.Create(startText, secondaryTokenType, precedence, SpecialOperatorType.Index, argSeperator, endText, 1, int.MaxValue);
        }

        public static OperatorDefinition CreateBinary([Localizable(false)]string text, int precedence, OperatorType operatorType)
        {
            return new StandardOperatorDefinition(text, precedence, operatorType, OperandCount.Binary);
        }

        public static OperatorDefinition CreateUnary([Localizable(false)]string text, int precedence, OperatorType operatorType)
        {
            return new StandardOperatorDefinition(text, precedence, operatorType, OperandCount.Unary);
        }

    }
}
