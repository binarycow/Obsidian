using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser.Operators
{
    public enum SpecialOperatorSubType
    {
        Unknown = 0,
        Property,
        MethodCall,
        Index,
    }
}
