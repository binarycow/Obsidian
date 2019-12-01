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

        public static T? FirstOrDefaultValueType<T>(this IEnumerable<T> source, Func<T, bool> predicate)
            where T : struct
        {
            source = source ?? throw new ArgumentNullException(nameof(source));
            predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            foreach (var item in source)
            {
                if(predicate(item))
                {
                    return item;
                }
            }
            return null;
        }
    }
}
