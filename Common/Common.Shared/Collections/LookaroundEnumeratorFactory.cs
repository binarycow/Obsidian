using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.LookaroundEnumerator;

namespace Common.Collections
{
    public static class LookaroundEnumeratorFactory
    {

        public static ILookaroundEnumerator<T> CreateLookaroundEnumerator<T>(IEnumerable<T> source, byte lookaheadCount = 1, byte lookbehindCount = 0)
        {
            return new EnumerableLookaroundEnumerator<T>(source, lookaheadCount, lookbehindCount);
            // TODO: Fix ArrayLookaroundEnumerator and actually use that...

            //return (source is T[] array) ?
            //    (ILookaroundEnumerator<T>)new ArrayLookaroundEnumerator<T>(array, lookaheadCount, lookbehindCount) :
            //    (ILookaroundEnumerator<T>)new EnumerableLookaroundEnumerator<T>(source, lookaheadCount, lookbehindCount);

        }
    }
}
