using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Common.ExpressionCreators
{
    public class StringBuilder
    {
        private static Type _StringBuilderType = typeof(System.Text.StringBuilder);
        private static Lazy<Dictionary<Type, MethodInfo>> _AppendMethods = new Lazy<Dictionary<Type, MethodInfo>>();
        private static Dictionary<Type, MethodInfo> AppendMethods => _AppendMethods.Value;

        private static Lazy<HashSet<Type>> _NoMethods = new Lazy<HashSet<Type>>();
        private static HashSet<Type> NoMethods => _NoMethods.Value;

        private MethodInfo GetAppendMethod(Type type)
        {
            if (AppendMethods.TryGetValue(type, out var methodInfo))
            {
                return methodInfo;
            }
            if (NoMethods.Contains(type) == false)
            {
                var foundMethod = _StringBuilderType.GetMethod("Append", new[] { type });
                if (foundMethod != null)
                {
                    AppendMethods.Add(type, foundMethod);
                    return foundMethod;
                }
                else
                {
                    NoMethods.Add(type);
                }
            }
            return GetAppendMethod(typeof(object));
        }

        public Expression Append(Expression stringBuilder, Expression item)
        {
            return Expression.Call(stringBuilder, GetAppendMethod(item.Type), new[] { item });
        }
    }
}
