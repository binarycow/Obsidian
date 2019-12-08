using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text;
using ExpressionParser.Parsing;

namespace ExpressionParser.References
{
    internal class ExpressionMethodGroup : MethodGroup
    {
        internal ExpressionMethodGroup(Expression referredObject, string methodName) : base(methodName)
        {
            ReferredObject = referredObject;
        }

        internal Expression ReferredObject { get; }

        
    }
}
