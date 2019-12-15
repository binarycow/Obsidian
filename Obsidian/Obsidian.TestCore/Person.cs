using System;
using System.Collections.Generic;
using System.Text;

namespace Obsidian.TestCore
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
    public class Person
    {
        public static string name => "John Smith";

        public string getName()
        {
            return "Jacob Smith";
        }
    }
}
