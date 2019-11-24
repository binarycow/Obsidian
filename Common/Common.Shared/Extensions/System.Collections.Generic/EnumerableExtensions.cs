using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Collections.Generic
{
    public static class EnumerableExtensions
    {
        public static T[] ToArrayWithoutInstantiation<T>(this IEnumerable<T> source)
        {
            return (source is T[] arr) ? arr : source.ToArray();
        }
        public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, T item)
        {
            return source.Concat(Enumerable.Repeat(item, 1));
        }
    }
}
