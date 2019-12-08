using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    internal sealed class CheckoutObject<T> : IDisposable
    {
        internal CheckoutObject(T checkedOutObject)
        {
            CheckedOutObject = checkedOutObject;
        }
        internal T CheckedOutObject { get; }
        private bool disposed = false;
        internal event EventHandler? Disposing;
        private void Dispose(bool disposing)
        {
            if (disposed) return;
            if(disposing)
            {
                Disposing?.Invoke(this, EventArgs.Empty);
                if (CheckedOutObject is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~CheckoutObject()
        {
            Dispose(false);
        }
    }
    internal static class CheckoutObject
    {
        internal static CheckoutObject<T> Create<T>(T obj)
        {
            return new CheckoutObject<T>(obj);
        }
    }

}
