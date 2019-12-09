using System;
using System.Collections.Generic;
using System.Text;
using ExpressionParser.Scopes;
using ExpressionParser.Transforming.Nodes;

namespace ExpressionParser.References
{
    internal class PipelineMethodGroup : MethodGroup, IEvaluatable
    {
        internal PipelineMethodGroup(UserDefinedFunction functionDefinition, object? referredObject = null) : base(functionDefinition.Declaration.Name)
        {
            FunctionDefinition = functionDefinition;
            ReferredObject = referredObject;
        }

        internal UserDefinedFunction FunctionDefinition { get; }
        internal object? ReferredObject { get; }

        public object? Transform<TScope, TRootScope>(DynamicTransformer<TScope, TRootScope> transformer)
            where TScope : DynamicScope
            where TRootScope : TScope
        {
            return transformer.Transform(this);
        }
    }
}
