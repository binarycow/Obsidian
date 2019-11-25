using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser.Configuration
{
    public class FunctionDefinition
    {
        public FunctionDefinition(string name, params OverloadDefinition[] overloads)
        {
            Name = name;
            OverloadDefinitions = overloads;
        }
        public string Name { get; }
        public OverloadDefinition[] OverloadDefinitions { get; }

        public static FunctionDefinition Create(string name, params OverloadDefinition[] overloads)
        {
            return new FunctionDefinition(name, overloads);
        }
    }
}
