using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressionParser
{
    public class UserDefinedArgumentData
    {
        private UserDefinedArgumentData(IEnumerable<UserDefinedArgument> definedPositionalArguments, IEnumerable<UserDefinedArgument> additionalPositionalArguments, IEnumerable<UserDefinedArgument> additionalKeywordArguments)
        {
            DefinedPositionalArguments = definedPositionalArguments.ToArrayWithoutInstantiation();
            AdditionalPositionalArguments = additionalPositionalArguments.ToArrayWithoutInstantiation();
            AdditionalKeywordArguments = additionalKeywordArguments.ToArrayWithoutInstantiation();
        }

        public UserDefinedArgument[] DefinedPositionalArguments { get; }
        public UserDefinedArgument[] AdditionalPositionalArguments { get; }
        public UserDefinedArgument[] AdditionalKeywordArguments { get; }


        public IEnumerable<UserDefinedArgument> AllArguments => DefinedPositionalArguments.Concat(AdditionalPositionalArguments).Concat(AdditionalKeywordArguments);


        internal static UserDefinedArgumentData Create(ParameterDeclaration[] declaration, object?[] passedValues)
        {
            var additionalPositionalArguments = new List<UserDefinedArgument>();
            var additionalKeywordArguments = new Dictionary<string, UserDefinedArgument>();
            var declaredArguments = new UserDefinedArgument?[declaration.Length];
            var initialPositionalArguments = passedValues.TakeWhile(arg => !(arg is ValueTuple<string, object?>)).ToArray();


            for (var argIndex = initialPositionalArguments.Length; argIndex < passedValues.Length; ++argIndex)
            {
                var arg = passedValues[argIndex];
                if (arg is ValueTuple<string, object?> tuple)
                {
                    additionalKeywordArguments.Add(tuple.Item1, new UserDefinedArgument(tuple.Item1, tuple.Item2, argIndex, true));
                }
                else
                {
                    additionalPositionalArguments.Add(new UserDefinedArgument(string.Empty, arg, argIndex, true));
                }
            }

            for(var argIndex = 0; argIndex < Math.Min(declaration.Length, initialPositionalArguments.Length); ++argIndex)
            {
                var dec = declaration[argIndex];
                var prov = initialPositionalArguments[argIndex];
                declaredArguments[argIndex] = new UserDefinedArgument(dec.Name, prov, argIndex, true);
            }

            for(var argIndex = 0; argIndex < declaredArguments.Length; ++argIndex)
            {
                var dec = declaration[argIndex];
                if (declaredArguments[argIndex] != null) continue;
                if(additionalKeywordArguments.TryGetValue(dec.Name, out var providedKeywordArg))
                {
                    declaredArguments[argIndex] = providedKeywordArg;
                    additionalKeywordArguments.Remove(declaration[argIndex].Name);
                    continue;
                }
                if(dec.Optional)
                {
                    declaredArguments[argIndex] = new UserDefinedArgument(dec.Name, dec.DefaultValue, argIndex, false);
                    continue;
                }
                throw new NotImplementedException(); //Argument not provided
            }

            var nonNullDeclaredArguments = declaredArguments.Where(arg => arg != null).OfType<UserDefinedArgument>().ToArray();

            return new UserDefinedArgumentData(
                nonNullDeclaredArguments,
                additionalPositionalArguments,
                additionalKeywordArguments.Values
            );

        }
    }
}
