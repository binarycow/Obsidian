using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obsidian.WhiteSpaceControl;

namespace Obsidian.AST.Nodes
{
    public abstract class AbstractContainerNode : ASTNode, IWhiteSpaceControlling, IWithChildren
    {
        public AbstractContainerNode(IEnumerable<ASTNode> children, WhiteSpaceControlMode startWhiteSpace, WhiteSpaceControlMode endWhiteSpace)
            : base(children.SelectMany(child => child.ParsingNodes))
        {
            Children = children.ToArrayWithoutInstantiation();
            StartWhiteSpace = startWhiteSpace;
            EndWhiteSpace = endWhiteSpace;
        }

        public ASTNode[] Children { get; }
        public WhiteSpaceControlMode StartWhiteSpace { get; }
        public WhiteSpaceControlMode EndWhiteSpace { get; }
    }
}
