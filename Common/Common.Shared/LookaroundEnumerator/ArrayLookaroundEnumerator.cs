//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Diagnostics.CodeAnalysis;
//using System.Linq;
//using System.Text;
//using Common.Collections;

//namespace Common.LookaroundEnumerator
//{
//    internal sealed class ArrayLookaroundEnumerator<T> : ILookaroundEnumerator<T>
//    {
//        internal ArrayLookaroundEnumerator(T[] array, byte lookaheadCount, byte lookbehindCount = 0)
//        {
//            Current = default!;
//            _Array = array;
//            LookaheadCount = lookaheadCount;
//            LookbehindCount = lookbehindCount;
//        }

//        private T[] _Array;
//        private int _CurrentIndex = -1;

//        internal T Current { get; private set; }

//        internal int LookaheadCount { get; }

//        internal int LookbehindCount { get; }

//        internal EnumeratorState State => throw new NotImplementedException();

//        object? IEnumerator.Current => Current;

//        internal void Dispose()
//        {

//        }

//        internal bool MoveNext()
//        {

//            if (_CurrentIndex < _Array.Length - 1)
//            {
//                ++_CurrentIndex;
//                Current = _Array[_CurrentIndex];
//                throw new NotImplementedException(); // CreateData();
//            }
//            else if (_CurrentIndex < _Array.Length)
//            {
//                ++_CurrentIndex;
//                Current = default!;
//                return false;
//            }
//            else
//            {
//                return false;
//            }
//        }

//        internal void Reset()
//        {
//            _CurrentIndex = -1;
//        }

//        internal bool TryGetNext([NotNullWhen(true)] out T value, int count = 1)
//        {
//            if (_CurrentIndex + count < _Array.Length)
//            {
//                value = _Array[_CurrentIndex + count];
//                return true;
//            }
//            value = default!;
//            return false;
//        }

//        internal bool TryGetPrevious([NotNullWhen(true)] out T value, int count = 1)
//        {
//            if (_CurrentIndex - count >= 0)
//            {
//                value = _Array[_CurrentIndex - count];
//                return true;
//            }
//            value = default!;
//            return false;
//        }

//        internal T MoveNextAndGetValue(out bool moveNextReturn)
//        {
//            moveNextReturn = MoveNext();
//            return Current;
//        }

//        internal T[] Read(int count)
//        {
//            count = Math.Min(count, _Array.Length - _CurrentIndex);
//            var returnArray = new T[count];
//            Array.Copy(_Array, _CurrentIndex, returnArray, 0, count);
//            return returnArray;
//        }
//        internal bool MoveNext(int count)
//        {
//            for (var i = 0; i < count; ++i)
//            {
//                if (MoveNext() == false)
//                {
//                    return false;
//                }
//            }
//            return true;
//        }

//        internal bool TryGetNextArray(int count, out T[] value)
//        {
//            throw new NotImplementedException();
//        }
//    }

//}
