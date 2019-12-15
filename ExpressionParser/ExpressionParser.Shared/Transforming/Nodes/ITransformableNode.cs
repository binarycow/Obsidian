using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressionParser.Transforming.Nodes
{
    internal interface ITransformableNode
    {
        TOutput Transform<TOutput>(INodeTransformVisitor<TOutput> visitor);
    }
}
