using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExpressionParser.Transforming.Nodes;

namespace ExpressionParser.Parsing
{
    public class DictionaryNode : ASTNode
    {
        public DictionaryNode(IEnumerable<DictionaryItemNode> values) : base(values.SelectMany(val => val.Tokens))
        {
            Values = values.ToArrayWithoutInstantiation();
        }

        public DictionaryItemNode[] Values { get; }

        public override TOutput Transform<TOutput>(INodeTransformVisitor<TOutput> visitor)
        {
            return visitor.Transform(this);
        }
    }
}
