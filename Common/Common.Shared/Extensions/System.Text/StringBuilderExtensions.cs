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
        public static Expression Append(this ExpressionExtensionData<StringBuilder> stringBuilder, object? expr)
        {
            return ExpressionEx.StringBuilder.Append(stringBuilder, Expression.Constant(expr));
        }
        public static Expression AppendLine(this ExpressionExtensionData<StringBuilder> stringBuilder, Expression expr)
        {
            return ExpressionEx.StringBuilder.AppendLine(stringBuilder, ExpressionEx.Object.ToStringEx(expr));
        }
        public static Expression AppendLine(this ExpressionExtensionData<StringBuilder> stringBuilder, object? expr)
        {
            return ExpressionEx.StringBuilder.AppendLine(stringBuilder, Expression.Constant(expr));
        }
    }
}
