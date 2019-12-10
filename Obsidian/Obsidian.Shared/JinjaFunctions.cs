using Common;
using Common.Collections;
using ExpressionParser;
using ExpressionParser.Scopes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Obsidian
{
    internal static class JinjaFunctions
    {
        internal static object? Super(IScope scope, UserDefinedArgumentData args)
        {
            var rootContext = GetRootContext(scope);
            if (rootContext.CurrentBlockName == null) throw new NotImplementedException();
            var nextBlock = rootContext.GetBlock(rootContext.CurrentBlockName, (rootContext.CurrentBlockIndex ?? 0) + 1);
            if (nextBlock == null) return null;
            nextBlock.Transform(rootContext.Transformer);
            return string.Empty;
            DynamicRootContext GetRootContext(IScope scope)
            {
                switch (scope)
                {
                    case DynamicRootContext rootContext:
                        return rootContext;
                    case DynamicContext dynamicContext:
                        var root = dynamicContext.FindRootScope();
                        if (root is DynamicRootContext dynRoot) return dynRoot;
                        throw new NotImplementedException();
                    default:
                        throw new NotImplementedException();
                }
            }
        }
        internal static object? Escape(UserDefinedArgumentData args)
        {
            if (args.TryGetArgumentValue<string>("s", out var obj) == false) throw new NotImplementedException();
            if (obj == null) throw new NullReferenceException();
            return obj.ToString(CultureInfo.InvariantCulture).HTMLEscape();
        }
        internal static object? Upper(UserDefinedArgumentData args)
        {
            if (args.TryGetArgumentValue<string>("s", out var obj) == false) throw new NotImplementedException();
            if (obj == null) throw new NullReferenceException();
            return obj.ToString(CultureInfo.InvariantCulture).ToUpper(CultureInfo.InvariantCulture);
        }
        internal static object? Abs(UserDefinedArgumentData args)
        {
            if (args.TryGetArgumentValue<Numerical>("x", out var obj) == false) throw new NotImplementedException();
            return obj.Abs();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
        internal static object? Attr(UserDefinedArgumentData args)
        {

            if (args.TryGetArgumentValue("obj", out var obj) == false) throw new NotImplementedException();
            if (args.TryGetArgumentValue<string>("name", out var name) == false) throw new NotImplementedException();
            if (obj == null) throw new NullReferenceException();
            if (name == null) throw new NullReferenceException();

            if (!(name is string propertyName)) throw new ArgumentException("name");
            return obj.GetType().GetProperty(propertyName)?.GetValue(obj);
        }

        internal static object? Batch(UserDefinedArgumentData args)
        {

            if (args.TryGetArgumentValue("value", out var value) == false) throw new NotImplementedException();
            if (args.TryGetArgumentValue<Numerical>("linecount", out var lineCountObj) == false) throw new NotImplementedException();
            var useDefaultValue = args.TryGetArgumentValue("fill_with", out var fillWith);

            value = value ?? throw new NotImplementedException();

            if (lineCountObj.TryToInt32(out var listSize) == false) throw new NotImplementedException();

            using var enumerator = EnumeratorFactory.GetEnumerator(value);

            var toReturn = new List<object?>();
            var currentList = new List<object?>();
            toReturn.Add(currentList);
            var itemCount = 0;
            while (enumerator.MoveNext())
            {
                currentList.Add(enumerator.Current);
                ++itemCount;
                if (itemCount == listSize)
                {
                    itemCount = 0;
                    currentList = new List<object?>();
                    toReturn.Add(currentList);
                }
            }
            if (useDefaultValue && itemCount > 0 && itemCount < listSize)
            {
                currentList.AddRange(Enumerable.Repeat(fillWith, listSize - itemCount));
            }
            if(currentList.Count == 0)
            {
                toReturn.Remove(currentList);
            }
            return toReturn;
        }

        internal static object? Capitalize(UserDefinedArgumentData args)
        {
            if (args.TryGetArgumentValue<string>("s", out var obj) == false) throw new NotImplementedException();
            var originalString = obj.ToString(CultureInfo.InvariantCulture);
            if (originalString.Length == 0) return originalString;
            if (originalString.Length == 1) return originalString.ToUpper(CultureInfo.InvariantCulture);
            return originalString[0].ToUpper().Concat(originalString.Substring(1));
        }
        internal static object? Center(UserDefinedArgumentData args)
        {
            if (args.TryGetArgumentValue<string>("s", out var value) == false) throw new NotImplementedException();
            if (args.TryGetArgumentValue<Numerical>("width", out var width) == false) throw new NotImplementedException();

            if (width <= value.Length)
            {
                return value;
            }

            var totalPaddingWidth = width - value.Length;
            var leftPaddingWidth = totalPaddingWidth / 2;
            return $"{new string(' ', leftPaddingWidth)}{value}{new string(' ', totalPaddingWidth - leftPaddingWidth)}";
        }
        internal static object? Default(UserDefinedArgumentData args)
        {
            if (args.TryGetArgumentValue<string>("value", out var value) == false) throw new NotImplementedException();
            var defaultValue = args.GetArgumentValue<object?>("default_value", "");
            var boolean = args.GetArgumentValue<bool>("boolean", false);

            if (boolean)
            {
                return value switch
                {
                    string strVal => string.IsNullOrEmpty(strVal),
                    _ => throw new NotImplementedException(),
                };
            }
            return value ?? defaultValue;
        }


        internal static object? DictSort(UserDefinedArgumentData args)
        {
            if (args.TryGetArgumentValue("value", out var dictionary) == false) throw new NotImplementedException();
            if (args.TryGetArgumentValue<bool>("case_sensitive", out var caseSensitive) == false) throw new NotImplementedException();
            if (args.TryGetArgumentValue<string>("by", out var by) == false) throw new NotImplementedException();
            if (args.TryGetArgumentValue<bool>("reverse", out var reverse) == false) throw new NotImplementedException();

            dictionary = dictionary ?? throw new NotImplementedException();
            var dictionaryType = dictionary.GetType();
            if (dictionaryType.IsAssignableToGenericType(typeof(Dictionary<,>), out var _) == false) throw new NotImplementedException();
            return DictionarySorter.SortDictionaryObj(dictionary, by == "key", reverse, caseSensitive);
        }

        




    }
}
