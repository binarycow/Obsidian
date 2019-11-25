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
    }
}
