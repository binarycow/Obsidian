using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser
{
    public class FunctionDeclaration
    {
        public FunctionDeclaration(string name, ParameterDeclaration[] arguments)
        {
            Name = name;
            Arguments = arguments;
        }

        public string Name { get; }
        public ParameterDeclaration[] Arguments { get; }
    }
}
