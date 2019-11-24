using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public sealed class CheckoutObject<T> : IDisposable
    {
        public CheckoutObject(T checkedOutObject)
        {
            CheckedOutObject = checkedOutObject;
        }
        public T CheckedOutObject { get; }
        private bool disposed = false;
        public event EventHandler? Disposing;
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
    public static class CheckoutObject
    {
        public static CheckoutObject<T> Create<T>(T obj)
        {
            return new CheckoutObject<T>(obj);
        }
    }

}
