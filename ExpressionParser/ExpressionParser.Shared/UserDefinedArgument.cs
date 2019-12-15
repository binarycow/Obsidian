using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace ExpressionParser
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
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

        public string DebuggerDisplay => $"Name: {Name}     |     Value: {{{Value}}}     |     Index: {Index}     |     Provided: {Provided}";
    }
}
