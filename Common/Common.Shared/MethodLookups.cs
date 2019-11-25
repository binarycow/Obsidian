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


        private static Dictionary<Tuple<Type, string, Type[]>, MethodInfo> Methods = new Dictionary<Tuple<Type, string, Type[]>, MethodInfo>();
        private static HashSet<Tuple<Type, string, Type[]>> NoMethods = new HashSet<Tuple<Type, string, Type[]>>();


        internal static MethodInfo GetMethod(Type type, string methodName, Type[] methodArguments)
        {
            TryGetMethod(type, methodName, methodArguments, out var methodInfo);
            return methodInfo ?? throw new NotImplementedException();
        }

        internal static bool TryGetMethod(Type type, string methodName, Type[] methodArguments, [NotNullWhen(true)]out MethodInfo? methodInfo)
        {
            var key = Tuple.Create(type, methodName, methodArguments);
            if(Methods.TryGetValue(key, out methodInfo))
            {
                return true;
            }
            if(NoMethods.Contains(key))
            {
                return false;
            }
            methodInfo = type.GetMethod(methodName, methodArguments);
            if(methodInfo == default)
            {
                NoMethods.Add(key);
                return false;
            }
            Methods.Add(key, methodInfo);
            return true;
        }
    }
}
