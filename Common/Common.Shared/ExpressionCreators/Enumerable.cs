//using System;
//using System.Collections.Generic;
//using System.Diagnostics.CodeAnalysis;
//using System.Linq;
//using System.Reflection;
//using System.Text;

//namespace Common.ExpressionCreators
//{
//#pragma warning disable CA1812 // Avoid uninstantiated internal classes
//    internal class Enumerable
//#pragma warning restore CA1812 // Avoid uninstantiated internal classes
//    {
//        private readonly Type _EnumerableType = typeof(System.Linq.Enumerable);

//        private readonly Lazy<Dictionary<Type, MethodInfo>> _ToArray_Methods = new Lazy<Dictionary<Type, MethodInfo>>();
//        private Dictionary<Type, MethodInfo> ToArray => _ToArray_Methods.Value;


//        private MethodInfo? GetToArrayMethod(Type type)
//        {
//            if (ToArray.TryGetValue(type, out var methodInfo))
//            {
//                return methodInfo;
//            }
//            var foundMethod = _EnumerableType.GetMethod("ToArray", new[] { type });
//            var genericMethod = foundMethod?.MakeGenericMethod(type);
//            if (genericMethod != null)
//            {
//                ToArray.Add(type, genericMethod);
//                return genericMethod;
//            }
//            return default;
//        }



//    }
//}
