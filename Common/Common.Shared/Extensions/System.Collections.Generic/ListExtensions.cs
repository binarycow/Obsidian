using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Collections.Generic
{
    public static class ListExtensions
    {
        public static bool TryGetIndex<T>(this IList<T> list, T item, out int index)
        {
            index = list.IndexOf(item);
            return index != -1;
        }
    }
}
