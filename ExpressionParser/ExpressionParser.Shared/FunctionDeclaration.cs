using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser
{
    public class FunctionDeclaration
    {
        public FunctionDeclaration(string name, ArgumentDeclaration[] arguments)
        {
            Name = name;
            Arguments = arguments;
        }

        public string Name { get; }
        public ArgumentDeclaration[] Arguments { get; }
    }
}
