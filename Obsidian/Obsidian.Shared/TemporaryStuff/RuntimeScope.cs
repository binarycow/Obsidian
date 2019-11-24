using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obsidian.TemporaryStuff
{
    public class RuntimeScope
    {
        private RuntimeScope(RuntimeScope? parentScope)
        {
            ParentScope = parentScope;
        }

        public IDictionary<string, object?> Variables { get; } = new Dictionary<string, object?>();

        public RuntimeScope? ParentScope { get; } = null;

        internal RuntimeScope CreateChild()
        {
            var newScope = new RuntimeScope(this);
            foreach (var name in Variables.Keys)
                newScope.AddLocalVariable(name, Variables[name]);
            return newScope;
        }

        internal void AddLocalVariable(string name, object? value)
        {
            Variables.Add(name, value);
        }

        public static RuntimeScope CreateRoot(IDictionary<string, object?> variables)
        {
            var scope = new RuntimeScope(null);
            foreach (var key in variables.Keys)
            {
                scope.AddLocalVariable(key, variables[key]);
            }
            return scope;
        }
    }
}
