using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser.Configuration
{
    internal class FunctionDefinition
    {
        internal FunctionDefinition(string name, params OverloadDefinition[] overloads)
        {
            Name = name;
            OverloadDefinitions = overloads;
        }
        internal string Name { get; }
        internal OverloadDefinition[] OverloadDefinitions { get; }

        internal static FunctionDefinition Create(string name, params OverloadDefinition[] overloads)
        {
            return new FunctionDefinition(name, overloads);
        }
    }
}
