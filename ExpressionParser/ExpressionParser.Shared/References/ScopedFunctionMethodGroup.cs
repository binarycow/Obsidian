using System;
using System.Collections.Generic;
using System.Text;
using ExpressionParser.Scopes;
using ExpressionParser.Transforming.Nodes;

namespace ExpressionParser.References
{
    internal class ScopedFunctionMethodGroup : MethodGroup
    {
        internal ScopedFunctionMethodGroup(ScopedUserDefinedFunction functionDefinition) : base(functionDefinition.Declaration.Name)
        {
            FunctionDefinition = functionDefinition;
        }

        internal ScopedUserDefinedFunction FunctionDefinition { get; }
    }
}
