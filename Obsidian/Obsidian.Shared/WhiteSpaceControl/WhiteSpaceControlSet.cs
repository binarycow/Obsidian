using System;
using System.Collections.Generic;
using System.Text;

namespace Obsidian.WhiteSpaceControl
{
    internal class WhiteSpaceControlSet
    {
        internal WhiteSpaceControlSet(WhiteSpaceMode? start = null, WhiteSpaceMode? end = null)
        {
            Start = start ?? WhiteSpaceMode.Default;
            End = end ?? WhiteSpaceMode.Default;
        }
        internal WhiteSpaceMode Start { get; }
        internal WhiteSpaceMode End { get; }
    }
}
