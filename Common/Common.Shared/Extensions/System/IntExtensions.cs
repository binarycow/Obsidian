using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    internal static class IntExtensions
    {
        internal static int ClampMax(this int number, int maximum) => number > maximum ? maximum : number;
    }
}
