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


        internal object? Invoke(object?[] args)
        {
            return Invoke(UserDefinedArgumentData.Create(Declaration.Arguments, args));
        }

        protected virtual object? Invoke(UserDefinedArgumentData argumentData)
        {
            return Body.Invoke(argumentData);
        }
    }
}
