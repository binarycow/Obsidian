using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ExpressionParser.Scopes;
using ExpressionParser.VariableManagement;
using ExpressionToString;

namespace ExpressionParser
{
    public class ExpressionData
    {
        private ExpressionData(Expression expression, Func<object?[], object?> compiled, IEnumerable<VariableInfo> variables)
        {
            ExpressionTree = expression;
            VariableData = variables.ToArrayWithoutInstantiation();
            _CompiledFunction = compiled;
            _Delegate = null;
        }
        private ExpressionData(Expression expression, Delegate @delegate, IEnumerable<VariableInfo> variables)
        {
            ExpressionTree = expression;
            VariableData = variables.ToArrayWithoutInstantiation();
            _CompiledFunction = null;
            _Delegate = @delegate;
        }
        public Expression ExpressionTree { get; }
        private Func<object?[], object?>? _CompiledFunction;
        private Delegate? _Delegate { get; }
    
        public VariableInfo[] VariableData { get; }

        public bool IsCompiled => _CompiledFunction != null;

        private object?[] GetArgArray(IDictionary<string, object?> variables)
        {
            var argArray = new object?[VariableData.Length];
            for(int i = 0; i < VariableData.Length; ++i)
            {
                if(variables.TryGetValue(VariableData[i].Name, out var objValue))
                {
                    if ((objValue?.GetType() ?? typeof(object)) != VariableData[i].Type) throw new NotImplementedException();
                    argArray[i] = objValue;
                    continue;
                }
                throw new NotImplementedException();
            }
            return argArray;
        }


        public object? Evaluate(IDictionary<string, object?> variables)
        {
            var debug = ExpressionTree.ToString("C#");
            var args = GetArgArray(variables);

            if (_CompiledFunction != null)
            {
                return _CompiledFunction(args);
            }
            if(_Delegate != null)
            {
                return _Delegate.DynamicInvoke((object)args);
            }
            throw new NotImplementedException();
        }
        public T EvaluateAs<T>(IDictionary<string, object?> variables)
        {
            return (T)Convert.ChangeType(Evaluate(variables), typeof(T));
        }

        public static ExpressionData CreateCompiled(Expression expression, Scope scope)
        {
            if(scope is RootScope rootScope)
            {
                return CreateCompiledRoot(expression, rootScope);
            }
            throw new NotImplementedException();
        }

        private static ExpressionData CreateCompiledRoot(Expression expression, RootScope scope)
        {
            var variableInfo = scope.GetVariableInfo();
            var castedExpression = Expression.Convert(expression, typeof(object));
            var lambda = Expression.Lambda<Func<object?[], object?>>(castedExpression, scope.RootParameterExpression);
            var compiled = lambda.Compile();
            return new ExpressionData(expression, compiled, variableInfo);
        }

        public static ExpressionData CreateDynamic(Expression expression, RootScope scope)
        {
            var variableInfo = scope.GetVariableInfo();
            var castedExpression = Expression.Convert(expression, typeof(object));
            var lambda = Expression.Lambda(castedExpression, scope.RootParameterExpression);
            var compiled = lambda.Compile();
            return new ExpressionData(expression, compiled, variableInfo);
        }
    }
}
