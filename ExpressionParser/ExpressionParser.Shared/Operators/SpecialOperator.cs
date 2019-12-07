using System;
using System.Collections.Generic;
using System.Text;
using ExpressionParser.Configuration;
using ExpressionParser.Lexing;
using ExpressionParser.Transforming.Operators;
using System.Diagnostics;

namespace ExpressionParser.Operators
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class SpecialOperator : Operator
    {
        public SpecialOperator(Token token, SpecialOperatorType operatorType) : base(token)
        {
            OperatorType = operatorType;
        }

        public SpecialOperatorType OperatorType { get; }

        public override TOutput Transform<TInput, TOutput>(IOperatorTransformVisitor<TInput, TOutput> visitor, TInput[] arguments)
        {
            return visitor.Transform(this, arguments);
        }
    }
}
