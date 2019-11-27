using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using ExpressionParser.Configuration;
using ExpressionParser.References;
using System.Linq;
using Common;
using ExpressionParser.Scopes;

namespace ExpressionParser
{
    public static class ExpressionResolver
    {


        public static bool TryGetPropertyInfo(Expression @object, string memberName, [NotNullWhen(true)]out PropertyInfo? propertyInfo)
        {
            var type = @object.Type;
            propertyInfo = type.GetProperty(memberName);
            return propertyInfo != null;
        }
        public static bool TryGetFieldInfo(Expression @object, string memberName, [NotNullWhen(true)]out FieldInfo? fieldInfo)
        {
            var type = @object.Type;
            fieldInfo = type.GetField(memberName);
            return fieldInfo != null;
        }




        public static bool TryGetPropertyOrFieldInfo(Expression @object, string memberName, [NotNullWhen(true)]out MemberInfo? memberInfo)
        {
            if (TryGetPropertyInfo(@object, memberName, out var propInfo))
            {
                memberInfo = propInfo;
                return true;
            }
            if (TryGetPropertyInfo(@object, memberName, out var fieldInfo))
            {
                memberInfo = fieldInfo;
                return true;
            }
            memberInfo = default;
            return false;
        }

        public static bool TryGetIndexer(Expression @object, [NotNullWhen(true)]out PropertyInfo? propertyInfo, params Type[] indexArguments)
        {
            var type = @object.Type;
            propertyInfo = type.GetProperty("Item", indexArguments);
            return propertyInfo != null;
        }

    }
}
