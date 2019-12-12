using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections;

namespace Common
{
    internal static class CollectionEx
    {


        internal static object?[]? ToArray(object? arrayObject)
        {
            if (arrayObject == null) return null;
            //if (arrayObject is Array arr) return (object?[])arr;
            if (arrayObject is IEnumerable ienum) return ienum.OfType<object>().ToArray();
            throw new NotImplementedException();
        }
    }
}
