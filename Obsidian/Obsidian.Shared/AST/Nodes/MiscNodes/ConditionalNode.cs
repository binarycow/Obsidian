using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Obsidian.Parsing;
using Obsidian.Transforming;

namespace Obsidian.AST.Nodes.MiscNodes
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class ConditionalNode : AbstractContainerNode
    {
        public ConditionalNode(ParsingNode? startParsingNode, ExpressionNode expression, IEnumerable<ASTNode> children, ParsingNode? endParsingNode) :
            base(startParsingNode, children, endParsingNode)
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
        public override void Transform(ITransformVisitor visitor)
        {
            visitor.Transform(this);
        }
    }
}
