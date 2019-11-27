using ExpressionParser.Scopes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser
{
    public class ScopeStack<TScope, TRootScope> 
        where TScope : class, IScope
        where TRootScope : class, TScope
    {
        public ScopeStack(TRootScope rootScope)
        {
            Root = rootScope;
            _Stack.Push(rootScope);
        }
        private Stack<TScope> _Stack = new Stack<TScope>();
        public TRootScope Root { get; }
        public TScope Current => _Stack.Peek();


        public void Push(string name)
        {
            var scope = Current.CreateChild(name) as TScope;
            if (scope == null) throw new NotImplementedException();
            _Stack.Push(scope);
        }
        public void Pop(string name)
        {
            var scope = _Stack.Pop();
            if (scope.Name != name) throw new NotImplementedException();
        }
    }
}
