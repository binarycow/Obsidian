using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser.Transforming.Operators
{
    public interface ITransformableOperator
    {
        TOutput Transform<TInput, TOutput>(IOperatorTransformVisitor<TInput, TOutput> visitor, TInput[] arguments);
    }
}
