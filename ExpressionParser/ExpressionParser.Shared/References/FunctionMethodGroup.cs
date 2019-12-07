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
        public FunctionMethodGroup(UserDefinedFunction functionDefinition) : base(functionDefinition.Declaration.Name)
        {
            FunctionDefinition = functionDefinition;
        }

        public UserDefinedFunction FunctionDefinition { get; }

    }
}
