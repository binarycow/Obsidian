using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using ExpressionParser.Configuration;
using ExpressionParser.Lexing;
using ExpressionParser.Transforming.Nodes;
using ExpressionParser.Transforming.Operators;

namespace ExpressionParser.Operators
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal abstract class Operator : ITransformableOperator
    {
        public Operator(Token token)
        {
            Token = token;
        }

        public Token Token { get; }
        public static Operator CreateBinary(OperatorDefinition definition, Token token, AssignmentOperatorBehavior assignmentOperatorBehavior)
        {
            return definition switch
            {
                StandardOperatorDefinition standardOperator => new StandardOperator(token, standardOperator.OperatorType, assignmentOperatorBehavior),
                SpecialOperatorDefinition specialOperator => new SpecialOperator(token, specialOperator.OperatorType),
                _ => throw new NotImplementedException(),
            };
        }
        public static Operator CreateUnary(OperatorDefinition definition, Token token)
        {
            return definition switch
            {
                StandardOperatorDefinition standardOperator => new StandardOperator(token, standardOperator.OperatorType, assignmentOperatorBehavior: default),
                _ => throw new NotImplementedException(),
            };
        }
        internal static Operator CreateSpecial(SpecialOperatorDefinition specialOperator, Token operatorToken)
        {
            return new SpecialOperator(operatorToken, specialOperator.OperatorType);
        }

        public abstract TOutput Transform<TInput, TOutput>(IOperatorTransformVisitor<TInput, TOutput> visitor, TInput[] arguments);

        public virtual string DebuggerDisplay => Token.TextValue;
    }
}
