using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text;
using ExpressionParser.Configuration;
using ExpressionParser.Parsing;

namespace ExpressionParser.References
{
    internal class FunctionMethodGroup : MethodGroup
    {
        internal FunctionMethodGroup(UserDefinedFunction functionDefinition) : base(functionDefinition.Declaration.Name)
        {
            FunctionDefinition = functionDefinition;
        }

        internal UserDefinedFunction FunctionDefinition { get; }

    }
}
