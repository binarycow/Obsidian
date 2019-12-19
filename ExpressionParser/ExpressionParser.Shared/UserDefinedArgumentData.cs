using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Common;
using ExpressionParser.Configuration;

namespace ExpressionParser
{
    public class UserDefinedArgumentData
    {
        internal UserDefinedArgumentData(IEnumerable<UserDefinedArgument> definedPositionalArguments, IEnumerable<UserDefinedArgument> additionalPositionalArguments, IEnumerable<UserDefinedArgument> additionalKeywordArguments)
        {
            DefinedPositionalArguments = definedPositionalArguments.ToArrayWithoutInstantiation();
            AdditionalPositionalArguments = additionalPositionalArguments.ToArrayWithoutInstantiation();
            AdditionalKeywordArguments = additionalKeywordArguments.ToArrayWithoutInstantiation();
        }

        public IEnumerable<UserDefinedArgument> DefinedPositionalArguments { get; }
        public IEnumerable<UserDefinedArgument> AdditionalPositionalArguments { get; }
        public IEnumerable<UserDefinedArgument> AdditionalKeywordArguments { get; }


        internal bool TryGetArgumentValue(string argumentName, out object? value)
        {
            value = default;
            var argument = DefinedPositionalArguments.FirstOrDefault(arg => arg.Name == argumentName);
            if (argument != default)
            {
                value = argument.Value;
                return true;
            }
            argument = AdditionalKeywordArguments.FirstOrDefault(arg => arg.Name == argumentName);
            if (argument != default)
            {
                value = argument.Value;
                return true;
            }
            return false;
        }

        internal T GetArgumentValue<T>(string argumentName, T defaultValue = default)
        {
            if(TryGetArgumentValue<T>(argumentName, out var value))
            {
                return value;
            }
            return defaultValue;
        }
        internal bool TryGetArgumentValue<T>(string argumentName, out T value)
        {
            value = default!;
            if (TryGetArgumentValue(argumentName, out var valueObj) == false) return false;

            if (valueObj == null && typeof(T).IsValueType) return false;

            if (valueObj == null) return true;

            if(typeof(T) == typeof(string))
            {
                value = (T)Convert.ChangeType(valueObj.ToString(), typeof(T), CultureInfo.InvariantCulture);
                return true;
            }

            if(typeof(T) == typeof(Numerical))
            {
                if(Numerical.TryCreate(valueObj, out var numerical))
                {
                    value = (T)Convert.ChangeType(numerical, typeof(T), CultureInfo.InvariantCulture);
                    return true;
                }
            }

            if(TypeCoercion.CanCast(valueObj.GetType(), typeof(T)))
            {
                value = (T)Convert.ChangeType(valueObj, typeof(T), CultureInfo.InvariantCulture);
                return true;
            }
            return false;
        }

        internal IEnumerable<UserDefinedArgument> AllArguments => DefinedPositionalArguments.Concat(AdditionalPositionalArguments).Concat(AdditionalKeywordArguments);

        internal static UserDefinedArgumentData Create(ILanguageDefinition languageDefinition, ParameterDeclaration[] declaration, object?[] passedValues)
        {
            var remainingDeclaredArguments = declaration.ToDictionary(dec => dec.Name);
            var definedPositionalArgs = new List<UserDefinedArgument>();
            var additionalPositionalArgs = new List<UserDefinedArgument>();
            var additionalKeywordArgs = new List<UserDefinedArgument>();
            var encounteredOutOfOrderNamedParam = false;

            for (var argIndex = 0; argIndex < passedValues.Length; ++argIndex)
            {
                var passed = passedValues[argIndex];
                if (argIndex < declaration.Length)
                {
                    var declared = declaration[argIndex];
                    if(passed is ValueTuple<string, object?> tuple)
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        definedPositionalArgs.Add(new UserDefinedArgument(declared.Name, passed, argIndex, true));
                        remainingDeclaredArguments.Remove(declared.Name);
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            return new UserDefinedArgumentData(
                definedPositionalArgs,
                additionalPositionalArgs, 
                additionalKeywordArgs
            );
        }
    }
}
