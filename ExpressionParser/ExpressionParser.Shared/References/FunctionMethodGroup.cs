using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text;
using ExpressionParser.Configuration;
using ExpressionParser.Parsing;

namespace ExpressionParser.References
{
    public class FunctionMethodGroup : MethodGroup
    {
        public FunctionMethodGroup(FunctionDefinition functionDefinition) : base(functionDefinition.Name)
        {
            FunctionDefinition = functionDefinition;
        }

        public FunctionDefinition FunctionDefinition { get; }

    }
}
