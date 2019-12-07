using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser.Transforming.Operators
{
    internal interface ITransformableOperator
    {
        TOutput Transform<TInput, TOutput>(IOperatorTransformVisitor<TInput, TOutput> visitor, TInput[] arguments);
    }
}
