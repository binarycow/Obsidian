using ExpressionParser.Scopes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser
{
    public class UserDefinedFunction
    {
        public UserDefinedFunction(FunctionDeclaration declaration, Func<UserDefinedArgumentData, object?> body)
        {
            Declaration = declaration;
            Body = body;
        }
        public FunctionDeclaration Declaration { get; }
        public Func<UserDefinedArgumentData, object?> Body { get; }


        internal object? Invoke<TScope, TRootScope>(ScopeStack<TScope, TRootScope> scopeStack, UserDefinedFunction left, object?[] args)
            where TScope : class, IScope
            where TRootScope : class, TScope
        {
            var argumentData = UserDefinedArgumentData.Create(left.Declaration.Arguments, args);
            return left.Body.Invoke(argumentData);
        }
    }
}
