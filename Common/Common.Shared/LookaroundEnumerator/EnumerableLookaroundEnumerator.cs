using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Common.Collections;

namespace Common.LookaroundEnumerator
{
    internal sealed class EnumerableLookaroundEnumerator<T> : ILookaroundEnumerator<T>
    {
        internal EnumerableLookaroundEnumerator(IEnumerable<T> source, byte lookaheadCount, byte lookbehindCount = 0)
        {
            source = source ?? throw new ArgumentNullException(nameof(source));
            _Enumerator = source.GetEnumerator();
            LookaheadCount = lookaheadCount;
            LookbehindCount = lookbehindCount;
            _Previous = new T[lookbehindCount];
            _Next = new T[lookaheadCount];
        }

        private readonly IEnumerator<T> _Enumerator;

        private readonly T[] _Previous;
        private readonly T[] _Next;

        //internal LookaroundData<T> Data { private get; private set; } = new LookaroundData<T>(Array.Empty<T>(), default!, Array.Empty<T>());

        public int LookaheadCount { get; private set; }
        public int LookbehindCount { get; private set; }
        private int _ValidPreviousValues;
        private int _ValidNextValues;

        public EnumeratorState State { get; private set; }
        private EnumeratorState _UnderlyingEnumeratorState;
        internal EnumeratorState UnderlyingEnumeratorState 
        {
            get => LookaheadCount > 0 ? _UnderlyingEnumeratorState : State;
            private set
            {
                if(LookaheadCount > 0)
                {
                    _UnderlyingEnumeratorState = value;
                }
            }
        }

        public T Current { get; private set; } = default!;

        object? IEnumerator.Current => Current;

        public bool TryGetNext([NotNullWhen(true)]out T value, int count = 1)
        {
            value = default!;
            if (State != EnumeratorState.Active) return false;
            if (LookaheadCount < count) return false;
            if (_ValidNextValues < count) return false;
            value = _Next[count - 1];
            return true;
        }
        public bool TryGetPrevious([NotNullWhen(true)]out T value, int count = 1)
        {
            value = default!;
            if (State == EnumeratorState.NotStarted) return false;
            if (LookbehindCount < count) return false;
            if (_ValidPreviousValues < count) return false;
            value = _Previous[count - 1];
            return true;
        }

        public bool MoveNext()
        {
            if(LookaheadCount == 0)
            {
                return NoLookaroundMoveNext();
            }
            return State switch
            {
                EnumeratorState.NotStarted => FirstMoveNext(),
                EnumeratorState.Active => SubsequentMoveNext(),
                EnumeratorState.Complete => false,
                _ => throw new NotImplementedException(),
            };

            bool NoLookaroundMoveNext()
            {
                if (State == EnumeratorState.Complete) return false;
                State = EnumeratorState.Active;
                UnderlyingEnumeratorState = EnumeratorState.Active;
                var retVal = _Enumerator.MoveNext();
                if(retVal == false)
                {
                    State = EnumeratorState.Complete;
                }
                Current = retVal ? _Enumerator.Current : Current;
                return retVal;
            }
        }

        private bool FirstMoveNext()
        {
            if (_Enumerator.MoveNext() == false)
            {
                UnderlyingEnumeratorState = EnumeratorState.Complete;
                State = EnumeratorState.Complete;
                return false;
            }
            UnderlyingEnumeratorState = EnumeratorState.Active;
            State = EnumeratorState.Active;
            Current = _Enumerator.Current;
            _ValidNextValues = 0;
            while (_ValidNextValues < LookaheadCount)
            {
                if (_Enumerator.MoveNext() == false)
                {
                    UnderlyingEnumeratorState = EnumeratorState.Complete;
                    break;
                }
                _Next[_ValidNextValues++] = _Enumerator.Current;
            }
            return true;
        }

        private bool SubsequentMoveNext()
        {
            if(LookbehindCount > 0)
            {
                _Previous.ShiftRight(1);
                _Previous[0] = Current;
                _ValidPreviousValues = (_ValidPreviousValues + 1).ClampMax(_Previous.Length);
            }

            if(_ValidNextValues == 0)
            {
                State = EnumeratorState.Complete;
                return false;
            }

            _Next.ShiftLeft(1, out var newCurrent);
            if(newCurrent != null && newCurrent.Length > 0)
            {
                Current = newCurrent[0];
            }
            --_ValidNextValues;
            
            if(UnderlyingEnumeratorState != EnumeratorState.Complete)
            {
                if(_Enumerator.MoveNext())
                {
                    _Next[_Next.Length - 1] = _Enumerator.Current;
                    ++_ValidNextValues;
                }
                else
                {
                    UnderlyingEnumeratorState = EnumeratorState.Complete;
                }
            }
            return true;
        }

        public void Reset()
        {
            _Enumerator.Reset();
        }

        public void Dispose()
        {
            _Enumerator.Dispose();
        }


        public T MoveNextAndGetValue(out bool moveNextReturn)
        {
            moveNextReturn = MoveNext();
            return Current;
        }

        public T[] Read(int count)
        {
            if(count <= 0)
            {
                return Array.Empty<T>();
            }

            var returnArray = new T[count];
            var i = 0;
            returnArray[i++] = Current;
            while(i < count && MoveNext())
            {
                returnArray[i++] = Current;
            }
            return returnArray;
        }

        public bool MoveNext(int count)
        {
            for(var i = 0; i < count; ++i)
            {
                if(MoveNext() == false)
                {
                    return false;
                }
            }
            return true;
        }

        public bool TryGetNextArray(int count, out T[] value)
        {
            var items = new List<T>();
            foreach(var num in Enumerable.Range(1, count))
            {
                if(TryGetNext(out var item, num))
                {
                    items.Add(item);
                    continue;
                }
                break;
            }
            value = items.ToArray();
            return value.Length == count;
        }




        public int StartBacktrackSession()
        {
            throw new NotImplementedException();
        }

        public bool ResetBacktrackSession(int id)
        {
            throw new NotImplementedException();
        }

        public bool CommitBacktrackSession(int id)
        {
            throw new NotImplementedException();
        }
    }

}
