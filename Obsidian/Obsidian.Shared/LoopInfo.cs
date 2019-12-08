using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Obsidian
{
    internal class LoopInfoClass<T> where T : class
    {
        internal LoopInfoClass(T?[] array, int currentindex)
        {
            index0 = currentindex;
            revindex0 = array.Length - 1 - currentindex;
            first = currentindex == 0;
            last = currentindex == array.Length - 1;
            length = array.Length;
            previtem = (first ? null : array[currentindex - 1])!;
            nextitem = (last ? null : array[currentindex + 1])!;
        }
        internal int index0 { get; }
        internal int index => index0 + 1;
        internal int revindex0 { get; }
        internal int revindex => revindex0 + 1;
        internal bool first { get; }
        internal bool last { get; }
        internal int length { get; }
        internal T previtem { get; }
        internal T nextitem { get; }
    }
}
