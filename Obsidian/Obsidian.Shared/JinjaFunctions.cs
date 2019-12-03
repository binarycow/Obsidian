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
            throw new NotImplementedException();
            //if (args.Length != 1) throw new NotImplementedException();
            //return args[0]?.ToString()?.HTMLEscape() ?? string.Empty;
        }
        public static object? Upper(UserDefinedArgumentData args)
        {
            throw new NotImplementedException();
            //if (args.Length != 1) throw new NotImplementedException();
            //return args[0].ToString().ToUpper();
        }
        public static object? Abs(UserDefinedArgumentData args)
        {
            throw new NotImplementedException();
            //if (args.Length != 1) throw new NotImplementedException();

            //switch(args[0])
            //{
            //    case null:
            //        throw new NullReferenceException();
            //    case decimal dec:
            //        return Math.Abs(dec);
            //    case double doub:
            //        return Math.Abs(doub);
            //    case short int16:
            //        return Math.Abs(int16);
            //    case int int32:
            //        return Math.Abs(int32);
            //    case long int64:
            //        return Math.Abs(int64);
            //    case sbyte sByte:
            //        return Math.Abs(sByte);
            //    case float single:
            //        return Math.Abs(single);
            //    default:
            //        var str = args[0]?.ToString() ?? throw new NullReferenceException();

            //        if(int.TryParse(str, out var i32))
            //        {
            //            return Math.Abs(i32);
            //        }
            //        if(double.TryParse(str, out var dou))
            //        {
            //            return Math.Abs(dou);
            //        }
            //        throw new ArgumentException();
            //}
        }
        public static object? Attr(UserDefinedArgumentData args)
        {
            throw new NotImplementedException();
            //if (args == null) throw new NullReferenceException();
            //if (args.Length != 2) throw new NotImplementedException();
            //var obj = args[0] ?? throw new NullReferenceException();
            //if (!(args[1] is string propertyName)) throw new ArgumentException();
            //return obj.GetType().GetProperty(propertyName)?.GetValue(args[0]);
        }

        public static object? Batch(UserDefinedArgumentData args)
        {
            throw new NotImplementedException();
            //if (args == null) throw new NullReferenceException();
            //if (args.Length < 2 || args.Length > 3) throw new NotImplementedException();

            //var obj = args[0] ?? throw new NotImplementedException();
            
            //if (args[1].TryToInt32(out var listSize) == false) throw new NotImplementedException();
            //var useDefaultValue = args.Length > 2;

            //using var enumerator = EnumeratorFactory.GetEnumerator(obj);

            //var toReturn = new List<object?>();
            //var currentList = new List<object?>();
            //toReturn.Add(currentList);
            //var itemCount = 0;
            //while(enumerator.MoveNext())
            //{
            //    currentList.Add(enumerator.Current);
            //    ++itemCount;
            //    if(itemCount == listSize)
            //    {
            //        itemCount = 0;
            //        currentList = new List<object?>();
            //        toReturn.Add(currentList);
            //    }
            //}
            //if(useDefaultValue && itemCount < listSize)
            //{
            //    currentList.AddRange(Enumerable.Repeat(args[2], listSize - itemCount));
            //}
            //return toReturn;
        }

        public static object? Capitalize(UserDefinedArgumentData args)
        {
            throw new NotImplementedException();
            //if (args == null || args.Length != 1) throw new NotImplementedException();
            //var obj = args[0] ?? throw new NullReferenceException();

            //var originalString = obj.ToString();
            //if (originalString.Length == 0) return originalString;
            //if (originalString.Length == 1) return originalString.ToUpper(CultureInfo.InvariantCulture);
            //return originalString[0].ToUpper().Concat(originalString.Substring(1));
        }
    }
}
