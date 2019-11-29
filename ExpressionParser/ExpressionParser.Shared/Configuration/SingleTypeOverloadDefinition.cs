using ExpressionParser.Scopes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser.Configuration
{
    public class SingleTypeOverloadDefinition : OverloadDefinition
    {
        public SingleTypeOverloadDefinition(Func<object?[], object?> func, Type returnType, Type argumentType,
            int minimumArguments, int maximumArguments) : base(func, returnType)
        {
            ArgumentType = argumentType;
            MinimumArguments = minimumArguments;
            MaximumArguments = maximumArguments;
        }
        public SingleTypeOverloadDefinition(Action<object?[]> action, Type returnType, Type argumentType,
            int minimumArguments, int maximumArguments) : base(action, returnType)
        {
            ArgumentType = argumentType;
            MinimumArguments = minimumArguments;
            MaximumArguments = maximumArguments;
        }
        public SingleTypeOverloadDefinition(Func<IScope, object?[], object?> func, Type returnType, Type argumentType,
            int minimumArguments, int maximumArguments) : base(func, returnType)
        {
            ArgumentType = argumentType;
            MinimumArguments = minimumArguments;
            MaximumArguments = maximumArguments;
        }
        public SingleTypeOverloadDefinition(Action<IScope, object?[]> action, Type returnType, Type argumentType,
            int minimumArguments, int maximumArguments) : base(action, returnType)
        {
            ArgumentType = argumentType;
            MinimumArguments = minimumArguments;
            MaximumArguments = maximumArguments;
        }

        public Type ArgumentType { get; }
        public int MinimumArguments { get; }
        public int MaximumArguments { get; }
    }
}
