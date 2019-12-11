using System;
using System.Collections.Generic;
using System.Text;
using ExpressionParser.Configuration;
using ExpressionParser.Scopes;
using static ExpressionParser.ScopedUserDefinedFunction;

namespace ExpressionParser
{
#pragma warning disable CA1812 // Avoid uninstantiated internal classes
    internal class ScopedUserDefinedFunction
#pragma warning restore CA1812 // Avoid uninstantiated internal classes
    {
        public delegate object? ScopedUserDefinedFunctionDelegate(IScope scope, UserDefinedArgumentData args);

        public ScopedUserDefinedFunction(FunctionDeclaration declaration, ScopedUserDefinedFunctionDelegate body)
        {
            Body = body;
            Declaration = declaration;
        }


        public FunctionDeclaration Declaration { get; }

        public ScopedUserDefinedFunctionDelegate Body { get; }

        internal object? Invoke(ILanguageDefinition languageDefinition, IScope scope, object?[] args)
        {
            return Invoke(scope, UserDefinedArgumentData.Create(languageDefinition, Declaration.Arguments.ToArrayWithoutInstantiation(), args));
        }
        protected virtual object? Invoke(IScope scope, UserDefinedArgumentData argumentData)
        {
            return Body.Invoke(scope, argumentData);
        }
    }
}
