using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser.Configuration
{
    public class SpecialOperatorDefinition : OperatorDefinition
    {
        public SpecialOperatorDefinition(string text, int precedence, SpecialOperatorType operatorType, string? endingText = null) : base(text, precedence, OperandCount.Binary)
        {
            OperatorType = operatorType;
            EndingText = endingText;
        }

        public string? EndingText { get; }

        public SpecialOperatorType OperatorType { get; }
    }
}
