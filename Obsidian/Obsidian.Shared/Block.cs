using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Obsidian
{
    public class Block
    {
        public Block(string name, int index, Expression expression)
        {
            Name = name;
            Index = index;
            Expression = expression;
        }
        public Expression Expression { get; }
        public string Name { get; }
        public int Index { get; }
    }
}
