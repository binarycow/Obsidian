using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser
{
    public class FunctionDeclaration
    {
        public FunctionDeclaration(Type returnType, string name, ParameterDeclaration[] arguments)
        {
            Name = name;
            Arguments = arguments;
            ReturnType = returnType;
        }

        public string Name { get; }
        public ParameterDeclaration[] Arguments { get; }
        public Type ReturnType { get; }
    }
}
