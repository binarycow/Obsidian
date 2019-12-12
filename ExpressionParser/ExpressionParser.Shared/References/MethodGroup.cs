using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text;
using ExpressionParser.Configuration;
using ExpressionParser.Parsing;

namespace ExpressionParser.References
{
    internal abstract class MethodGroup
    {
        internal MethodGroup(string methodName)
        {
            MethodName = methodName;
        }
        internal string MethodName { get; }

        internal static ScopedFunctionMethodGroup Create(ScopedUserDefinedFunction scopedFunction)
        {
            return new ScopedFunctionMethodGroup(scopedFunction);
        }

        internal static MethodGroup Create(UserDefinedFunction functionDefinition)
        {
            return new FunctionMethodGroup(functionDefinition);
        }
        internal static MethodGroup Create(UserDefinedTest functionDefinition)
        {
            return new TestMethodGroup(functionDefinition);
        }
        internal static MethodGroup Create(UserDefinedFunction functionDefinition, object? referredObject)
        {
            return new PipelineMethodGroup(functionDefinition, referredObject);
        }
        internal static MethodGroup Create(Expression expression, string methodName)
        {
            return new ExpressionMethodGroup(expression, methodName);
        }

    }
}
