using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Obsidian.Transforming;
using Obsidian.WhiteSpaceControl;

namespace Obsidian.AST.Nodes.MiscNodes
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class ConditionalNode : AbstractContainerNode
    {
        public ConditionalNode(ExpressionNode expression, IEnumerable<ASTNode> children, WhiteSpaceControlMode startWhiteSpace, WhiteSpaceControlMode endWhiteSpace) :
            base(children, startWhiteSpace, endWhiteSpace)
        {
            Expression = expression;
        }

        private string DebuggerDisplay => $"{nameof(ConditionalNode)} ({Expression})";

        public ExpressionNode Expression { get; }

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
