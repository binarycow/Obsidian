using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Obsidian
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class LoopInfoClass<T> where T : class
    {
        public LoopInfoClass(T?[] array, int currentindex)
        {
            array = array ?? throw new ArgumentNullException(nameof(array));
            index0 = currentindex;
            revindex0 = array.Length - 1 - currentindex;
            first = currentindex == 0;
            last = currentindex == array.Length - 1;
            length = array.Length;
            previtem = (first ? null : array[currentindex - 1])!;
            nextitem = (last ? null : array[currentindex + 1])!;
        }

        public int index0 { get; }
        public int index => index0 + 1;
        public int revindex0 { get; }
        public int revindex => revindex0 + 1;
        public bool first { get; }
        public bool last { get; }
        public int length { get; }
        public T previtem { get; }
        public T nextitem { get; }
    }
}
