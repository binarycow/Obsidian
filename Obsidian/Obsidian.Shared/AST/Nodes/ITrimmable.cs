using Obsidian.WhiteSpaceControl;
using System;
using System.Collections.Generic;
using System.Text;

namespace Obsidian.AST.Nodes
{
    public interface ITrimmable
    {
        public WhiteSpaceMode StartMode { get; }
        public WhiteSpaceMode EndMode { get; }

    }
}
