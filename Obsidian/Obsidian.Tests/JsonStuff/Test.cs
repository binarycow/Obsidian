using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Obsidian.Tests
{
    public class Test : Item
    {
        public string TestName { get; set; }
        public string RootPath { get; set; }
        public string InputFile { get; set; }
        public string ExpectedFile { get; set; }
        public string ActualFile { get; set; }
        public string VariablesFile { get; set; }
        public bool trim_blocks { get; set; } = false;
        public bool lstrip_blocks { get; set; } = false;

        public override string Name => TestName;

        public override Item this[string name] => throw new NotImplementedException();
    }
}
