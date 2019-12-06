﻿using System;
using System.Collections.Generic;
using System.Text;
using ExpressionParser.Lexing;

namespace ExpressionParser.Configuration
{
    public abstract class OperatorDefinition
    {
        protected OperatorDefinition(string text, int precedence, OperandCount operandCount)
        {
            Text = text;
            Precedence = precedence;
            OperandCount = operandCount;
        }

        protected OperatorDefinition(string text, TokenType? secondaryTokenType, int precedence, OperandCount operandCount) : this(text, precedence, operandCount)
        {
            SecondaryTokenType = secondaryTokenType;
        }

        public TokenType? SecondaryTokenType { get; }
        public string Text { get; }
        public int Precedence { get; }
        public OperandCount OperandCount { get; }

        internal static OperatorDefinition CreateMemberAccess(string text, int precedence)
        {
            return SpecialOperatorDefinition.Create(text, precedence, SpecialOperatorType.PropertyAccess);
        }

        internal static OperatorDefinition CreatePipeline(string text, int precedence)
        {
            return SpecialOperatorDefinition.Create(text, precedence, SpecialOperatorType.Pipeline);
        }

        internal static OperatorDefinition CreateMethod(string startText, TokenType argSeperator, TokenType endText, int precedence)
        {
            return SpecialOperatorDefinition.Create(startText, precedence, SpecialOperatorType.MethodCall, argSeperator, endText, 0, int.MaxValue);
        }

        internal static OperatorDefinition CreateIndex(string startText, TokenType argSeperator, TokenType endText, int precedence)
        {
            return SpecialOperatorDefinition.Create(startText, precedence, SpecialOperatorType.Index, argSeperator, endText, 1, int.MaxValue);
        }
        internal static OperatorDefinition CreateMethod(string startText, TokenType secondaryTokenType, TokenType argSeperator, TokenType endText, int precedence)
        {
            return SpecialOperatorDefinition.Create(startText, secondaryTokenType, precedence, SpecialOperatorType.MethodCall, argSeperator, endText, 0, int.MaxValue);
        }

        internal static OperatorDefinition CreateIndex(string startText, TokenType secondaryTokenType, TokenType argSeperator, TokenType endText, int precedence)
        {
            return SpecialOperatorDefinition.Create(startText, secondaryTokenType, precedence, SpecialOperatorType.Index, argSeperator, endText, 1, int.MaxValue);
        }

        public static OperatorDefinition CreateBinary(string text, int precedence, OperatorType operatorType)
        {
            return new StandardOperatorDefinition(text, precedence, operatorType, OperandCount.Binary);
        }

        internal static OperatorDefinition CreateUnary(string text, int precedence, OperatorType operatorType)
        {
            return new StandardOperatorDefinition(text, precedence, operatorType, OperandCount.Unary);
        }

    }
}
