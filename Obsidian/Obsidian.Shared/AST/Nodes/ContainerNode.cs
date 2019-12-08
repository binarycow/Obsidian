using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Obsidian.Parsing;
using Obsidian.Transforming;
using Obsidian.WhiteSpaceControl;

namespace Obsidian.AST.Nodes
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class ContainerNode : AbstractContainerNode, IWhiteSpaceControlling
    {
        internal ContainerNode(ParsingNode? startParsingNode, IEnumerable<ASTNode> children, ParsingNode? endParsingNode, WhiteSpaceControlSet? whiteSpace = null)
            : base(startParsingNode, children, endParsingNode)
        {
            WhiteSpaceControl = whiteSpace ?? new WhiteSpaceControlSet();
        }

        public WhiteSpaceControlSet WhiteSpaceControl { get; }

        private static string DebuggerDisplay => $"{nameof(ContainerNode)}";

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
