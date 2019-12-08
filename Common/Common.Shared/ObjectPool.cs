using System;
using System.Collections.Concurrent;

namespace Common
{
    internal class ObjectPool<T>
    {
        internal ObjectPool(Func<T> creationFunction, Action<T> clearAction)
        {
            _Available = new Lazy<ConcurrentQueue<CheckoutObject<T>>>(); 
            _CheckedOut = new Lazy<ConcurrentDictionary<CheckoutObject<T>, byte>>();
            _CreationFunction = creationFunction;
            _ClearAction = clearAction;
        }


        private readonly Lazy<ConcurrentQueue<CheckoutObject<T>>> _Available;
        private ConcurrentQueue<CheckoutObject<T>> Available => _Available.Value;

        private readonly Lazy<ConcurrentDictionary<CheckoutObject<T>, byte>> _CheckedOut;
        private ConcurrentDictionary<CheckoutObject<T>, byte> CheckedOut => _CheckedOut.Value;
        private readonly Func<T> _CreationFunction;
        private readonly Action<T> _ClearAction;

        internal CheckoutObject<T> Checkout()
        {
            if (Available.Count > 0 && Available.TryDequeue(out CheckoutObject<T> checkoutRecord) == false)
            {
                CheckedOut.TryAdd(checkoutRecord, 0);
                return checkoutRecord;
            }
            checkoutRecord = new CheckoutObject<T>(_CreationFunction());
            checkoutRecord.Disposing += CheckedOutItem_Disposing;
            CheckedOut.TryAdd(checkoutRecord, 0);
            return checkoutRecord;
        }

        private void CheckedOutItem_Disposing(object sender, EventArgs e)
        {
            if (sender is CheckoutObject<T> checkoutObj)
            {
                Return(checkoutObj);
            }
        }

        internal void Return(CheckoutObject<T> checkoutRecord)
        {
            CheckedOut.TryRemove(checkoutRecord, out _);
            _ClearAction(checkoutRecord.CheckedOutObject);
            Available.Enqueue(checkoutRecord);
        }
    }
}
