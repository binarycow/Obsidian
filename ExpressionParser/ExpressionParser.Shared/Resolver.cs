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

namespace ExpressionParser
{
    public static class Resolver
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

        internal static object? CallMethod(object? left, object?[] args)
        {
            switch(left)
            {
                case FunctionMethodGroup methodGroup:
                    return FuncMethodGroup(methodGroup, args);
                default:
                    throw new NotImplementedException();
            }

            object? FuncMethodGroup(FunctionMethodGroup left, object?[] args)
            {
                if (left.FunctionDefinition.OverloadDefinitions.Length != 1) throw new NotImplementedException();
                var overload = left.FunctionDefinition.OverloadDefinitions[0];
                switch(overload)
                {
                    case SingleTypeOverloadDefinition singleOverload:
                        if (args.Length < singleOverload.MinimumArguments) throw new NotImplementedException();
                        if (args.Length > singleOverload.MaximumArguments) throw new NotImplementedException();

                        var invalidTypes = args.Any(arg => TypeCoercion.CanCast(arg?.GetType() ?? typeof(object), singleOverload.ArgumentType));
                        if (invalidTypes) throw new NotImplementedException();

                        var result = singleOverload.Function(args);
                        if ((result?.GetType() ?? typeof(object)) != singleOverload.ReturnType) throw new NotImplementedException();
                        return result;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}
