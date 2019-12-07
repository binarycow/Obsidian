using System;
using System.Collections.Generic;
using System.Text;
using Obsidian.WhiteSpaceControl;

namespace Obsidian.AST.Nodes
{
    internal interface IWhiteSpaceControlling
    {
        public WhiteSpaceControlSet WhiteSpaceControl { get; }
    }
}
