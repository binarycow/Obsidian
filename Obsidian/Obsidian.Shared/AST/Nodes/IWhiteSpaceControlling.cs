using System;
using System.Collections.Generic;
using System.Text;
using Obsidian.WhiteSpaceControl;

namespace Obsidian.AST.Nodes
{
    public interface IWhiteSpaceControlling
    {
        public WhiteSpaceControlMode StartWhiteSpace { get; }
        public WhiteSpaceControlMode EndWhiteSpace { get; }
    }
}
