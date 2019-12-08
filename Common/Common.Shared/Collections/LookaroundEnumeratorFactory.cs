using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.LookaroundEnumerator;

namespace Common.Collections
{
    internal static class LookaroundEnumeratorFactory
    {

        internal static ILookaroundEnumerator<T> CreateLookaroundEnumerator<T>(IEnumerable<T> source, byte lookaheadCount = 1, byte lookbehindCount = 0)
        {
            // TODO: Fix ArrayLookaroundEnumerator and use that
            return new EnumerableLookaroundEnumerator<T>(source, lookaheadCount, lookbehindCount);
            //return (source is T[] array) ?
            //    (ILookaroundEnumerator<T>)new ArrayLookaroundEnumerator<T>(array, lookaheadCount, lookbehindCount) :
            //    (ILookaroundEnumerator<T>)new EnumerableLookaroundEnumerator<T>(source, lookaheadCount, lookbehindCount);
        }
    }
}
