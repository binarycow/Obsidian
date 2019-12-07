using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser.Configuration
{
    public enum SpecialOperatorType
    {
        Unknown = 0,
        PropertyAccess,
        MethodCall,
        Index,
        Pipeline,
    }
}
