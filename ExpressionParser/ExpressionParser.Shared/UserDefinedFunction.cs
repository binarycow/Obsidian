using ExpressionParser.Configuration;
using ExpressionParser.Scopes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static ExpressionParser.UserDefinedFunction;

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


        internal object? Invoke(ILanguageDefinition languageDefinition, object?[] args)
        {
            return Invoke(UserDefinedArgumentData.Create(languageDefinition, Declaration.Arguments.ToArrayWithoutInstantiation(), args));
        }
        internal object? Invoke(ILanguageDefinition languageDefinition, object? pipelineObject, object?[] args)
        {
            return Invoke(UserDefinedArgumentData.Create(languageDefinition, Declaration.Arguments.ToArrayWithoutInstantiation(), pipelineObject.YieldOne().Concat(args).ToArray()));
        }

        protected virtual object? Invoke(UserDefinedArgumentData argumentData)
        {
            return Body.Invoke(argumentData);
        }
    }
}
