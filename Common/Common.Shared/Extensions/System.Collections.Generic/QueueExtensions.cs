using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Collections.Generic
{
    internal static class QueueExtensions
    {
        internal static void Enqueue<T>(this Queue<T> queue, IEnumerable<T> items)
        {
            foreach(var item in items)
            {
                queue.Enqueue(item);
            }
        }
    }
}
