using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressionParser.Transforming.Nodes
{
    public interface ITransformableNode
    {
        TOutput Transform<TOutput>(INodeTransformVisitor<TOutput> visitor);
    }
}
