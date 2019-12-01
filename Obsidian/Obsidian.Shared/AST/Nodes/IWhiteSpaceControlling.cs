using System;
using System.Collections.Generic;
using System.Text;
using Obsidian.WhiteSpaceControl;

namespace Obsidian.AST.Nodes
{
    public interface IWhiteSpaceControlling
    {
        public WhiteSpaceControlSet WhiteSpaceControl { get; }
    }
}
