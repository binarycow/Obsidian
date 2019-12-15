using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    internal class StringBuilderPool : ObjectPool<StringBuilder>
    {
        internal static Lazy<StringBuilderPool> _Instance = new Lazy<StringBuilderPool>(() => new StringBuilderPool());
        internal static StringBuilderPool Instance => _Instance.Value;


        private StringBuilderPool() : base(() => new StringBuilder(), stringBuilder => stringBuilder.Clear())
        {

        }
    }
}
