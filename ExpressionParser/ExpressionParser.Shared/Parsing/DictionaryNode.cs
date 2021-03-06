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
    internal class DictionaryNode : ASTNode
#pragma warning restore CA1812 // Avoid uninstantiated internal classes
    {
        internal DictionaryNode(IEnumerable<DictionaryItemNode> values) : base(values.SelectMany(val => val.Tokens))
        {
            Values = values.ToArrayWithoutInstantiation();
        }

        internal DictionaryItemNode[] Values { get; }

        internal override string DebuggerDisplay => $"{{{string.Join(",", Values.Select(item => item.DebuggerDisplay))}}}";
        public override TOutput Transform<TOutput>(INodeTransformVisitor<TOutput> visitor)
        {
            return visitor.Transform(this);
        }
    }
}
