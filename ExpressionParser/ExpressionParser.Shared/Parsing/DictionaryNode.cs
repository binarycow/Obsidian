using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExpressionParser.Transforming.Nodes;
using System.Diagnostics;

namespace ExpressionParser.Parsing
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class DictionaryNode : ASTNode
    {
        public DictionaryNode(IEnumerable<DictionaryItemNode> values) : base(values.SelectMany(val => val.Tokens))
        {
            Values = values.ToArrayWithoutInstantiation();
        }

        public DictionaryItemNode[] Values { get; }

        public override string DebuggerDisplay => $"{{{string.Join(",", Values.Select(item => item.DebuggerDisplay))}}}";
        public override TOutput Transform<TOutput>(INodeTransformVisitor<TOutput> visitor)
        {
            return visitor.Transform(this);
        }
    }
}
