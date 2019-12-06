using ExpressionParser.Scopes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressionParser
{
    public class UserDefinedFunction
    {
        public delegate object? UserDefinedFunctionDelegate(UserDefinedArgumentData args);


        public UserDefinedFunction(FunctionDeclaration declaration, UserDefinedFunctionDelegate body)
        {
            Declaration = declaration;
            Body = body;
        }
        public FunctionDeclaration Declaration { get; }
        public UserDefinedFunctionDelegate Body { get; }


        internal object? Invoke(object?[] args)
        {
            return Invoke(UserDefinedArgumentData.Create(Declaration.Arguments, args));
        }
        internal object? Invoke(object? pipelineObject, object?[] args)
        {
            return Invoke(UserDefinedArgumentData.Create(Declaration.Arguments, pipelineObject.YieldOne().Concat(args).ToArray()));
        }

        protected virtual object? Invoke(UserDefinedArgumentData argumentData)
        {
            return Body.Invoke(argumentData);
        }
    }
}
