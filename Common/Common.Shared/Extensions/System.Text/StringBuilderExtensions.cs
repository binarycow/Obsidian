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

        public static void AppendCustom(this StringBuilder stringBuilder, object? value, CustomToStringProvider customProvider)
        {
            stringBuilder = stringBuilder ?? throw new ArgumentNullException(nameof(stringBuilder));
            customProvider = customProvider ?? throw new ArgumentNullException(nameof(customProvider));
            stringBuilder.Append(customProvider.ToString(value));
        }

        public static void AppendLineCustom(this StringBuilder stringBuilder, object? value, CustomToStringProvider customProvider)
        {
            stringBuilder = stringBuilder ?? throw new ArgumentNullException(nameof(stringBuilder));
            customProvider = customProvider ?? throw new ArgumentNullException(nameof(customProvider));
            stringBuilder.AppendLine(customProvider.ToString(value));
        }

        public static void AppendCustomRange(this StringBuilder stringBuilder, IEnumerable<object?> values, CustomToStringProvider customProvider)
        {
            foreach(var value in values)
            {
                stringBuilder.AppendCustom(value, customProvider);
            }
        }

        public static void AppendLineCustomRange(this StringBuilder stringBuilder, IEnumerable<object?> values, CustomToStringProvider customProvider)
        {
            foreach (var value in values)
            {
                stringBuilder.AppendLineCustom(value, customProvider);
            }
        }

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
