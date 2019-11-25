using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text;
using ExpressionParser.Parsing;

namespace ExpressionParser.References
{
    public class ExpressionMethodGroup : MethodGroup
    {
        public ExpressionMethodGroup(Expression referredObject, string methodName) : base(methodName)
        {
            ReferredObject = referredObject;
        }

        public Expression ReferredObject { get; }

        
    }
}
