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
        private UserDefinedArgumentData(IEnumerable<UserDefinedArgument> definedPositionalArguments, IEnumerable<UserDefinedArgument> additionalPositionalArguments, IEnumerable<UserDefinedArgument> additionalKeywordArguments)
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
            throw new NotImplementedException();
            //var additionalPositionalArguments = new List<UserDefinedArgument>();
            //var additionalKeywordArguments = new Dictionary<string, UserDefinedArgument>();
            //var declaredArguments = new UserDefinedArgument?[declaration.Length];
            //var initialPositionalArguments = passedValues.TakeWhile(arg => !(arg is ValueTuple<string, object?>)).ToArray();

            //for (var argIndex = initialPositionalArguments.Length; argIndex < passedValues.Length; ++argIndex)
            //{
            //    var arg = passedValues[argIndex];
            //    if (arg is ValueTuple<string, object?> tuple)
            //    {
            //        additionalKeywordArguments.Add(tuple.Item1, new UserDefinedArgument(tuple.Item1, tuple.Item2, argIndex, true));
            //    }
            //    else
            //    {
            //        additionalPositionalArguments.Add(new UserDefinedArgument(string.Empty, arg, argIndex, true));
            //    }
            //}

            //for (var argIndex = 0; argIndex < Math.Min(declaration.Length, initialPositionalArguments.Length); ++argIndex)
            //{
            //    var dec = declaration[argIndex];
            //    var prov = initialPositionalArguments[argIndex];
            //    declaredArguments[argIndex] = new UserDefinedArgument(dec.Name, prov, argIndex, true);
            //}

            //for (var argIndex = 0; argIndex < declaredArguments.Length; ++argIndex)
            //{
            //    var dec = declaration[argIndex];
            //    if (declaredArguments[argIndex] != null) continue;
            //    if (additionalKeywordArguments.TryGetValue(dec.Name, out var providedKeywordArg))
            //    {
            //        declaredArguments[argIndex] = providedKeywordArg;
            //        additionalKeywordArguments.Remove(declaration[argIndex].Name);
            //        continue;
            //    }
            //    if (dec.Optional)
            //    {
            //        declaredArguments[argIndex] = new UserDefinedArgument(dec.Name, dec.DefaultValue, argIndex, false);
            //        continue;
            //    }
            //    if (languageDefinition.RequireNonDefaultArguments == false)
            //    {
            //        declaredArguments[argIndex] = new UserDefinedArgument(dec.Name, null, argIndex, false);
            //        continue;
            //    }
            //    throw new NotImplementedException(); //Argument not provided
            //}

            //var nonNullDeclaredArguments = declaredArguments.Where(arg => arg != null).OfType<UserDefinedArgument>().ToArray();

            //return new UserDefinedArgumentData(
            //    nonNullDeclaredArguments,
            //    additionalPositionalArguments,
            //    additionalKeywordArguments.Values
            //);

        }
    }
}
