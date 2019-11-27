using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text;

namespace ExpressionParser.Scopes
{
    public interface IScope
    {
        public IScope? ParentScope { get; }
        public string? Name { get; }
        public bool IsRootScope { get; }

        public IScope? FindScope(string name);
        public IScope FindRootScope();

        public IScope CreateChild(string name);
        public IScope CreateChild();


    }
}
