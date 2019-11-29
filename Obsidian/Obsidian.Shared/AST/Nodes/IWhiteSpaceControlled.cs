using System;
using System.Collections.Generic;
using System.Text;
using Obsidian.WhiteSpaceControl;

namespace Obsidian.AST.Nodes
{
    public interface IWhiteSpaceControlled
    {
        public WhiteSpaceControlMode ControlMode { get; set; }
    }
}
