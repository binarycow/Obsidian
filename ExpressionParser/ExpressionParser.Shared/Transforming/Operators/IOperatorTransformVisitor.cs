using System;
using System.Collections.Generic;
using System.Text;
using ExpressionParser.Operators;

namespace ExpressionParser.Transforming.Operators
{
    internal interface IOperatorTransformVisitor<TInput, TOutput>
    {
        TOutput Transform(StandardOperator item, TInput[] args);
        TOutput Transform(SpecialOperator item, TInput[] args);
    }
}
