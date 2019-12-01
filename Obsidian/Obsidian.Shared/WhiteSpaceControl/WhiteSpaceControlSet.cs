using System;
using System.Collections.Generic;
using System.Text;

namespace Obsidian.WhiteSpaceControl
{
    public class WhiteSpaceControlSet
    {
        public WhiteSpaceControlSet(WhiteSpaceMode? start = null, WhiteSpaceMode? end = null)
        {
            Start = start ?? WhiteSpaceMode.Default;
            End = end ?? WhiteSpaceMode.Default;
        }
        public WhiteSpaceMode Start { get; }
        public WhiteSpaceMode End { get; }
    }
}
