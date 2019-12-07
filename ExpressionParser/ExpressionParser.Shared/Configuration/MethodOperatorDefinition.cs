using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressionParser.Configuration
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "<Pending>")]
    internal class MethodOperatorDefinition : OperatorDefinition
    {
        public MethodOperatorDefinition(string startText, string endText, int precedence, SpecialOperatorType operatorType) : base(startText, precedence, OperandCount.Binary)
        {
            EndText = endText;
            OperatorType = operatorType;
        }

        public string EndText { get; }
        public SpecialOperatorType OperatorType { get; }
    }
}
