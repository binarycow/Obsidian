using ExpressionParser;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Obsidian
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    public class JinjaUserDefinedFunction : UserDefinedFunction
    {
        internal JinjaUserDefinedFunction(FunctionDeclaration declaration, UserDefinedFunctionDelegate body, 
            bool usesCaller = false) : base(declaration, body)
        {
            name = declaration.Name;
            arguments = new ReadOnlyCollection<string>(declaration.Arguments.Select(arg => arg.Name).ToArray());
            caller = usesCaller;
        }

        public string name { get; }

        public ReadOnlyCollection<string> arguments { get; }

        public static bool catch_kwargs => true; // TODO: Implement this!

        public static bool catch_varargs => true; // TODO: Implement this!

        public bool caller { get; }

        protected override object? Invoke(UserDefinedArgumentData argumentData)
        {
            return base.Invoke(argumentData);
        }
    }
}
