using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser
{
    public class ParameterDeclaration
    {
        public ParameterDeclaration(string name, object? defaultValue) : this(name)
        {
            DefaultValue = defaultValue;
            Optional = true;
        }
        public ParameterDeclaration(string name)
        {
            Name = name;
        }
        public string Name { get; }
        public object? DefaultValue { get; } = null;
        public bool Optional { get; } = false;
    }
}
