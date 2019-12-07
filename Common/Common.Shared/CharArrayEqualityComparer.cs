using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    internal class CharArrayEqualityComparer : IEqualityComparer<char[]>
    {
        private CharArrayEqualityComparer()
        {

        }
        public static readonly Lazy<CharArrayEqualityComparer> _Instance = new Lazy<CharArrayEqualityComparer>(() => new CharArrayEqualityComparer());
        public static CharArrayEqualityComparer Instance => _Instance.Value;

        public bool Equals(char[] x, char[] y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            if (x.Length != y.Length) return false;
            for (var i = 0; i < x.Length; ++i)
            {
                if (x[i] != x[i]) return false;
            }
            return true;
        }

        public int GetHashCode(char[] obj)
        {
            return new string(obj).GetHashCode();
        }
    }
}
