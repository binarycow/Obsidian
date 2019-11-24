using System;
using System.Collections.Generic;
using System.Text;
using ExpressionParser.Configuration;
using ExpressionParser.Lexing;
using ExpressionParser.Transforming.Nodes;
using ExpressionParser.Transforming.Operators;

namespace ExpressionParser.Operators
{
    public abstract class Operator : ITransformableOperator
    {
        public Operator(Token token)
        {
            Token = token;
        }

        public Token Token { get; }
        public static Operator CreateBinary(OperatorDefinition definition, Token token)
        {
            switch(definition)
            {
                case StandardOperatorDefinition standardOperator:
                    return new StandardOperator(token, standardOperator.OperatorType);
                case SpecialOperatorDefinition specialOperator:
                    return new SpecialOperator(token, specialOperator.OperatorType, SpecialOperatorSubType.Unknown);
                default:
                    throw new NotImplementedException();
            }
        }
        public static Operator CreateUnary(OperatorDefinition definition, Token token)
        {
            switch (definition)
            {
                case StandardOperatorDefinition standardOperator:
                    return new StandardOperator(token, standardOperator.OperatorType);
                default:
                    throw new NotImplementedException();
            }
        }
        internal static Operator CreateMemberAccess(SpecialOperatorDefinition specialOperator, Token operatorToken, SpecialOperatorSubType subType)
        {
            return new SpecialOperator(operatorToken, specialOperator.OperatorType, subType);
        }

        public abstract TOutput Transform<TInput, TOutput>(IOperatorTransformVisitor<TInput, TOutput> visitor, TInput[] arguments);

    }
}
