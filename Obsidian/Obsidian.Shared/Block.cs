using ExpressionParser;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Obsidian
{
    public class Block
    {
        public Block(string name, int index, ExpressionData expression)
        {
            Name = name;
            Index = index;
            Expression = expression;
        }
        public ExpressionData Expression { get; }
        public string Name { get; }
        public int Index { get; }

        public string Render(Dictionary<string, object?> parameters)
        {
            return Expression.Evaluate(parameters)?.ToString() ?? string.Empty;
        }
    }
}
