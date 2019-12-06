using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser.References
{
    public class PipelineMethodGroup : MethodGroup
    {
        public PipelineMethodGroup(UserDefinedFunction functionDefinition, object? referredObject = null) : base(functionDefinition.Declaration.Name)
        {
            FunctionDefinition = functionDefinition;
            ReferredObject = referredObject;
        }

        public UserDefinedFunction FunctionDefinition { get; }
        public object? ReferredObject { get; }
    }
}
