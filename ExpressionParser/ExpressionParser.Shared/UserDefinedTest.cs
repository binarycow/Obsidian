using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExpressionParser.Configuration;

namespace ExpressionParser
{
    public class UserDefinedTest
    {
        public delegate bool UserDefinedTestDelegate(UserDefinedArgumentData args);


        public UserDefinedTest(FunctionDeclaration<bool> declaration, UserDefinedTestDelegate body)
        {
            Declaration = declaration;
            Body = body;
        }
        public FunctionDeclaration<bool> Declaration { get; }
        public UserDefinedTestDelegate Body { get; }


        internal bool Invoke(ILanguageDefinition languageDefinition, object?[] args)
        {
            return Invoke(UserDefinedArgumentData.Create(languageDefinition, Declaration.Arguments.ToArrayWithoutInstantiation(), args));
        }
        internal bool Invoke(ILanguageDefinition languageDefinition, object? pipelineObject, object?[] args)
        {
            return Invoke(UserDefinedArgumentData.Create(languageDefinition, Declaration.Arguments.ToArrayWithoutInstantiation(), pipelineObject.YieldOne().Concat(args).ToArray()));
        }

        protected virtual bool Invoke(UserDefinedArgumentData argumentData)
        {
            return Body.Invoke(argumentData);
        }
    }
}
