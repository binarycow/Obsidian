using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class ArrayExtensions
    {
        public static void ShiftLeft<T>(this T[] array, int count, out T[] poppedObjects)
        {
            ShiftLeft(array, count, out var nullablePoppedObjects, true);
            poppedObjects = nullablePoppedObjects ?? Array.Empty<T>();
        }
        public static void ShiftLeft<T>(this T[] array, int count)
        {
            ShiftLeft(array, count, out _, false);
        }
        private static void ShiftLeft<T>(this T[] array, int count, out T[]? poppedObjects, bool popObjects)
        {
            if (count <= 0)
            {
                poppedObjects = null;
                return;
            }
            poppedObjects = popObjects ? new T[count] : null;

            if (poppedObjects != null)
            {
                for (int idx = 0; idx < count; ++idx)
                {
                    var index = idx;
                    poppedObjects[poppedObjects.Length - 1 - idx] = array[index];
                }
            }
            for (int idx = 0; idx < array.Length - count; ++idx)
            {
                array[idx] = array[idx + count];
            }
            for (int idx = array.Length - count; idx < array.Length; ++idx)
            {
                array[idx] = default!;
            }
        }
        public static void ShiftRight<T>(this T[] array, int count, out T[] poppedObjects)
        {
            ShiftRight(array, count, out var nullablePoppedObjects, true);
            poppedObjects = nullablePoppedObjects ?? Array.Empty<T>();
        }
        public static void ShiftRight<T>(this T[] array, int count)
        {
            ShiftRight(array, count, out _, false);
        }
        private static void ShiftRight<T>(this T[] array, int count, out T[]? poppedObjects, bool popObjects)
        {
            if (count <= 0)
            {
                poppedObjects = null;
                return;
            }
            poppedObjects = popObjects ? new T[count] : null;

            if (poppedObjects != null)
            {
                for (int idx = 0; idx < count; ++idx)
                {
                    var index = array.Length - 1 - idx;
                    poppedObjects[idx] = array[index];
                }
            }
            var sourceIndexStart = array.Length - count - 1;
            var destIndexStart = array.Length - 1;
            for (int idx = 0; idx < array.Length - count; ++idx)
            {
                var sourceIndex = sourceIndexStart - idx;
                var destinationIndex = destIndexStart - idx;
                array[destinationIndex] = array[sourceIndex];
            }
            for(int idx = 0; idx < count; ++idx)
            {
                array[idx] = default!;
            }
        }
    }
}
