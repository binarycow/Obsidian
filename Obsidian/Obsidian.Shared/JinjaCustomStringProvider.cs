using Common;
using Obsidian.AST.Nodes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Obsidian
{
    internal class JinjaCustomStringProvider : CustomToStringProvider
    {
        private JinjaCustomStringProvider()
        {
            Register<ContainerNode>(containerNode => $"{nameof(ContainerNode)}");
        }

        private static readonly Lazy<JinjaCustomStringProvider> _Instance = new Lazy<JinjaCustomStringProvider>(() => new JinjaCustomStringProvider());
        public static JinjaCustomStringProvider Instance => _Instance.Value;

        public override string FormatIEnumerable(IEnumerable<object?> enumerable)
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

        public override string FormatTuple(object? tuple, PropertyInfo[] tupleProperties)
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
