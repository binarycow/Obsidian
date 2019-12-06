using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace System
{
    public static class ObjectExtensions
    {
        public static IEnumerable<T> YieldOne<T>(this T obj)
        {
            yield return obj;
        }

        public static bool TryConvert<T>(this object obj, [NotNullWhen(true)]out T converted)
        {
            converted = default!;
            if(obj is T convertedLocal)
            {
                converted = convertedLocal;
                return true;
            }
            return false;
        }

        public static bool TryToInt32(this object? obj, out int integerValue)
        {
            integerValue = default;
            if(obj == null)
            {
                return false;
            }
            if (obj is int iVal)
            {
                integerValue = iVal;
                return true;
            }
            return int.TryParse(obj.ToString(), out integerValue);
        }
    }
}
