using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Common.ExpressionCreators;
using ExpressionParser.VariableManagement;

namespace ExpressionParser.Scopes
{
    public class Scope
    {
        protected Scope(Scope? parentScope, string? name = null)
        {
            ParentScope = parentScope;
            Name = name;
        }



        private Scope? ParentScope { get; }
        private Dictionary<string, Expression> _LocalVariables = new Dictionary<string, Expression>();

        public IEnumerable<Expression> LocalVariables => _LocalVariables.Values;

        public BlockExpression CloseScope(IEnumerable<Expression> body)
        {
            return Expression.Block(LocalVariables.OfType<ParameterExpression>(), body.ToArray());
        }


        public virtual bool IsRoot => false;

        public string? Name { get; }



        public ExpressionExtensionData<TData> AddAndCreateLocalVariable_Generic<TData>(string name, out Expression assignExpression,
            Type[] typeArguments,
            params Expression[] constructorArguments)
        {
            var variable = Expression.Variable(typeof(TData), name);
            _LocalVariables.Add(name, variable);
            if (ExpressionExtensionData.TryCreate_Generic<TData>(variable, out var expressionExtensionData,
                out var newExpression, typeArguments, constructorArguments) == false || expressionExtensionData == null)
            {
                throw new NotImplementedException(); // Couldn't create variable
            }
            assignExpression = Expression.Assign(variable, newExpression);
            return expressionExtensionData;
        }
        public ExpressionExtensionData<TData> AddAndCreateLocalVariable<TData>(string name, out Expression assignExpression, 
            params Expression[] constructorArguments)
        {
            return AddAndCreateLocalVariable_Generic<TData>(name, out assignExpression, Type.EmptyTypes, constructorArguments);
        }

        public ParameterExpression AddLocalVariable(string name, Expression assignValue, out Expression assignExpression)
        {
            var variable = Expression.Variable(assignValue.Type, name);
            _LocalVariables.Add(name, variable);
            assignExpression = Expression.Assign(variable, assignValue);
            return variable;
        }




        public virtual bool TryGetVariable(string variableName, [NotNullWhen(true)]out Expression? expression)
        {
            if (_LocalVariables.TryGetValue(variableName, out expression))
            {
                return true;
            }

            return ParentScope != null ? 
                ParentScope.TryGetVariable(variableName, out expression) : 
                false;
        }

        public virtual Scope FindRootScope()
        {
            return ParentScope?.FindRootScope() ?? this;
        }

        public Scope CreateChild(string? name = null)
        {
            return new Scope(this, name);
        }

        public IDictionary<string, Expression> VariableWalk()
        {
            return VariableWalk(new Dictionary<string, Expression>());
        }
        protected virtual IDictionary<string, Expression> VariableWalk(IDictionary<string, Expression> dict)
        {
            foreach (var key in _LocalVariables.Keys)
            {
                if (dict.ContainsKey(key) == false)
                {
                    dict.Add(key, _LocalVariables[key]);
                }
            }
            return ParentScope?.VariableWalk(dict) ?? dict;
        }
    }
}
