using ExpressionParser.Scopes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser.Configuration
{
    internal abstract class OverloadDefinition
    {
        internal OverloadDefinition(Func<object?[], object?> func, Type returnType)
        {
            ReturnType = returnType;
            ScopedFunction = default;
            ScopedAction = default;
            Function = func;
            Action = default;
        }
        internal OverloadDefinition(Action<object?[]>? action, Type returnType)
        {
            ReturnType = returnType;
            ScopedFunction = default;
            ScopedAction = default;
            Function = default;
            Action = action;
        }
        internal OverloadDefinition(Func<IScope, object?[], object?> func, Type returnType)
        {
            ReturnType = returnType;
            ScopedFunction = func;
            ScopedAction = default;
            Function = default;
            Action = default;
        }
        internal OverloadDefinition(Action<IScope, object?[]>? action, Type returnType)
        {
            ReturnType = returnType;
            ScopedFunction = default;
            ScopedAction = action;
            Function = default;
            Action = default;
        }

        internal Func<object?[], object?>? Function { get; }
        internal Action<object?[]>? Action { get; }
        internal Func<IScope, object?[], object?>? ScopedFunction { get; }
        internal Action<IScope, object?[]>? ScopedAction { get; }
        internal Type ReturnType { get; set; }


        internal object? Invoke(IScope scope, object?[] args)
        {
            if (Function != null) return Function(args);
            if (ScopedFunction != null) return ScopedFunction(scope, args);
            if(Action != null)
            {
                Action(args);
                return Void.Instance;
            }
            if(ScopedAction != null)
            {
                ScopedAction(scope, args);
                return Void.Instance;
            }
            throw new NotImplementedException();
        }

        internal static OverloadDefinition CreateEmpty(Func<object?> func, Type returnType)
        {
            return new SingleTypeOverloadDefinition(args => func(), returnType, typeof(void), 0, 0);
        }
        internal static OverloadDefinition CreateEmptyVoid(Action func)
        {
            return new SingleTypeOverloadDefinition(args => func(), typeof(void), typeof(void), 0, 0);
        }


        internal static OverloadDefinition CreateSingleType(Func<object?[], object?> func, Type returnType, int minimumArguments, int maximumArguments, Type argumentType)
        {
            return new SingleTypeOverloadDefinition(func, returnType, argumentType, minimumArguments, maximumArguments);
        }
        internal static OverloadDefinition CreateSingleType(Func<object?[], object?> func, Type returnType, int minimumArguments, Type argumentType)
        {
            return new SingleTypeOverloadDefinition(func, returnType, argumentType, minimumArguments, int.MaxValue);
        }
        internal static OverloadDefinition CreateSingleType(Func<object?[], object?> func, Type returnType, Type argumentType)
        {
            return new SingleTypeOverloadDefinition(func, returnType, argumentType, 0, int.MaxValue);
        }
        internal static OverloadDefinition CreateSingleType(Func<object?[], object?> func, Type returnType, int minimumArguments, int maximumArguments)
        {
            return new SingleTypeOverloadDefinition(func, returnType, typeof(object), minimumArguments, maximumArguments);
        }
        internal static OverloadDefinition CreateSingleType(Func<object?[], object?> func, Type returnType, int minimumArguments)
        {
            return new SingleTypeOverloadDefinition(func, returnType, typeof(object), minimumArguments, int.MaxValue);
        }
        internal static OverloadDefinition CreateSingleType(Func<object?[], object?> func, Type returnType)
        {
            return new SingleTypeOverloadDefinition(func, returnType, typeof(object), 0, int.MaxValue);
        }


        internal static OverloadDefinition CreateSingleTypeVoid(Action<object?[]> func, int minimumArguments, int maximumArguments, Type argumentType)
        {
            return new SingleTypeOverloadDefinition(func, typeof(void), argumentType, minimumArguments, maximumArguments);
        }
        internal static OverloadDefinition CreateSingleTypeVoid(Action<object?[]> func, int minimumArguments, Type argumentType)
        {
            return new SingleTypeOverloadDefinition(func, typeof(void), argumentType, minimumArguments, int.MaxValue);
        }
        internal static OverloadDefinition CreateSingleTypeVoid(Action<object?[]> func, Type argumentType)
        {
            return new SingleTypeOverloadDefinition(func, typeof(void), argumentType, 0, int.MaxValue);
        }
        internal static OverloadDefinition CreateSingleTypeVoid(Action<object?[]> func, int minimumArguments, int maximumArguments)
        {
            return new SingleTypeOverloadDefinition(func, typeof(void), typeof(object), minimumArguments, maximumArguments);
        }
        internal static OverloadDefinition CreateSingleTypeVoid(Action<object?[]> func, int minimumArguments)
        {
            return new SingleTypeOverloadDefinition(func, typeof(void), typeof(object), minimumArguments, int.MaxValue);
        }
        internal static OverloadDefinition CreateSingleTypeVoid(Action<object?[]> func)
        {
            return new SingleTypeOverloadDefinition(func, typeof(void), typeof(object), 0, int.MaxValue);
        }


        internal static OverloadDefinition CreateMultiType(Func<object?[], object?> func, Type returnType, params Type[] argumentTypes)
        {
            return new MultiTypeOverloadDefinition(func, returnType, argumentTypes);
        }
        internal static OverloadDefinition CreateMultiTypeVoid(Action<object?[]> func, params Type[] argumentTypes)
        {
            return new MultiTypeOverloadDefinition(func, typeof(void), argumentTypes);
        }




        internal static OverloadDefinition CreateEmpty(Func<IScope, object?> func, Type returnType)
        {
            return new SingleTypeOverloadDefinition((scope, args) => func(scope), returnType, typeof(void), 0, 0);
        }
        internal static OverloadDefinition CreateEmptyVoid(Action<IScope> func)
        {
            return new SingleTypeOverloadDefinition((scope, args) => func(scope), typeof(void), typeof(void), 0, 0);
        }

        internal static OverloadDefinition CreateSingleType(Func<IScope, object?[], object?> func, Type returnType, int minimumArguments, int maximumArguments, Type argumentType)
        {
            return new SingleTypeOverloadDefinition(func, returnType, argumentType, minimumArguments, maximumArguments);
        }
        internal static OverloadDefinition CreateSingleType(Func<IScope, object?[], object?> func, Type returnType, int minimumArguments, Type argumentType)
        {
            return new SingleTypeOverloadDefinition(func, returnType, argumentType, minimumArguments, int.MaxValue);
        }
        internal static OverloadDefinition CreateSingleType(Func<IScope, object?[], object?> func, Type returnType, Type argumentType)
        {
            return new SingleTypeOverloadDefinition(func, returnType, argumentType, 0, int.MaxValue);
        }
        internal static OverloadDefinition CreateSingleType(Func<IScope, object?[], object?> func, Type returnType, int minimumArguments, int maximumArguments)
        {
            return new SingleTypeOverloadDefinition(func, returnType, typeof(object), minimumArguments, maximumArguments);
        }
        internal static OverloadDefinition CreateSingleType(Func<IScope, object?[], object?> func, Type returnType, int minimumArguments)
        {
            return new SingleTypeOverloadDefinition(func, returnType, typeof(object), minimumArguments, int.MaxValue);
        }
        internal static OverloadDefinition CreateSingleType(Func<IScope, object?[], object?> func, Type returnType)
        {
            return new SingleTypeOverloadDefinition(func, returnType, typeof(object), 0, int.MaxValue);
        }


        internal static OverloadDefinition CreateSingleTypeVoid(Action<IScope, object?[]> func, int minimumArguments, int maximumArguments, Type argumentType)
        {
            return new SingleTypeOverloadDefinition(func, typeof(void), argumentType, minimumArguments, maximumArguments);
        }
        internal static OverloadDefinition CreateSingleTypeVoid(Action<IScope, object?[]> func, int minimumArguments, Type argumentType)
        {
            return new SingleTypeOverloadDefinition(func, typeof(void), argumentType, minimumArguments, int.MaxValue);
        }
        internal static OverloadDefinition CreateSingleTypeVoid(Action<IScope, object?[]> func, Type argumentType)
        {
            return new SingleTypeOverloadDefinition(func, typeof(void), argumentType, 0, int.MaxValue);
        }
        internal static OverloadDefinition CreateSingleTypeVoid(Action<IScope, object?[]> func, int minimumArguments, int maximumArguments)
        {
            return new SingleTypeOverloadDefinition(func, typeof(void), typeof(object), minimumArguments, maximumArguments);
        }
        internal static OverloadDefinition CreateSingleTypeVoid(Action<IScope, object?[]> func, int minimumArguments)
        {
            return new SingleTypeOverloadDefinition(func, typeof(void), typeof(object), minimumArguments, int.MaxValue);
        }
        internal static OverloadDefinition CreateSingleTypeVoid(Action<IScope, object?[]> func)
        {
            return new SingleTypeOverloadDefinition(func, typeof(void), typeof(object), 0, int.MaxValue);
        }


        internal static OverloadDefinition CreateMultiType(Func<IScope, object?[], object?> func, Type returnType, params Type[] argumentTypes)
        {
            return new MultiTypeOverloadDefinition(func, returnType, argumentTypes);
        }
        internal static OverloadDefinition CreateMultiTypeVoid(Action<IScope, object?[]> func, params Type[] argumentTypes)
        {
            return new MultiTypeOverloadDefinition(func, typeof(void), argumentTypes);
        }


    }
}
