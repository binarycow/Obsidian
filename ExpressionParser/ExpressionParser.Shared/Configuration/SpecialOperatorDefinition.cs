using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser.Configuration
{
    public class SpecialOperatorDefinition : OperatorDefinition
    {
        public SpecialOperatorDefinition(string text, int precedence, SpecialOperatorType operatorType) : base(text, precedence, OperandCount.Binary)
        {
            OperatorType = operatorType;
        }

        public SpecialOperatorType OperatorType { get; }
    }
}
