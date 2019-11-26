using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser
{
#pragma warning disable CA1052 // Static holder types should be Static or NotInheritable
    public class Void
#pragma warning restore CA1052 // Static holder types should be Static or NotInheritable
    {
        private static Lazy<Void> _Instance = new Lazy<Void>(() => new Void());
        internal static Void Instance => _Instance.Value;

        private Void()
        {

        }
    }
}
