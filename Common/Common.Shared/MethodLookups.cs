using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;

namespace Common
{
    public static class MethodLookups
    {



        private static Type _Enumerable = typeof(System.Linq.Enumerable);
        private static Lazy<Dictionary<Type, MethodInfo>> _EnumerableToArray = new Lazy<Dictionary<Type, MethodInfo>>();

        public static bool TryGet_Enumerable_ToArray(Type type, [NotNullWhen(true)]out MethodInfo? methodInfo)
        {
            if(_EnumerableToArray.Value.TryGetValue(type, out methodInfo))
            {
                return true;
            }

            var nonGenericMethodInfo = _Enumerable.GetMethod("ToArray");
            var genericMethodInfo = nonGenericMethodInfo?.MakeGenericMethod(type);
            if (genericMethodInfo != null)
            {
                _EnumerableToArray.Value.Add(type, genericMethodInfo);
                methodInfo = genericMethodInfo;
                return true;
            }
            methodInfo = default;
            return false;
        }



        private static Dictionary<Tuple<Type, Type[], Type[]>, ConstructorInfo> Constructors = new Dictionary<Tuple<Type, Type[], Type[]>, ConstructorInfo>();
        private static HashSet<Tuple<Type, Type[], Type[]>> NoConstructors = new HashSet<Tuple<Type, Type[], Type[]>>();

        public static bool TryGet_Constructor([NotNullWhen(true)]out ConstructorInfo? constructorInfo,
            Type type, Type[] constructorArguments)
        {
            return TryGet_Constructor_GenericType(out constructorInfo, type, Type.EmptyTypes, constructorArguments);
        }

        public static bool TryGet_Constructor_GenericType([NotNullWhen(true)]out ConstructorInfo? constructorInfo, 
            Type openGenericType, Type[] typeTypeArguments, Type[] constructorArguments)
        {
            var key = Tuple.Create(openGenericType, typeTypeArguments, constructorArguments);
            if(Constructors.TryGetValue(key, out constructorInfo))
            {
                return true;
            }
            if(NoConstructors.Contains(key) == true)
            {
                return false;
            }
            var typeToCreate = openGenericType;
            if(typeTypeArguments.Length != 0)
            {
                typeToCreate = typeToCreate.MakeGenericType(typeTypeArguments);
            }
            constructorInfo = typeToCreate.GetConstructor(constructorArguments);
            return constructorInfo != default;
        }
    }
}
