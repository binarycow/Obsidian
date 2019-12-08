using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser.Configuration
{
    internal enum SpecialOperatorType
    {
        Unknown = 0,
        PropertyAccess,
        MethodCall,
        Index,
        Pipeline,
    }
}
