using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text;

namespace ExpressionParser.Scopes
{
    public interface IScope<T> where T : class, IScope<T>
    {
        public T? ParentScope { get; }
        public string? Name { get; }
        public bool IsRootScope { get; }

        public T? FindScope(string name);
        public T FindRootScope();

        public T CreateChild(string name);
        public T CreateChild();


    }
}
