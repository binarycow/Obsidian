using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressionParser
{
    public class FunctionDeclaration
    {
        public FunctionDeclaration(Type returnType, string name, ParameterDeclaration[] arguments, string[]? aliases = null)
        {
            Name = name;
            Arguments = arguments;
            ReturnType = returnType;
            Aliases = aliases ?? Enumerable.Empty<string>();
        }

        public IEnumerable<string> Aliases { get; }
        public string Name { get; }
        public IEnumerable<ParameterDeclaration> Arguments { get; }
        public Type ReturnType { get; }
    }
}
