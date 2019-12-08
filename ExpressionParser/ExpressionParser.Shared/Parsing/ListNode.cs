using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExpressionParser.Transforming.Nodes;
using System.Diagnostics;

namespace ExpressionParser.Parsing
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
#pragma warning disable CA1812 // Avoid uninstantiated internal classes
    internal class ListNode : ASTNode
#pragma warning restore CA1812 // Avoid uninstantiated internal classes
    {
        internal ListNode(IEnumerable<ASTNode> listItems) : base(listItems.SelectMany(child => child.Tokens))
        {
            ListItems = listItems.ToArrayWithoutInstantiation();
        }

        internal ASTNode[] ListItems { get; }

        internal override string DebuggerDisplay => $"[{string.Join(",", ListItems.Select(item => item.DebuggerDisplay))}]";

        public override TOutput Transform<TOutput>(INodeTransformVisitor<TOutput> visitor)
        {
            return visitor.Transform(this);
        }
    }
}
