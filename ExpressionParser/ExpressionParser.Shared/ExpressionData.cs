using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Common;
using ExpressionParser.Scopes;

namespace ExpressionParser
{
    internal class ExpressionData
    {
        internal class ParameterInfo
        {
            internal ParameterInfo(string name, Type type)
            {
                Name = name;
                Type = type;
            }
            internal string Name { get; }
            internal Type Type { get; }
        }

        private ExpressionData(Type returnType, ParameterInfo[] parameters, Expression expressionTree, Delegate @delegate, bool compiled)
        {
            ReturnType = returnType;
            Parameters = parameters;
            ExpressionTree = expressionTree;
            Delegate = @delegate;
            Compiled = compiled;
        }
        internal Type ReturnType { get; }
        internal ParameterInfo[] Parameters { get; }
        internal Expression ExpressionTree { get; }
        internal Delegate Delegate { get; }
        internal bool Compiled { get; }

        internal Type[] ParameterTypes => Parameters.Select(param => param.Type).ToArray();

        internal object? Evaluate(IDictionary<string, object?> variables)
        {
            var typedArguments = Parameters.Select(param =>
            {
                if (variables.TryGetValue(param.Name, out var paramValue) == false) throw new NotImplementedException();
                if (TypeCoercion.CanCast(paramValue?.GetType() ?? typeof(object), param.Type) == false) throw new NotImplementedException();
                return Convert.ChangeType(paramValue, param.Type, CultureInfo.InvariantCulture);
            }).ToArray();
            var invokeMethod = Delegate.GetType().GetMethod("Invoke", ParameterTypes);

            if(ReturnType == typeof(void))
            {
                invokeMethod.Invoke(Delegate, typedArguments);
                return Void.Instance;
            }
            return invokeMethod.Invoke(Delegate, typedArguments);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        internal T EvaluateAs<T>(IDictionary<string, object?> variables)
        {
            throw new NotImplementedException();
        }

        internal static ExpressionData CreateCompiled(Expression expression, CompiledScope scope)
        {
            return CreateDynamic(expression, scope);
        }

        internal static ExpressionData CreateDynamic(Expression expression, CompiledScope scope)
        {
            var parameterExpressions = scope.Variables.ToArray();
            var parameterInfo = parameterExpressions.Select(param => new ParameterInfo(param.Name, param.Type)).ToArray();
            var parameterTypes = parameterExpressions.Select(param => param.Type).ToArray();

            var funcType = GetDelegateType(expression.Type, parameterTypes, out var genericTypes);

            var lambda = GetLambdaMethod(funcType).Invoke(null, new object[] { expression, parameterExpressions });
            var compileMethod = lambda.GetType().GetMethod("Compile", Type.EmptyTypes);
            var compiled = compileMethod.Invoke(lambda, Array.Empty<object>());
            if (!(compiled is Delegate compiledDelegate))
            {
                throw new NotImplementedException();
            }
            return new ExpressionData(expression.Type, parameterInfo, expression, compiledDelegate, true);
        }

        private static Type GetDelegateType(Type returnType, Type[] parameterTypes, out Type[] genericTypes)
        {
            string openFuncType;
            if(returnType == typeof(void) && parameterTypes.Length == 0)
            {
                genericTypes = Type.EmptyTypes;
                return typeof(Action);
            }
            if(returnType == typeof(void))
            {
                genericTypes = parameterTypes;
                openFuncType = $"System.Action`{genericTypes.Length}";
            }
            else
            {
                genericTypes = parameterTypes.Concat(returnType).ToArray();
                openFuncType = $"System.Func`{genericTypes.Length}";
            }
            return Type.GetType(openFuncType).MakeGenericType(genericTypes);
        }



        private static MethodInfo GetLambdaMethod(Type delegateType)
        {
            var openGenericMethod = typeof(Expression)
                .GetMethod(
                    nameof(Expression.Lambda), 1,
                    new Type[] { typeof(Expression), typeof(ParameterExpression[]) }
                );
            return openGenericMethod.MakeGenericMethod(delegateType);
        }

    }
}
