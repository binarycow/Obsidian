using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Common
{
    internal static class TypeCoercion
    {

        static readonly Dictionary<Type, List<Type>> ImplicitNumericConversions = new Dictionary<Type, List<Type>>();

        static TypeCoercion()
        {
            ImplicitNumericConversions.Add(typeof(sbyte), new List<Type> { typeof(short), typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal) });
            ImplicitNumericConversions.Add(typeof(byte), new List<Type> { typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) });
            ImplicitNumericConversions.Add(typeof(short), new List<Type> { typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal) });
            ImplicitNumericConversions.Add(typeof(ushort), new List<Type> { typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) });
            ImplicitNumericConversions.Add(typeof(int), new List<Type> { typeof(long), typeof(float), typeof(double), typeof(decimal) });
            ImplicitNumericConversions.Add(typeof(uint), new List<Type> { typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) });
            ImplicitNumericConversions.Add(typeof(long), new List<Type> { typeof(float), typeof(double), typeof(decimal) });
            ImplicitNumericConversions.Add(typeof(char), new List<Type> { typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) });
            ImplicitNumericConversions.Add(typeof(float), new List<Type> { typeof(double) });
            ImplicitNumericConversions.Add(typeof(ulong), new List<Type> { typeof(float), typeof(double), typeof(decimal) });
        }

        static bool HasImplicitConversion(Type definedOn, Type baseType, Type targetType)
        {
            return definedOn.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(mi => mi.Name == "op_Implicit" && mi.ReturnType == targetType)
                .Any(mi =>
                {
                    ParameterInfo pi = mi.GetParameters().FirstOrDefault();
                    return pi != null && pi.ParameterType == baseType;
                });

        }

        internal static bool CanCast(Type? from, Type to)
        {
            if (from == null)
            {
                throw new NotImplementedException();
            }

            if (from.IsAssignableFrom(to))
            {
                return true;
            }
            if (HasImplicitConversion(from, from, to) || HasImplicitConversion(to, from, to))
            {
                return true;
            }
            if (ImplicitNumericConversions.TryGetValue(from, out List<Type> list))
            {
                if (list.Contains(to))
                    return true;
            }

            if (to.IsEnum)
            {
                return CanCast(from, Enum.GetUnderlyingType(to));
            }

            var toType = Nullable.GetUnderlyingType(to);
            if (toType != null)
            {
                return CanCast(from, toType);
            }

            return false;
        }


    }

}
