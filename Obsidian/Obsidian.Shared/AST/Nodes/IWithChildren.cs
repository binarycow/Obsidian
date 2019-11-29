using System;
using System.Collections.Generic;
using System.Text;

namespace Obsidian.AST.Nodes
{
    public interface IWithChildren
    {
        public ASTNode[] Children { get; }
    }
}
