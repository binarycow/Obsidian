using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace System
{
    public static class TypeExtensions
    {
        public static bool TryGetEnumerableBaseType(this Type type, [NotNullWhen(true)]out Type? baseType)
        {
            if(type.IsArray)
            {
                baseType = type.GetElementType();
                return baseType != null;
            }
            throw new NotImplementedException();
        }

        public static bool IsAssignableToGenericType(this Type givenType, Type genericType, [NotNullWhen(true)]out Type[]? genericTypeArguments)
        {
            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            {
                genericTypeArguments = givenType.GetGenericArguments();
                return true;
            }

            var interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                {
                    genericTypeArguments = it.GetGenericArguments();
                    return true;
                }
            }

            Type baseType = givenType.BaseType;
            if (baseType == null)
            {
                genericTypeArguments = default;
                return false;
            }

            return IsAssignableToGenericType(baseType, genericType, out genericTypeArguments);
        }
    }
}
