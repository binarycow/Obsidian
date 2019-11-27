using ExpressionParser.Scopes;
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
            ScopedFunction = default;
            ScopedAction = default;
            Function = func;
            Action = default;
        }
        public OverloadDefinition(Action<object?[]>? action, Type returnType)
        {
            ReturnType = returnType;
            ScopedFunction = default;
            ScopedAction = default;
            Function = default;
            Action = action;
        }
        public OverloadDefinition(Func<IScope, object?[], object?> func, Type returnType)
        {
            ReturnType = returnType;
            ScopedFunction = func;
            ScopedAction = default;
            Function = default;
            Action = default;
        }
        public OverloadDefinition(Action<IScope, object?[]>? action, Type returnType)
        {
            ReturnType = returnType;
            ScopedFunction = default;
            ScopedAction = action;
            Function = default;
            Action = default;
        }

        public Func<object?[], object?>? Function { get; }
        public Action<object?[]>? Action { get; }
        public Func<IScope, object?[], object?>? ScopedFunction { get; }
        public Action<IScope, object?[]>? ScopedAction { get; }
        public Type ReturnType { get; set; }


        public object? Invoke(IScope scope, object?[] args)
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




        public static OverloadDefinition CreateEmpty(Func<IScope, object?> func, Type returnType)
        {
            return new SingleTypeOverloadDefinition((scope, args) => func(scope), returnType, typeof(void), 0, 0);
        }
        public static OverloadDefinition CreateEmptyVoid(Action<IScope> func)
        {
            return new SingleTypeOverloadDefinition((scope, args) => func(scope), typeof(void), typeof(void), 0, 0);
        }

        public static OverloadDefinition CreateSingleType(Func<IScope, object?[], object?> func, Type returnType, int minimumArguments, int maximumArguments, Type argumentType)
        {
            return new SingleTypeOverloadDefinition(func, returnType, argumentType, minimumArguments, maximumArguments);
        }
        public static OverloadDefinition CreateSingleType(Func<IScope, object?[], object?> func, Type returnType, int minimumArguments, Type argumentType)
        {
            return new SingleTypeOverloadDefinition(func, returnType, argumentType, minimumArguments, int.MaxValue);
        }
        public static OverloadDefinition CreateSingleType(Func<IScope, object?[], object?> func, Type returnType, Type argumentType)
        {
            return new SingleTypeOverloadDefinition(func, returnType, argumentType, 0, int.MaxValue);
        }
        public static OverloadDefinition CreateSingleType(Func<IScope, object?[], object?> func, Type returnType, int minimumArguments, int maximumArguments)
        {
            return new SingleTypeOverloadDefinition(func, returnType, typeof(object), minimumArguments, maximumArguments);
        }
        public static OverloadDefinition CreateSingleType(Func<IScope, object?[], object?> func, Type returnType, int minimumArguments)
        {
            return new SingleTypeOverloadDefinition(func, returnType, typeof(object), minimumArguments, int.MaxValue);
        }
        public static OverloadDefinition CreateSingleType(Func<IScope, object?[], object?> func, Type returnType)
        {
            return new SingleTypeOverloadDefinition(func, returnType, typeof(object), 0, int.MaxValue);
        }


        public static OverloadDefinition CreateSingleTypeVoid(Action<IScope, object?[]> func, int minimumArguments, int maximumArguments, Type argumentType)
        {
            return new SingleTypeOverloadDefinition(func, typeof(void), argumentType, minimumArguments, maximumArguments);
        }
        public static OverloadDefinition CreateSingleTypeVoid(Action<IScope, object?[]> func, int minimumArguments, Type argumentType)
        {
            return new SingleTypeOverloadDefinition(func, typeof(void), argumentType, minimumArguments, int.MaxValue);
        }
        public static OverloadDefinition CreateSingleTypeVoid(Action<IScope, object?[]> func, Type argumentType)
        {
            return new SingleTypeOverloadDefinition(func, typeof(void), argumentType, 0, int.MaxValue);
        }
        public static OverloadDefinition CreateSingleTypeVoid(Action<IScope, object?[]> func, int minimumArguments, int maximumArguments)
        {
            return new SingleTypeOverloadDefinition(func, typeof(void), typeof(object), minimumArguments, maximumArguments);
        }
        public static OverloadDefinition CreateSingleTypeVoid(Action<IScope, object?[]> func, int minimumArguments)
        {
            return new SingleTypeOverloadDefinition(func, typeof(void), typeof(object), minimumArguments, int.MaxValue);
        }
        public static OverloadDefinition CreateSingleTypeVoid(Action<IScope, object?[]> func)
        {
            return new SingleTypeOverloadDefinition(func, typeof(void), typeof(object), 0, int.MaxValue);
        }


        public static OverloadDefinition CreateMultiType(Func<IScope, object?[], object?> func, Type returnType, params Type[] argumentTypes)
        {
            return new MultiTypeOverloadDefinition(func, returnType, argumentTypes);
        }
        public static OverloadDefinition CreateMultiTypeVoid(Action<IScope, object?[]> func, params Type[] argumentTypes)
        {
            return new MultiTypeOverloadDefinition(func, typeof(void), argumentTypes);
        }


    }
}
