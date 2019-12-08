using ExpressionParser;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Obsidian
{
    internal class JinjaUserDefinedFunction : UserDefinedFunction
    {
        internal JinjaUserDefinedFunction(FunctionDeclaration declaration, UserDefinedFunctionDelegate body) : base(declaration, body)
        {
            name = declaration.Name;
            arguments = new ReadOnlyCollection<string>(declaration.Arguments.Select(arg => arg.Name).ToArray());
        }
        internal string name { get; }
        internal ReadOnlyCollection<string> arguments { get; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
        internal static bool catch_kwargs => true; // TODO: Implement this!

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
        internal static bool catch_varargs => true; // TODO: Implement this!

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
        internal static bool caller => true; // TODO: Implement this!

        protected override object? Invoke(UserDefinedArgumentData argumentData)
        {
            return base.Invoke(argumentData);
        }
    }
}
