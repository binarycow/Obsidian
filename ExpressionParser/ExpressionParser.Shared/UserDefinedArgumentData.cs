using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser
{
    public class UserDefinedArgumentData
    {
        public UserDefinedArgumentData((int index, string name, object? value)[] positionalArguments, (string name, object? value)[] keywordArguments)
        {
            PositionalArguments = positionalArguments;
            KeywordArguments = keywordArguments;
        }

        public (int index, string name, object? value)[] PositionalArguments { get; }
        public (string name, object? value)[] KeywordArguments { get; }



        public static UserDefinedArgumentData Create(ArgumentDeclaration[] declaration, object?[] passedValues)
        {
            declaration = declaration ?? throw new ArgumentNullException(nameof(declaration));
            passedValues = passedValues ?? throw new ArgumentNullException(nameof(passedValues));
            var positional = new List<(int index, string name, object? value)>();
            var keyword = new List<(string name, object? value)>();

            for(var i = 0; i < declaration.Length; ++i)
            {
                var value = i < passedValues.Length ? passedValues[i] : declaration[i].DefaultValue;
                positional.Add((index: i, name: declaration[i].Name, value: value));
            }
            return new UserDefinedArgumentData(positional.ToArray(), keyword.ToArray());
        }
    }
}
