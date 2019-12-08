using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser.References
{
    internal class PipelineMethodGroup : MethodGroup
    {
        internal PipelineMethodGroup(UserDefinedFunction functionDefinition, object? referredObject = null) : base(functionDefinition.Declaration.Name)
        {
            FunctionDefinition = functionDefinition;
            ReferredObject = referredObject;
        }

        internal UserDefinedFunction FunctionDefinition { get; }
        internal object? ReferredObject { get; }
    }
}
