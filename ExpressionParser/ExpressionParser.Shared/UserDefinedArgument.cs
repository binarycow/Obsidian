using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser
{
    public class UserDefinedArgument
    {
        internal UserDefinedArgument(string name, object? value, int index, bool provided)
        {
            Name = name;
            Value = value;
            Index = index;
            Provided = provided;
        }
        internal string Name { get; }
        internal object? Value { get; }
        internal int Index { get; }
        internal bool Provided { get; }
    }
}
