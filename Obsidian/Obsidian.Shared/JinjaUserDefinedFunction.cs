using ExpressionParser;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Obsidian
{
    public class JinjaUserDefinedFunction : UserDefinedFunction
    {
        internal JinjaUserDefinedFunction(FunctionDeclaration declaration, UserDefinedFunctionDelegate body) : base(declaration, body)
        {
            name = declaration.Name;
            arguments = new ReadOnlyCollection<string>(declaration.Arguments.Select(arg => arg.Name).ToArray());
        }
        public string name { get; }
        public ReadOnlyCollection<string> arguments { get; }
        public bool catch_kwargs => true; // TODO: Implement this!
        public bool catch_varargs => true; // TODO: Implement this!
        public bool caller => true; // TODO: Implement this!

        protected override object? Invoke(UserDefinedArgumentData argumentData)
        {
            return base.Invoke(argumentData);
        }
    }
}
