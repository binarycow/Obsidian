using ExpressionParser;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Obsidian
{
    internal class Block
    {
        internal Block(string name, int index, Expression expression)
        {
            Name = name;
            Index = index;
            Expression = expression;
        }
        internal Expression Expression { get; }
        internal string Name { get; }
        internal int Index { get; }

        //internal string Render(Dictionary<string, object?> parameters)
        //{
        //    return Expression.Evaluate(parameters)?.ToString() ?? string.Empty;
        //}
    }
}
