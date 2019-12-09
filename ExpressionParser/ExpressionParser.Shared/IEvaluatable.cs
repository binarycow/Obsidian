using System;
using System.Collections.Generic;
using System.Text;
using ExpressionParser.Scopes;
using ExpressionParser.Transforming.Nodes;

namespace ExpressionParser
{
    internal interface IEvaluatable
    {
        object? Transform<TScope, TRootScope>(DynamicTransformer<TScope, TRootScope> transformer)
            where TScope : DynamicScope
            where TRootScope : TScope;
    }
}
