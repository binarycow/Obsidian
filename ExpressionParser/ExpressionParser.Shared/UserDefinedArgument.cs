using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser
{
    public class UserDefinedArgument
    {
        public UserDefinedArgument(string name, object? value, int index, bool provided)
        {
            Name = name;
            Value = value;
            Index = index;
            Provided = provided;
        }
        public string Name { get; }
        public object? Value { get; }
        public int Index { get; }
        public bool Provided { get; }
    }
}
