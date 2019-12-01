using System;
using System.Collections.Generic;
using System.Text;

namespace Obsidian.WhiteSpaceControl
{
    public class WhiteSpaceControlSet
    {
        public WhiteSpaceControlSet(WhiteSpaceMode? outsideStart = null, WhiteSpaceMode? outsideEnd = null, 
            WhiteSpaceMode? insideStart = null, WhiteSpaceMode? insideEnd = null)
        {
            OutsideStart = outsideStart ?? WhiteSpaceMode.Default;
            OutsideEnd = outsideEnd ?? WhiteSpaceMode.Default;
            InsideStart = insideStart ?? WhiteSpaceMode.Default;
            InsideEnd = insideEnd ?? WhiteSpaceMode.Default;
        }
        public WhiteSpaceMode OutsideStart { get; }
        public WhiteSpaceMode OutsideEnd { get; }
        public WhiteSpaceMode InsideStart { get; }
        public WhiteSpaceMode InsideEnd { get; }
    }
}
