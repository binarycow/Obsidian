using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Common.Collections
{
    public static class EnumeratorFactory
    {
        internal static IEnumerator<object?> GetEnumerator(object? obj)
        {
            switch(obj)
            {
                case IEnumerable<object?> objEnumerable:
                    return objEnumerable.GetEnumerator();
                case IEnumerable enumerable:
                    return enumerable.OfType<object>().GetEnumerator();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
