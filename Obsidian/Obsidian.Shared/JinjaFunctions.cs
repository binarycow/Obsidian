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
    public static class JinjaFunctions
    {

        public static object? Super(UserDefinedArgumentData args)
        {
            throw new NotImplementedException();

            // TODO: This only works with string render tranformer

            //var rootContext = GetRootContext(scope);
            //if (rootContext.CurrentBlockName == null) throw new NotImplementedException();
            //var nextBlock = rootContext.GetBlock(rootContext.CurrentBlockName, (rootContext.CurrentBlockIndex ?? 0) + 1);
            //if (nextBlock == null) return null;
            //nextBlock.Transform(rootContext.Transformer);
            //return string.Empty;
            //DynamicRootContext GetRootContext(IScope scope)
            //{
            //    switch (scope)
            //    {
            //        case DynamicRootContext rootContext:
            //            return rootContext;
            //        case DynamicContext dynamicContext:
            //            var root = dynamicContext.FindRootScope();
            //            if (root is DynamicRootContext dynRoot) return dynRoot;
            //            throw new NotImplementedException();
            //        default:
            //            throw new NotImplementedException();
            //    }

            //}
        }
        public static object? Escape(UserDefinedArgumentData args)
        {
            if (args.TryGetArgumentValue<string>("s", out var obj) == false) throw new NotImplementedException();
            if (obj == null) throw new NullReferenceException();
            return obj.ToString().HTMLEscape();
        }
        public static object? Upper(UserDefinedArgumentData args)
        {
            if (args.TryGetArgumentValue<string>("s", out var obj) == false) throw new NotImplementedException();
            if (obj == null) throw new NullReferenceException();
            return obj.ToString().ToUpper(CultureInfo.InvariantCulture);
        }
        public static object? Abs(UserDefinedArgumentData args)
        {

            if (args.TryGetArgumentValue("x", out var obj) == false) throw new NotImplementedException();

            switch (obj)
            {
                case null:
                    throw new NullReferenceException();
                case decimal dec:
                    return Math.Abs(dec);
                case double doub:
                    return Math.Abs(doub);
                case short int16:
                    return Math.Abs(int16);
                case int int32:
                    return Math.Abs(int32);
                case long int64:
                    return Math.Abs(int64);
                case sbyte sByte:
                    return Math.Abs(sByte);
                case float single:
                    return Math.Abs(single);
                default:
                    var str = obj?.ToString() ?? throw new NullReferenceException();

                    if (int.TryParse(str, out var i32))
                    {
                        return Math.Abs(i32);
                    }
                    if (double.TryParse(str, out var dou))
                    {
                        return Math.Abs(dou);
                    }
                    throw new ArgumentException();
            }
        }
        public static object? Attr(UserDefinedArgumentData args)
        {

            if (args.TryGetArgumentValue("obj", out var obj) == false) throw new NotImplementedException();
            if (args.TryGetArgumentValue<string>("name", out var name) == false) throw new NotImplementedException();
            if (obj == null) throw new NullReferenceException();
            if (name == null) throw new NullReferenceException();

            if (!(name is string propertyName)) throw new ArgumentException();
            return obj.GetType().GetProperty(propertyName)?.GetValue(obj);
        }

        public static object? Batch(UserDefinedArgumentData args)
        {

            if (args.TryGetArgumentValue("value", out var value) == false) throw new NotImplementedException();
            if (args.TryGetArgumentValue<int>("linecount", out var lineCountObj) == false) throw new NotImplementedException();
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
            if (useDefaultValue && itemCount < listSize)
            {
                currentList.AddRange(Enumerable.Repeat(fillWith, listSize - itemCount));
            }
            return toReturn;
        }

        public static object? Capitalize(UserDefinedArgumentData args)
        {
            if (args.TryGetArgumentValue<string>("s", out var obj) == false) throw new NotImplementedException();
            var originalString = obj.ToString();
            if (originalString.Length == 0) return originalString;
            if (originalString.Length == 1) return originalString.ToUpper(CultureInfo.InvariantCulture);
            return originalString[0].ToUpper().Concat(originalString.Substring(1));
        }
        public static object? Center(UserDefinedArgumentData args)
        {
            if (args.TryGetArgumentValue<string>("value", out var value) == false) throw new NotImplementedException();
            if (args.TryGetArgumentValue<int>("width", out var width) == false) throw new NotImplementedException();

            if (width <= value.Length)
            {
                return value;
            }

            var totalPaddingWidth = width - value.Length;
            var leftPaddingWidth = totalPaddingWidth / 2;
            return $"{new string(' ', leftPaddingWidth)}{value}{new string(' ', totalPaddingWidth - leftPaddingWidth)}";
        }
        public static object? Default(UserDefinedArgumentData args)
        {
            if (args.TryGetArgumentValue<string>("value", out var value) == false) throw new NotImplementedException();
            var defaultValue = args.GetArgumentValue<object?>("default_value", "");
            var boolean = args.GetArgumentValue("boolean", false);

            if (boolean)
            {

                throw new NotImplementedException();
            }
            return value ?? defaultValue;
        }




    }
}
