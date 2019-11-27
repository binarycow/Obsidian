using Common;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ExpressionParser.Scopes
{
    public class Scope : ICompiledScope
    {
        private Scope(string? name, ICompiledScope? parentScope)
        {
            Name = name;
            ParentScope = parentScope;
        }
        public ICompiledScope? ParentScope { get; }
        public string? Name { get; }

        private readonly Dictionary<string, ParameterExpression> _Variables = new Dictionary<string, ParameterExpression>();

        public IEnumerable<ParameterExpression> Variables => _Variables.Values.ToArrayWithoutInstantiation();

        public IEnumerable<ParameterExpression> VariableWalk()
        {
            return Variables.Concat(ParentScope?.VariableWalk() ?? Enumerable.Empty<ParameterExpression>());
        }

        public bool IsRootScope => ParentScope == default;

        public ParameterExpression this[string name]
        {
            get
            {
                if (TryGetVariable(name, out var parameter)) return parameter;
                throw new NotImplementedException();
            }
        }


        public void DefineVariable(ParameterExpression variable)
        {
            _Variables.Add(variable.Name, variable);
        }
        public ParameterExpression DefineVariable(string name, Type type)
        {
            var variable = Expression.Variable(type, name);
            _Variables.Add(name, variable);
            return variable;
        }
        public ParameterExpression DefineAndSetVariable(string name, Expression valueToSet, out BinaryExpression assignmentExpression)
        {
            var variable = DefineVariable(name, valueToSet.Type);
            assignmentExpression = Expression.Assign(variable, valueToSet);
            return variable;
        }
        public ParameterExpression DefineAndSetVariable(string name, object? valueToSet, out BinaryExpression assignmentExpression)
        {
            return DefineAndSetVariable(name, Expression.Constant(valueToSet), out assignmentExpression);
        }

        public BinaryExpression DefineAndSetVariable(string name, Expression valueToSet)
        {
            DefineAndSetVariable(name, valueToSet, out var assignmentExpression);
            return assignmentExpression;
        }

        public BinaryExpression DefineAndSetVariable(string name, object? valueToSet)
        {
            DefineAndSetVariable(name, valueToSet, out var assignmentExpression);
            return assignmentExpression;
        }

        public bool TryGetVariable(string name, [NotNullWhen(true)] out ParameterExpression? variable)
        {
            if (_Variables.TryGetValue(name, out variable)) return true;
            if (ParentScope?.TryGetVariable(name, out variable) == true) return true;
            return false;
        }

        public BlockExpression CloseScope(IEnumerable<Expression> body)
        {
            if(IsRootScope)
            {
                throw new NotImplementedException();
            }
            var bodyArray = body.ToArrayWithoutInstantiation();
            return Expression.Block(Variables, bodyArray);
        }

        public ICompiledScope CreateChild(string name)
        {
            return new Scope(name, this);
        }

        public ICompiledScope CreateChild()
        {
            return new Scope(null, this);
        }

        public ICompiledScope? FindScope(string name)
        {
            if (Name == name) return this;
            return ParentScope?.FindScope(name);
        }

        public ICompiledScope FindRootScope()
        {
            return ParentScope == null ? this : ParentScope.FindRootScope();
        }


        public Expression ToDictionary()
        {
            return ExpressionEx.CreateDictionary<string, object?>(VariableWalk().ToArray());
        }



        public static ICompiledScope CreateRootScope(string name, IDictionary<string, object?> parameters)
        {
            var scope = new Scope(name, null);
            foreach (var key in parameters.Keys)
            {
                scope.DefineVariable(key, parameters[key]?.GetType() ?? typeof(object));
            }
            return scope;
        }

        internal static ICompiledScope CreateDerivedRootScope(string name, params ICompiledScope?[] scopes)
        {
            var newScope = new Scope(name, null);
            foreach(var scope in scopes)
            {
                if (scope == null) continue;
                foreach(var parameter in scope.Variables)
                {
                    if(newScope.TryGetVariable(parameter.Name, out _) == false)
                    {
                        newScope.DefineVariable(parameter);
                    }
                }
            }
            return newScope;
        }
    }
}
