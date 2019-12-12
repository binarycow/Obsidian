using System;
using System.Collections.Generic;
using System.Text;

namespace Obsidian.Tests
{
    public static class AssertConfig
    {
        public static IAsserter MyAssert { get; set; } = new NUnitAsserter();
    }
}
