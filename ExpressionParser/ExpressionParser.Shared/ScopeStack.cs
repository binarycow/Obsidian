using ExpressionParser.Scopes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "<Pending>")]
    internal class ScopeStack<TScope, TRootScope> 
        where TScope : class, IScope
        where TRootScope : class, TScope
    {
        public ScopeStack(TRootScope rootScope)
        {
            Root = rootScope;
            _Stack.Push(rootScope);
        }
        private readonly Stack<TScope> _Stack = new Stack<TScope>();
        public TRootScope Root { get; }
        public TScope Current => _Stack.Peek();


        public void Push(string name)
        {
            if (!(Current.CreateChild(name) is TScope scope)) throw new NotImplementedException();
            _Stack.Push(scope);
        }
        public void Pop(string name)
        {
            var scope = _Stack.Pop();
            if (scope.Name != name) throw new NotImplementedException();
        }
    }
}
