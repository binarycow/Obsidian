using ExpressionParser.Scopes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser.Configuration
{
    internal class SingleTypeOverloadDefinition : OverloadDefinition
    {
        internal SingleTypeOverloadDefinition(Func<object?[], object?> func, Type returnType, Type argumentType,
            int minimumArguments, int maximumArguments) : base(func, returnType)
        {
            ArgumentType = argumentType;
            MinimumArguments = minimumArguments;
            MaximumArguments = maximumArguments;
        }
        internal SingleTypeOverloadDefinition(Action<object?[]> action, Type returnType, Type argumentType,
            int minimumArguments, int maximumArguments) : base(action, returnType)
        {
            ArgumentType = argumentType;
            MinimumArguments = minimumArguments;
            MaximumArguments = maximumArguments;
        }
        internal SingleTypeOverloadDefinition(Func<IScope, object?[], object?> func, Type returnType, Type argumentType,
            int minimumArguments, int maximumArguments) : base(func, returnType)
        {
            ArgumentType = argumentType;
            MinimumArguments = minimumArguments;
            MaximumArguments = maximumArguments;
        }
        internal SingleTypeOverloadDefinition(Action<IScope, object?[]> action, Type returnType, Type argumentType,
            int minimumArguments, int maximumArguments) : base(action, returnType)
        {
            ArgumentType = argumentType;
            MinimumArguments = minimumArguments;
            MaximumArguments = maximumArguments;
        }

        internal Type ArgumentType { get; }
        internal int MinimumArguments { get; }
        internal int MaximumArguments { get; }
    }
}
