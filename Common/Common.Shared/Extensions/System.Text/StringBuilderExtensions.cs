using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Common;
using Common.ExpressionCreators;

namespace System.Text
{
    public static class StringBuilderExtensions
    {
        public static Expression Append(this ExpressionExtensionData<StringBuilder> stringBuilder, Expression expr)
        {
            return ExpressionEx.StringBuilder.Append(stringBuilder, expr);
        }
    }
}
