using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text;

namespace ExpressionParser.Scopes
{
    internal class DynamicScope : IScope
    {
        internal DynamicScope(string name, DynamicScope parent)
        {
            Name = name;
            DynamicParent = parent;
        }

        protected DynamicScope(DynamicScope parent)
        {
            Name = null;
            DynamicParent = parent;
        }
        protected DynamicScope(string? name)
        {
            Name = name;
            DynamicParent = null;
        }

        public IScope? ParentScope => DynamicParent;
        internal DynamicScope? DynamicParent { get; }

        public string? Name { get; }

        public bool IsRootScope => ParentScope == null;


        private readonly Dictionary<string, object?> _Variables = new Dictionary<string, object?>();



        internal static DynamicScope CreateRootScope(string? name, IDictionary<string, object?> variables)
        {
            var scope = new DynamicScope(name);
            foreach(var key in variables.Keys)
            {
                scope.DefineAndSetVariable(key, variables[key]);
            }
            return scope;
        }

        public IScope? FindScope(string name)
        {
            throw new NotImplementedException();
        }

        public IScope FindRootScope()
        {
            throw new NotImplementedException();
        }

        public virtual IScope CreateChild(string name)
        {
            throw new NotImplementedException();
        }

        public virtual IScope CreateChild()
        {
            throw new NotImplementedException();
        }

        internal void DefineAndSetVariable(string name, object? valueToSet)
        {
            _Variables.Upsert(name, valueToSet);
        }

        internal bool TryGetVariable(string name, out object? value)
        {
            if (_Variables.TryGetValue(name, out value)) return true;
            if (DynamicParent == null) return false;
            return DynamicParent.TryGetVariable(name, out value);
        }
        internal bool TryGetVariable<T>(string name, out T? value) where T : class
        {
            value = default;
            if(TryGetVariable(name, out var objVariableValue))
            {
                value = objVariableValue as T;
            }
            return value != default;
        }
    }
}
