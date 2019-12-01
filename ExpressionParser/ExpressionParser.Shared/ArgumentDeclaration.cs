using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser
{
    public class ArgumentDeclaration
    {
        public ArgumentDeclaration(string name, object? defaultValue) : this(name)
        {
            DefaultValue = defaultValue;
            HasDefaultValue = true;
        }
        public ArgumentDeclaration(string name)
        {
            Name = name;
        }
        public string Name { get; }
        public object? DefaultValue { get; } = null;
        public bool HasDefaultValue { get; } = false;
    }
}
