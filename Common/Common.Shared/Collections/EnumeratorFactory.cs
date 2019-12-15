using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Common.Collections
{
    internal static class EnumeratorFactory
    {
        internal static IEnumerator<object?> GetEnumerator(object? obj)
        {
            return obj switch
            {
                IEnumerable<object?> objEnumerable => objEnumerable.GetEnumerator(),
                IEnumerable enumerable => enumerable.OfType<object>().GetEnumerator(),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
