using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser.Configuration
{
    public class MultiTypeOverloadDefinition : OverloadDefinition
    {
        public MultiTypeOverloadDefinition(Func<object?[], object?> func, Type returnType, params Type[] argumentTypes) : base(func, returnType)
        {
            ArgumentTypes = argumentTypes;
        }
        public MultiTypeOverloadDefinition(Action<object?[]>? action, Type returnType, params Type[] argumentTypes) : base(action, returnType)
        {
            ArgumentTypes = argumentTypes;
        }

        public Type[] ArgumentTypes { get; }
    }
}
