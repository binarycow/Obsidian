using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExpressionParser.Transforming.Nodes;

namespace ExpressionParser.Parsing
{
    public class ListNode : ASTNode
    {
        public ListNode(IEnumerable<ASTNode> listItems) : base(listItems.SelectMany(child => child.Tokens))
        {
            ListItems = listItems.ToArrayWithoutInstantiation();
        }

        public ASTNode[] ListItems { get; }

        public override TOutput Transform<TOutput>(INodeTransformVisitor<TOutput> visitor)
        {
            return visitor.Transform(this);
        }
    }
}
