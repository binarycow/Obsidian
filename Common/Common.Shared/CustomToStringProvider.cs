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
    public abstract class CustomToStringProvider
    {
        protected CustomToStringProvider()
        {
        }


        private Dictionary<Type, Func<object, string>> _Dictionary = new Dictionary<Type, Func<object, string>>();


        public virtual string FormatTuple(object? tuple, PropertyInfo[] tupleProperties)
        {
            return tuple?.ToString() ?? DefaultValue;
        }
        public virtual string FormatIEnumerable(IEnumerable<object?> enumerable)
        {
            return enumerable?.ToString() ?? DefaultValue;
        }


        public string DefaultValue { get; } = "";

        public void Register<T>(Func<T, string> toStringFunction)
            where T : class
        {
            _Dictionary.Upsert(typeof(T), obj => {
                var typed = (T)obj;
                return typed != null ? toStringFunction(typed) : DefaultValue;
            });
        }

        public string ToString(object? item)
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
            var type = item?.GetType() ?? typeof(object);
            // Check if its "IEnumerable" or "IEnumerable<T>"

            if (item is IEnumerable enumerable)
            {
                return FormatIEnumerable(enumerable.OfType<object?>());
            }
            if (Reflection.TryGetIEnumerable(item, out var ienumerableT))
            {
                return FormatIEnumerable(ienumerableT);
            }
            // Check for Tuple
            if(Reflection.IsTuple(item, out var tupleProperties))
            {
                return FormatTuple(item, tupleProperties);
            }
            return item?.ToString() ?? DefaultValue;
        }
    }
}
