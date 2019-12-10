using Common;
using Obsidian.AST.Nodes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Obsidian
{
    internal class JinjaCustomStringProvider : CustomToStringProvider
    {
        private JinjaCustomStringProvider()
        {
            Register<ContainerNode>(containerNode => $"{nameof(ContainerNode)}");
            Register<ArgumentNameCollection>(argCollection => $"({string.Join(", ", argCollection.Select(arg => $"'{arg}'"))})");
            RegisterValueType<double>(doubleVal => doubleVal.ToString("0.0###########", CultureInfo.InvariantCulture));
        }

        private static readonly Lazy<JinjaCustomStringProvider> _Instance = new Lazy<JinjaCustomStringProvider>(() => new JinjaCustomStringProvider());
        internal static JinjaCustomStringProvider Instance => _Instance.Value;

        internal override string FormatIDictionary(IEnumerable<KeyValuePair<object, object?>> dictionary)
        {
            if (dictionary.Any()) throw new NotImplementedException();
            return "{}";
        }

        internal override string FormatIEnumerable(IEnumerable<object?> enumerable)
        {
            using var checkout = StringBuilderPool.Instance.Checkout();
            var stringBuilder = checkout.CheckedOutObject;
            stringBuilder.Append("[");

            var first = true;
            foreach(var item in enumerable)
            {
                if(first == false)
                {
                    stringBuilder.Append(", ");
                }
                first = false;

                if(item is string _)
                {
                    stringBuilder.Append($"'{this.ToString(item)}'");
                }
                else
                {
                    stringBuilder.Append(this.ToString(item));
                }
            }
            stringBuilder.Append("]");
            return stringBuilder.ToString();
        }

        internal override string FormatTuple(object? tuple, PropertyInfo[] tupleProperties)
        {
            using var checkout = StringBuilderPool.Instance.Checkout();
            var stringBuilder = checkout.CheckedOutObject;
            stringBuilder.Append("(");

            var first = true;
            foreach (var item in tupleProperties)
            {
                if (first == false)
                {
                    stringBuilder.Append(", ");
                }
                first = false;

                var value = item.GetValue(tuple);
                if(value is string _)
                {
                    stringBuilder.Append($"'{this.ToString(value)}'");
                }
                else
                {
                    stringBuilder.Append(this.ToString(value));
                }
            }
            stringBuilder.Append(")");
            return stringBuilder.ToString();
        }
    }
}
