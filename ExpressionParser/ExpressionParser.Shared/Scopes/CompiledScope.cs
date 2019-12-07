using Common;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ExpressionParser.Scopes
{
    public class CompiledScope : IScope
    {
        private CompiledScope(string? name, CompiledScope? parentScope)
        {
            Name = name;
            ParentScopeCompiled = parentScope;
        }
        public IScope? ParentScope => ParentScopeCompiled;
        public CompiledScope? ParentScopeCompiled { get; }
        public string? Name { get; }

        private readonly Dictionary<string, ParameterExpression> _Variables = new Dictionary<string, ParameterExpression>();

        public IEnumerable<ParameterExpression> Variables => _Variables.Values.ToArrayWithoutInstantiation();

        public IEnumerable<ParameterExpression> VariableWalk()
        {
            return Variables.Concat(ParentScopeCompiled?.VariableWalk() ?? Enumerable.Empty<ParameterExpression>());
        }

        public bool IsRootScope => ParentScope == default;

        public ParameterExpression this[string name]
        {
            get
            {
                if (TryGetVariable(name, out var parameter)) return parameter;
                throw new KeyNotFoundException($"Variable of name {name} not defined.");
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
            if (ParentScopeCompiled?.TryGetVariable(name, out variable) == true) return true;
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

        public IScope CreateChild(string name) => CreateCompiledChild(name);
        public CompiledScope CreateCompiledChild(string name)
        {
            return new CompiledScope(name, this);
        }

        public IScope CreateChild()
        {
            return new CompiledScope(null, this);
        }

        public IScope? FindScope(string name)
        {
            if (Name == name) return this;
            return ParentScope?.FindScope(name);
        }

        public IScope FindRootScope()
        {
            return ParentScope == null ? this : ParentScope.FindRootScope();
        }


        public Expression ToDictionary()
        {
            return ExpressionEx.CreateDictionary<string, object?>(VariableWalk().ToArray());
        }



        public static CompiledScope CreateRootScope(string name, IDictionary<string, object?> parameters)
        {
            var scope = new CompiledScope(name, null);
            foreach (var key in parameters.Keys)
            {
                scope.DefineVariable(key, parameters[key]?.GetType() ?? typeof(object));
            }
            return scope;
        }

        internal static CompiledScope CreateDerivedRootScope(string name, params CompiledScope?[] scopes)
        {
            var newScope = new CompiledScope(name, null);
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
