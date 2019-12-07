using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Common.Collections;
using Common.LookaroundEnumerator;

namespace Common.Collections
{
    internal interface ILookaroundEnumerator<T> : IEnumerator<T>
    {
        bool TryGetNextArray(int count, out T[] value);
        bool TryGetNext([NotNullWhen(true)]out T value, int count = 1);
        bool TryGetPrevious([NotNullWhen(true)]out T value, int count = 1);
        //LookaroundData<T> Data { get; }
        int LookaheadCount { get; }
        EnumeratorState State { get; }
        T MoveNextAndGetValue(out bool moveNextReturn);
        public T[] Read(int count);
        public bool MoveNext(int count);
    }
}
