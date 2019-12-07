using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ExpressionParser.Transforming.Nodes;

namespace ExpressionParser.Parsing
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class TupleNode : ASTNode
    {
        public TupleNode(IEnumerable<ASTNode> listItems) : base(listItems.SelectMany(child => child.Tokens))
        {
            TupleItems = listItems.ToArrayWithoutInstantiation();
        }

        public ASTNode[] TupleItems { get; }

        public override string DebuggerDisplay => $"({string.Join(",", TupleItems.Select(item => item.DebuggerDisplay))})";

        public override TOutput Transform<TOutput>(INodeTransformVisitor<TOutput> visitor)
        {
            return visitor.Transform(this);
        }
    }
}
