using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Common
{
    public class CustomToStringProvider
    {
        private CustomToStringProvider()
        {
            Register<ReadOnlyCollection<string>>(coll => $"({string.Join(", ", coll.Select(item => $"'{item}'"))})");
            Register<Dictionary<string,object?>>(coll => $"{{{string.Join(", ", coll.Select(item => $"'{item.Key}' = {item.Value}"))}}}");
            Register<string[]>(coll => $"({string.Join(", ", coll.Select(item => $"'{item}'"))})");
        }

        private static Lazy<CustomToStringProvider> _Instance = new Lazy<CustomToStringProvider>(() => new CustomToStringProvider());
        public static CustomToStringProvider Instance => _Instance.Value;


        private Dictionary<Type, Func<object, string>> _Dictionary = new Dictionary<Type, Func<object, string>>();

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
            if(item == null)
            {
                return DefaultValue;
            }
            if(_Dictionary.TryGetValue(item.GetType(), out var func) == false)
            {
                return item.ToString();
            }
            return func(item);
        }
    }
}
