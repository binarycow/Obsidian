using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Common.Collections;
using Obsidian.Parsing;
using Obsidian.Transforming;
using Obsidian.WhiteSpaceControl;

namespace Obsidian.AST.Nodes.MiscNodes
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class WhiteSpaceNode : ASTNode, IWhiteSpace
    {
        internal WhiteSpaceNode(IEnumerable<ParsingNode> parsingNodes) : base(null, parsingNodes, null)
        {
        }
        internal WhiteSpaceNode(ParsingNode parsingNode) : base(parsingNode)
        {
        }

        public WhiteSpaceMode WhiteSpaceMode { get; set; }

        private string DebuggerDisplay => $"{nameof(WhiteSpaceNode)} : \"{ToString(debug: true)}\" Mode: {WhiteSpaceMode}";
        internal static WhiteSpaceNode Parse(ILookaroundEnumerator<ParsingNode> enumerator)
        {
            var nodes = new Queue<ParsingNode>();

            nodes.Enqueue(enumerator.Current);

            while (enumerator.TryGetNext(out var nextNode) && nextNode.NodeType == ParsingNodeType.WhiteSpace)
            {
                enumerator.MoveNext();
                nodes.Enqueue(nextNode);
            }
            return new WhiteSpaceNode(nodes);
        }
        public override TOutput Transform<TOutput>(ITransformVisitor<TOutput> visitor)
        {
            return visitor.Transform(this);
        }
        public override void Transform(ITransformVisitor visitor)
        {
            visitor.Transform(this);
        }

        public override void Transform(IManualWhiteSpaceTransformVisitor visitor, bool inner = false)
        {
            visitor.Transform(this, inner);
        }
        public override TOutput Transform<TOutput>(IForceTransformVisitor<TOutput> visitor, bool force)
        {
            return visitor.Transform(this, force);
        }
    }
}
