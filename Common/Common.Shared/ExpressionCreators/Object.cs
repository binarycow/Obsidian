using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Common.ExpressionCreators
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "<Pending>")]

    internal class Object
    {
#pragma warning disable CA1822 // Mark members as static
        public Expression ToStringEx(Expression obj)
#pragma warning restore CA1822 // Mark members as static
        {
            return Expression.Call(obj, "ToString", Type.EmptyTypes);
        }
    }
}
