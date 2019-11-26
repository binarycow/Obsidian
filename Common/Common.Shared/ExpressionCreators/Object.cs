using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Common.ExpressionCreators
{
    public class Object
    {
        public Expression ToStringEx(Expression obj)
        {
            return Expression.Call(obj, "ToString", Type.EmptyTypes);
        }
    }
}
