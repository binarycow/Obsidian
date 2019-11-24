using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Obsidian.TestGeneration
{
    public class Test : TestItem
    {
        public Test()
        {

        }
        public string TestName { get; set; }
        public override string Kind { get; } = "Test";

    }
}
