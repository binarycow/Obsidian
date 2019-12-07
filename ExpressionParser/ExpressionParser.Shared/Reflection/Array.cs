using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ExpressionParser.Reflection
{
    internal static class Array
    {
        private static readonly Lazy<Dictionary<Type, PropertyInfo>> _Length = new Lazy<Dictionary<Type, PropertyInfo>>();
        private static Dictionary<Type, PropertyInfo> Length => _Length.Value;



        public static bool TryLength(Expression array, [NotNullWhen(true)]out Expression? arrayLengthProperty)
        {
            arrayLengthProperty = default;
            if (array.Type.IsArray == false) return false;

            if(Length.TryGetValue(array.Type, out var propertyInfo) == false)
            {
                propertyInfo = array.Type.GetProperty("Length");
                if (propertyInfo == null) return false;
            }
            arrayLengthProperty = Expression.MakeMemberAccess(array, propertyInfo);
            return true;
        }
    }
}
