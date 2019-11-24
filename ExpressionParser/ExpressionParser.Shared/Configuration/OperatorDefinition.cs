using System;
using System.Collections.Generic;
using System.Text;

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

        public string Text { get; }
        public int Precedence { get; }
        public OperandCount OperandCount { get; }

        internal static OperatorDefinition CreateMemberAccess(string text, int precedence)
        {
            return new SpecialOperatorDefinition(text, precedence, SpecialOperatorType.MemberAccess);
        }

        public static OperatorDefinition CreateBinary(string text, int precedence, OperatorType operatorType)
        {
            return new StandardOperatorDefinition(text, precedence, operatorType, OperandCount.Binary);
        }
    }
}
