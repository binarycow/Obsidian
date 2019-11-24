using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExpressionParser.Configuration;
using ExpressionParser.Lexing;
using ExpressionParser.Transforming.Operators;

namespace ExpressionParser.Operators
{
    public class StandardOperator : Operator
    {
        public StandardOperator(Token token, OperatorType operatorType) : base(token)
        {
            OperatorType = operatorType;
        }

        public OperatorType OperatorType { get; }
        public override TOutput Transform<TInput, TOutput>(IOperatorTransformVisitor<TInput, TOutput> visitor, TInput[] arguments)
        {
            return visitor.Transform(this, arguments);
        }
    }
}
