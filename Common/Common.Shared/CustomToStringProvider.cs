using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Common
{
    internal abstract class CustomToStringProvider
    {
        protected CustomToStringProvider()
        {
        }


        private readonly Dictionary<Type, Func<object, string>> _Dictionary = new Dictionary<Type, Func<object, string>>();


        internal virtual string FormatTuple(object? tuple, PropertyInfo[] tupleProperties)
        {
            return tuple?.ToString() ?? DefaultValue;
        }
        internal virtual string FormatIEnumerable(IEnumerable<object?> enumerable)
        {
            return enumerable?.ToString() ?? DefaultValue;
        }


        internal string DefaultValue { get; } = "";

        internal void Register<T>(Func<T, string> toStringFunction)
            where T : class
        {
            _Dictionary.Upsert(typeof(T), obj => {
                var typed = (T)obj;
                return typed != null ? toStringFunction(typed) : DefaultValue;
            });
        }

        internal string ToString(object? item)
        {
            if (item == null)
            {
                return DefaultValue;
            }
            if (_Dictionary.TryGetValue(item.GetType(), out var func))
            {
                return func(item);
            }
            if (item is string str)
            {
                return str;
            }
            // Check if its "IEnumerable" or "IEnumerable<T>"

            if (item is IEnumerable enumerable)
            {
                return FormatIEnumerable(enumerable.OfType<object?>());
            }
            if (ReflectionHelpers.TryGetIEnumerable(item, out var ienumerableT))
            {
                return FormatIEnumerable(ienumerableT);
            }
            // Check for Tuple
            if(ReflectionHelpers.IsTuple(item, out var tupleProperties))
            {
                return FormatTuple(item, tupleProperties);
            }
            return item?.ToString() ?? DefaultValue;
        }
    }
}
