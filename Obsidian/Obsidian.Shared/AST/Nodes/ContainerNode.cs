using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Obsidian.Transforming;
using Obsidian.WhiteSpaceControl;

namespace Obsidian.AST.Nodes
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class ContainerNode : AbstractContainerNode
    {
        public ContainerNode(IEnumerable<ASTNode> children, WhiteSpaceControlMode startWhiteSpace, WhiteSpaceControlMode endWhiteSpace)
            : base(children, startWhiteSpace, endWhiteSpace)
        {
        }

        private string DebuggerDisplay => $"{nameof(ContainerNode)}";

        public override TOutput Transform<TOutput>(ITransformVisitor<TOutput> visitor)
        {
            return visitor.Transform(this);
        }

        public override TOutput Transform<TOutput>(IForceTransformVisitor<TOutput> visitor, bool force)
        {
            return visitor.Transform(this, force);
        }
    }
}
