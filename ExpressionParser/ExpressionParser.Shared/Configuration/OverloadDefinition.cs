using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser.Configuration
{
    public abstract class OverloadDefinition
    {
        public OverloadDefinition(Func<object?[], object?> func, Type returnType)
        {
            ReturnType = returnType;
            Function = func;
            Action = default;
        }
        public OverloadDefinition(Action<object?[]>? action, Type returnType)
        {
            ReturnType = returnType;
            Function = default;
            Action = action;
        }

        public Func<object?[], object?>? Function { get; }
        public Action<object?[]>? Action { get; }
        public Type ReturnType { get; set; }

        public static OverloadDefinition CreateEmpty(Func<object?> func, Type returnType)
        {
            return new SingleTypeOverloadDefinition(args => func(), returnType, typeof(void), 0, 0);
        }
        public static OverloadDefinition CreateEmptyVoid(Action func)
        {
            return new SingleTypeOverloadDefinition(args => func(), typeof(void), typeof(void), 0, 0);
        }

        public static OverloadDefinition CreateSingleType(Func<object?[], object?> func, Type returnType, int minimumArguments, int maximumArguments, Type argumentType)
        {
            return new SingleTypeOverloadDefinition(func, returnType, argumentType, minimumArguments, maximumArguments);
        }
        public static OverloadDefinition CreateSingleType(Func<object?[], object?> func, Type returnType, int minimumArguments, Type argumentType)
        {
            return new SingleTypeOverloadDefinition(func, returnType, argumentType, minimumArguments, int.MaxValue);
        }
        public static OverloadDefinition CreateSingleType(Func<object?[], object?> func, Type returnType, Type argumentType)
        {
            return new SingleTypeOverloadDefinition(func, returnType, argumentType, 0, int.MaxValue);
        }
        public static OverloadDefinition CreateSingleType(Func<object?[], object?> func, Type returnType, int minimumArguments, int maximumArguments)
        {
            return new SingleTypeOverloadDefinition(func, returnType, typeof(object), minimumArguments, maximumArguments);
        }
        public static OverloadDefinition CreateSingleType(Func<object?[], object?> func, Type returnType, int minimumArguments)
        {
            return new SingleTypeOverloadDefinition(func, returnType, typeof(object), minimumArguments, int.MaxValue);
        }
        public static OverloadDefinition CreateSingleType(Func<object?[], object?> func, Type returnType)
        {
            return new SingleTypeOverloadDefinition(func, returnType, typeof(object), 0, int.MaxValue);
        }


        public static OverloadDefinition CreateSingleTypeVoid(Action<object?[]> func, int minimumArguments, int maximumArguments, Type argumentType)
        {
            return new SingleTypeOverloadDefinition(func, typeof(void), argumentType, minimumArguments, maximumArguments);
        }
        public static OverloadDefinition CreateSingleTypeVoid(Action<object?[]> func, int minimumArguments, Type argumentType)
        {
            return new SingleTypeOverloadDefinition(func, typeof(void), argumentType, minimumArguments, int.MaxValue);
        }
        public static OverloadDefinition CreateSingleTypeVoid(Action<object?[]> func, Type argumentType)
        {
            return new SingleTypeOverloadDefinition(func, typeof(void), argumentType, 0, int.MaxValue);
        }
        public static OverloadDefinition CreateSingleTypeVoid(Action<object?[]> func, int minimumArguments, int maximumArguments)
        {
            return new SingleTypeOverloadDefinition(func, typeof(void), typeof(object), minimumArguments, maximumArguments);
        }
        public static OverloadDefinition CreateSingleTypeVoid(Action<object?[]> func, int minimumArguments)
        {
            return new SingleTypeOverloadDefinition(func, typeof(void), typeof(object), minimumArguments, int.MaxValue);
        }
        public static OverloadDefinition CreateSingleTypeVoid(Action<object?[]> func)
        {
            return new SingleTypeOverloadDefinition(func, typeof(void), typeof(object), 0, int.MaxValue);
        }


        public static OverloadDefinition CreateMultiType(Func<object?[], object?> func, Type returnType, params Type[] argumentTypes)
        {
            return new MultiTypeOverloadDefinition(func, returnType, argumentTypes);
        }
        public static OverloadDefinition CreateMultiTypeVoid(Action<object?[]> func, params Type[] argumentTypes)
        {
            return new MultiTypeOverloadDefinition(func, typeof(void), argumentTypes);
        }
    }
}
