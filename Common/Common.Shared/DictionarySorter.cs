using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public static class DictionarySorter
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

        public static IEnumerable<KeyValuePair<TKey, TValue>> SortDictionary<TKey, TValue>(IDictionary<TKey, TValue> dictionary, bool byKey, bool reverse, bool caseSensitive)
        {



        }
    }
}
