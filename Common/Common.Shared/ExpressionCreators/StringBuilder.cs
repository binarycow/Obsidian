using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Common.ExpressionCreators
{
#pragma warning disable CA1812 // Avoid uninstantiated internal classes
    internal class StringBuilder
#pragma warning restore CA1812 // Avoid uninstantiated internal classes
    {
        private static readonly Type _StringBuilderType = typeof(System.Text.StringBuilder);
        private static readonly Lazy<Dictionary<Type, MethodInfo>> _AppendMethods = new Lazy<Dictionary<Type, MethodInfo>>();
        private static Dictionary<Type, MethodInfo> AppendMethods => _AppendMethods.Value;
        private static HashSet<Type> NoAppendMethods => _NoAppendMethods.Value;

        private static readonly Lazy<HashSet<Type>> _NoAppendMethods = new Lazy<HashSet<Type>>();


        private static readonly Lazy<Dictionary<Type, MethodInfo>> _AppendLineMethods = new Lazy<Dictionary<Type, MethodInfo>>();
        private static Dictionary<Type, MethodInfo> AppendLineMethods => _AppendLineMethods.Value;
        private static HashSet<Type> NoAppendLineMethods => _NoAppendLineMethods.Value;

        private static readonly Lazy<HashSet<Type>> _NoAppendLineMethods = new Lazy<HashSet<Type>>();

        private MethodInfo GetAppendMethod(Type type)
        {
            if (AppendMethods.TryGetValue(type, out var methodInfo))
            {
                return methodInfo;
            }
            if (NoAppendMethods.Contains(type) == false)
            {
                var foundMethod = _StringBuilderType.GetMethod("Append", new[] { type });
                if (foundMethod != null)
                {
                    AppendMethods.Add(type, foundMethod);
                    return foundMethod;
                }
                else
                {
                    NoAppendMethods.Add(type);
                }
            }
            return GetAppendMethod(typeof(object));
        }

        private MethodInfo GetAppendLineMethod(Type type)
        {
            if (AppendLineMethods.TryGetValue(type, out var methodInfo))
            {
                return methodInfo;
            }
            if (NoAppendLineMethods.Contains(type) == false)
            {
                var foundMethod = _StringBuilderType.GetMethod("AppendLine", new[] { type });
                if (foundMethod != null)
                {
                    AppendLineMethods.Add(type, foundMethod);
                    return foundMethod;
                }
                else
                {
                    NoAppendLineMethods.Add(type);
                }
            }
            if (type == typeof(object)) throw new NotImplementedException();
            return GetAppendLineMethod(typeof(object));
        }

        public Expression Append(Expression stringBuilder, Expression item)
        {
            return Expression.Call(stringBuilder, GetAppendMethod(item.Type), new[] { item });
        }
        public Expression AppendLine(Expression stringBuilder, Expression item)
        {
            return Expression.Call(stringBuilder, GetAppendLineMethod(item.Type), new[] { item });
        }
    }
}
