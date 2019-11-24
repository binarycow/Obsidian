using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Common.ExpressionCreators
{
    public class Enumerable
    {
        private Type _EnumerableType = typeof(System.Linq.Enumerable);

        private Lazy<Dictionary<Type, MethodInfo>> _ToArray_Methods = new Lazy<Dictionary<Type, MethodInfo>>();
        private Dictionary<Type, MethodInfo> ToArray => _ToArray_Methods.Value;


        private MethodInfo GetToArrayMethod(Type type)
        {
            if (ToArray.TryGetValue(type, out var methodInfo))
            {
                return methodInfo;
            }
            var foundMethod = _EnumerableType.GetMethod("ToArray", new[] { type });
            var genericMethod = foundMethod?.MakeGenericMethod(type);
            if (genericMethod != null)
            {
                ToArray.Add(type, genericMethod);
                return genericMethod;
            }
            return default;
        }



    }
}
