using ExpressionParser;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Obsidian
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class LoopInfo
    {
        internal LoopInfo(object?[] array, Func<object?[], object?>? callable = null, int depth = 0)
        {
            _Array = array ?? throw new ArgumentNullException(nameof(array));
            index0 = 0;
            depth0 = depth;
            Callable = callable;
        }

        private object?[] _Array;
        public int depth0 { get; }
        public int depth => depth0 + 1;
        public int index0 { get; internal set; }
        public int index => index0 + 1;
        public int revindex0 => _Array.Length - index0;
        public int revindex => revindex0 + 1;
        public bool first => index0 == 0;
        public bool last => index0 == _Array.Length - 1;
        public int length => _Array.Length;
        public object? previtem => index0 > 0 ? _Array[index0 - 1] : default;
        public object? nextitem => index0 < _Array.Length - 1 ? _Array[index0 + 1] : default;


        internal Func<object?[], object?>? Callable { get; set; }
        [Callable]
        internal object? Call(object?[] args)
        {
            return Callable?.Invoke(args);
        }
    }
}
