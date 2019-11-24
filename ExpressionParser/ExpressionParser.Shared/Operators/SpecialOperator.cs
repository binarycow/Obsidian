using System;
using System.Collections.Generic;
using System.Text;
using ExpressionParser.Configuration;
using ExpressionParser.Lexing;
using ExpressionParser.Transforming.Operators;

namespace ExpressionParser.Operators
{
    public class SpecialOperator : Operator
    {
        public SpecialOperator(Token token, SpecialOperatorType operatorType, SpecialOperatorSubType subType) : base(token)
        {
            OperatorType = operatorType;
            SubType = subType;
        }

        public SpecialOperatorType OperatorType { get; }
        public SpecialOperatorSubType SubType { get; }

        public override TOutput Transform<TInput, TOutput>(IOperatorTransformVisitor<TInput, TOutput> visitor, TInput[] arguments)
        {
            return visitor.Transform(this, arguments);
        }
    }
}
