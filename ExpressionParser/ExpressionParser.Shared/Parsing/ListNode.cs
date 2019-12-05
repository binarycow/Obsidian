using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExpressionParser.Transforming.Nodes;
using System.Diagnostics;

namespace ExpressionParser.Parsing
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class ListNode : ASTNode
    {
        public ListNode(IEnumerable<ASTNode> listItems) : base(listItems.SelectMany(child => child.Tokens))
        {
            ListItems = listItems.ToArrayWithoutInstantiation();
        }

        public ASTNode[] ListItems { get; }

        public override string DebuggerDisplay => $"[{string.Join(",", ListItems.Select(item => item.DebuggerDisplay))}]";

        public override TOutput Transform<TOutput>(INodeTransformVisitor<TOutput> visitor)
        {
            return visitor.Transform(this);
        }
    }
}
