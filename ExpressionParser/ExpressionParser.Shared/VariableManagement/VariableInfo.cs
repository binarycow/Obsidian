using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressionParser.VariableManagement
{
    public class VariableInfo
    {
        public VariableInfo(string name, Type type, int index)
        {
            Name = name;
            Type = type;
            Index = index;
        }
        public int Index { get; }
        public Type Type { get; }
        public string Name { get; }
    }
}
