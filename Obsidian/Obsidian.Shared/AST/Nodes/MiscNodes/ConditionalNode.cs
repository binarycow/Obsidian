using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Obsidian.Parsing;
using Obsidian.Transforming;
using Obsidian.WhiteSpaceControl;

namespace Obsidian.AST.Nodes.MiscNodes
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class ConditionalNode : AbstractContainerNode, IWhiteSpaceControlling
    {
        public ConditionalNode(ParsingNode? startParsingNode, ExpressionNode expression, IEnumerable<ASTNode> children, ParsingNode? endParsingNode, WhiteSpaceControlSet? whiteSpace = null) :
            base(startParsingNode, children, endParsingNode)
        {
            Expression = expression;
            WhiteSpaceControl = whiteSpace ?? new WhiteSpaceControlSet();
        }

        private string DebuggerDisplay => $"{nameof(ConditionalNode)} ({Expression})";

        public ExpressionNode Expression { get; }

        public WhiteSpaceControlSet WhiteSpaceControl { get; }

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
