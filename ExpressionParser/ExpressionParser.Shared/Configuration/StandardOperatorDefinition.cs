using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressionParser.Configuration
{
    internal class StandardOperatorDefinition : OperatorDefinition
    {
        internal StandardOperatorDefinition(string text, int precedence, OperatorType operatorType, OperandCount operandCount) : base(text, precedence, operandCount)
        {
            OperatorType = operatorType;
        }

        internal OperatorType OperatorType { get; }
    }
}
