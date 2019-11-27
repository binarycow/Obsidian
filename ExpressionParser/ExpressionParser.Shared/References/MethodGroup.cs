using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text;
using ExpressionParser.Configuration;
using ExpressionParser.Parsing;

namespace ExpressionParser.References
{
    public abstract class MethodGroup
    {
        public MethodGroup(string methodName)
        {
            MethodName = methodName;
        }
        public string MethodName { get; }

        public static MethodGroup Create(FunctionDefinition functionDefinition)
        {
            return new FunctionMethodGroup(functionDefinition);
        }
        public static MethodGroup Create(Expression expression, string methodName)
        {
            return new ExpressionMethodGroup(expression, methodName);
        }
    }
}
