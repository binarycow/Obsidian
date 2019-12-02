using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressionParser.Configuration
{
    public static class ILanguageDefinitionExtensions
    {
        public static void Validate(this ILanguageDefinition languageDefinition)
        {
            if(languageDefinition.Functions.Any(func => func.OverloadDefinitions.Length != 1))
            {
                throw new NotImplementedException();
            }
        }

        public static IEnumerable<(FunctionDefinition function, OverloadDefinition overload)> PipelineFunctions(this ILanguageDefinition languageDefinition)
        {
            languageDefinition = languageDefinition ?? throw new ArgumentNullException(nameof(languageDefinition));
            return languageDefinition.Functions.SelectMany(GetPipelineOverloads);

            IEnumerable<(FunctionDefinition function, OverloadDefinition overload)> GetPipelineOverloads(FunctionDefinition function)
            {
                foreach(var overload in function.OverloadDefinitions)
                {
                    if (overload.ReturnType == typeof(void)) continue;
                    switch(overload)
                    {
                        case SingleTypeOverloadDefinition singleType:
                            if (singleType.ArgumentType != typeof(object)) continue;
                            if (1 < singleType.MinimumArguments) continue;
                            if (1 > singleType.MaximumArguments) continue;
                            yield return (function, overload);
                            continue;
                    }
                }
            }
        }
    }
}
