using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExpressionParser.Configuration;
using ExpressionParser.Lexing;
using ExpressionParser.Transforming.Operators;
using System.Diagnostics;

namespace ExpressionParser.Operators
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class StandardOperator : Operator
    {
        internal StandardOperator(Token token, OperatorType operatorType, AssignmentOperatorBehavior assignmentOperatorBehavior) : base(token)
        {
            OperatorType = operatorType;
            AssignmentOperatorBehavior = assignmentOperatorBehavior;
        }

        internal OperatorType OperatorType { get; }
        internal AssignmentOperatorBehavior AssignmentOperatorBehavior { get; }
        public override TOutput Transform<TInput, TOutput>(IOperatorTransformVisitor<TInput, TOutput> visitor, TInput[] arguments)
        {
            return visitor.Transform(this, arguments);
        }
    }
}
