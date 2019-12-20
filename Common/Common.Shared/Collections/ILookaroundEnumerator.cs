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
        public bool TryGetNextArray(int count, out T[] value);
        public bool TryGetNext([NotNullWhen(true)]out T value, int count = 1);
        public bool TryGetPrevious([NotNullWhen(true)]out T value, int count = 1);
        //LookaroundData<T> Data { get; }
        public int LookaheadCount { get; }
        public EnumeratorState State { get; }
        public T MoveNextAndGetValue(out bool moveNextReturn);
        public T[] Read(int count);
        public bool MoveNext(int count);
        public int StartBacktrackSession();
        public bool ResetBacktrackSession(int id);
        public bool CommitBacktrackSession(int id);
    }
}
