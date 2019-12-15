using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser
{
    [AttributeUsage(AttributeTargets.Method)]
    internal class CallableAttribute : Attribute
    {
    }
}
