using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections;

namespace Common
{
    public static class CollectionEx
    {


        public static object?[]? ToArray(object? obj)
        {
            if (obj == null) return null;
            if (obj is Array arr) return (object?[])arr;
            if (obj is IEnumerable ienum) return ienum.OfType<object>().ToArray();
            throw new NotImplementedException();
        }
    }
}
