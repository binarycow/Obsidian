using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obsidian.Parsing
{
    internal enum ParsingNodeType
    {
        Statement,
        NewLine,
        WhiteSpace,
        Expression,
        Output,
        Comment,
        Empty,
    }
}
