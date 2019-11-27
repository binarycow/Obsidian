using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text;

namespace ExpressionParser.Scopes
{
    public class DynamicScope : IDynamicScope
    {
        public DynamicScope(string name, IDynamicScope parent)
        {
            Name = name;
            ParentScope = parent;
        }

        public IDynamicScope? ParentScope { get; }

        public string? Name { get; }

        public bool IsRootScope => ParentScope == null;


        private Dictionary<string, object?> _Variables = new Dictionary<string, object?>();



        public static DynamicScope CreateRootScope(string? name, IDictionary<string, object?> variables)
        {
            var scope = new DynamicScope(name, null);
            foreach(var key in variables.Keys)
            {
                scope.DefineAndSetVariable(key, variables[key]);
            }
            return scope;
        }

        public IDynamicScope? FindScope(string name)
        {
            throw new NotImplementedException();
        }

        public IDynamicScope FindRootScope()
        {
            throw new NotImplementedException();
        }

        public IDynamicScope CreateChild(string name)
        {
            throw new NotImplementedException();
        }

        public IDynamicScope CreateChild()
        {
            throw new NotImplementedException();
        }

        public void DefineAndSetVariable(string name, object? valueToSet)
        {
            _Variables.Add(name, valueToSet);
        }

        public bool TryGetVariable(string name, out object? value)
        {
            return _Variables.TryGetValue(name, out value);
        }
    }
}
