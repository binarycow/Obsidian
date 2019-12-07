using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Common.Collections;

namespace Common.LookaroundEnumerator
{
    public sealed class ArrayLookaroundEnumerator<T> : ILookaroundEnumerator<T>
    {
        public ArrayLookaroundEnumerator(T[] array, byte lookaheadCount, byte lookbehindCount = 0)
        {
            Current = default!;
            _Array = array;
            LookaheadCount = lookaheadCount;
            LookbehindCount = lookbehindCount;
        }

        private T[] _Array;
        private int _CurrentIndex = -1;

        public T Current { get; private set; }

        public int LookaheadCount { get; }

        public int LookbehindCount { get; }

        public EnumeratorState State => throw new NotImplementedException();

        object? IEnumerator.Current => Current;

        public void Dispose()
        {

        }

        public bool MoveNext()
        {

            if (_CurrentIndex < _Array.Length - 1)
            {
                ++_CurrentIndex;
                Current = _Array[_CurrentIndex];
                throw new NotImplementedException(); // CreateData();
                return true;
            }
            else if (_CurrentIndex < _Array.Length)
            {
                ++_CurrentIndex;
                Current = default!;
                return false;
            }
            else
            {
                return false;
            }
        }

        public void Reset()
        {
            _CurrentIndex = -1;
        }

        public bool TryGetNext([NotNullWhen(true)] out T value, int count = 1)
        {
            if (_CurrentIndex + count < _Array.Length)
            {
                value = _Array[_CurrentIndex + count];
                return true;
            }
            value = default!;
            return false;
        }

        public bool TryGetPrevious([NotNullWhen(true)] out T value, int count = 1)
        {
            if (_CurrentIndex - count >= 0)
            {
                value = _Array[_CurrentIndex - count];
                return true;
            }
            value = default!;
            return false;
        }

        public T MoveNextAndGetValue(out bool moveNextReturn)
        {
            moveNextReturn = MoveNext();
            return Current;
        }

        public T[] Read(int count)
        {
            count = Math.Min(count, _Array.Length - _CurrentIndex);
            var returnArray = new T[count];
            Array.Copy(_Array, _CurrentIndex, returnArray, 0, count);
            return returnArray;
        }
        public bool MoveNext(int count)
        {
            for (var i = 0; i < count; ++i)
            {
                if (MoveNext() == false)
                {
                    return false;
                }
            }
            return true;
        }

        public bool TryGetNextArray(int count, out T[] value)
        {
            throw new NotImplementedException();
        }
    }

}
