using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Common
{
    internal static class DictionarySorter
    {

        internal class CaseInsensitiveComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                return string.Compare(x, y, StringComparison.OrdinalIgnoreCase);
            }
        }


        public static IEnumerable<KeyValuePair<TKey, TValue>> SortDictionaryByKeys<TKey, TValue>(IDictionary<TKey, TValue> dictionary, bool reverse)
            where TKey : IComparable<TKey>
        {
            if (reverse)
            {
                return dictionary.OrderByDescending(kvp => kvp.Key);
            }
            else
            {
                return dictionary.OrderBy(kvp => kvp.Key);
            }
        }
        public static IEnumerable<KeyValuePair<TKey, TValue>> SortDictionaryByValues<TKey, TValue>(IDictionary<TKey, TValue> dictionary, bool reverse)
            where TValue : IComparable<TValue>
        {
            if (reverse)
            {
                return dictionary.OrderByDescending(kvp => kvp.Value);
            }
            else
            {
                return dictionary.OrderBy(kvp => kvp.Value);
            }
        }
        public static IEnumerable<KeyValuePair<TKey, string>> SortDictionaryByStringValues<TKey>(IDictionary<TKey, string> dictionary, bool reverse, bool caseSensitive)
        {
            if (reverse && caseSensitive)
            {
                return dictionary.OrderByDescending(kvp => kvp.Value);
            }
            else if (reverse)
            {
                return dictionary.OrderByDescending(kvp => kvp.Value, new CaseInsensitiveComparer());
            }
            else if (caseSensitive)
            {
                return dictionary.OrderBy(kvp => kvp.Value);
            }
            else
            {
                return dictionary.OrderBy(kvp => kvp.Value, new CaseInsensitiveComparer());
            }
        }
        public static IEnumerable<KeyValuePair<string, TValue>> SortDictionaryByStringKeys<TValue>(IDictionary<string, TValue> dictionary, bool reverse, bool caseSensitive)
        {
            if (reverse && caseSensitive)
            {
                return dictionary.OrderByDescending(kvp => kvp.Key);
            }
            else if (reverse)
            {
                return dictionary.OrderByDescending(kvp => kvp.Key, new CaseInsensitiveComparer());
            }
            else if (caseSensitive)
            {
                return dictionary.OrderBy(kvp => kvp.Key);
            }
            else
            {
                return dictionary.OrderBy(kvp => kvp.Key, new CaseInsensitiveComparer());
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
#pragma warning disable CA1801 // Review unused parameters
        private static MethodInfo CreateMethod(string methodName, Type[] genericTypes, int boolCount)
#pragma warning restore CA1801 // Review unused parameters
        {
            //var args = typeof(Dictionary<,>).YieldOne().Concat(Enumerable.Repeat(typeof(bool), boolCount)).ToArray();
            var method = typeof(DictionarySorter).GetMethod(methodName);
            return method.MakeGenericMethod(genericTypes);
        }

        public static IEnumerable<KeyValuePair<TKey, TValue>> SortDictionary<TKey, TValue>(IDictionary<TKey, TValue> dictionary, bool byKey, bool reverse, bool caseSensitive)
        {
            var isStringType = false;
            MethodInfo method;
            if(byKey)
            {
                if(typeof(TKey).IsComparable() == false)
                {
                    return dictionary;
                }
                if(typeof(TKey) == typeof(string))
                {
                    method = CreateMethod(nameof(SortDictionaryByStringKeys), new[] { typeof(TValue) }, boolCount: 2);
                    isStringType = true;
                }
                else
                {
                    method = CreateMethod(nameof(SortDictionaryByKeys), new[] { typeof(TKey), typeof(TValue) }, boolCount: 1);
                }
            }
            else
            {
                if(typeof(TValue).IsComparable() == false)
                {
                    return dictionary;
                }
                if (typeof(TValue) == typeof(string))
                {
                    method = CreateMethod(nameof(SortDictionaryByStringValues), new[] { typeof(TKey) }, boolCount: 2);
                    isStringType = true;
                }
                else
                {
                    method = CreateMethod(nameof(SortDictionaryByValues), new[] { typeof(TKey), typeof(TValue) }, boolCount: 1);
                }
            }
            if(method == null)
            {
                return dictionary;
            }
            var args = isStringType ? new object[] { dictionary, reverse, caseSensitive } : new object[] { dictionary, reverse };
            var results = method.Invoke(null, args);
            return (IEnumerable<KeyValuePair<TKey, TValue>>)
                Convert.ChangeType(results, typeof(IEnumerable<KeyValuePair<TKey, TValue>>), CultureInfo.InvariantCulture);
        }

        public static object? SortDictionaryObj(object? dictionary, bool byKey, bool reverse, bool caseSensitive)
        {
            if (dictionary == null) return null;
            if (dictionary.GetType().IsAssignableToGenericType(typeof(Dictionary<,>), out var genericTypeArguments) == false) return dictionary;

            var method = typeof(DictionarySorter).GetMethod(nameof(SortDictionary));
            var genericMethod = method.MakeGenericMethod(genericTypeArguments);
            return genericMethod.Invoke(null, new object[] { dictionary, byKey, reverse, caseSensitive});
        }
    }
}
