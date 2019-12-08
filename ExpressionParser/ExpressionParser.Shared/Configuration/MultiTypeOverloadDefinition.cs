using ExpressionParser.Scopes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser.Configuration
{
    internal class MultiTypeOverloadDefinition : OverloadDefinition
    {
        internal MultiTypeOverloadDefinition(Func<object?[], object?> func, Type returnType, params Type[] argumentTypes) : base(func, returnType)
        {
            ArgumentTypes = argumentTypes;
        }
        internal MultiTypeOverloadDefinition(Action<object?[]>? action, Type returnType, params Type[] argumentTypes) : base(action, returnType)
        {
            ArgumentTypes = argumentTypes;
        }
        internal MultiTypeOverloadDefinition(Func<IScope, object?[], object?> func, Type returnType, params Type[] argumentTypes) : base(func, returnType)
        {
            ArgumentTypes = argumentTypes;
        }
        internal MultiTypeOverloadDefinition(Action<IScope, object?[]>? action, Type returnType, params Type[] argumentTypes) : base(action, returnType)
        {
            ArgumentTypes = argumentTypes;
        }

        internal Type[] ArgumentTypes { get; }
    }
}
