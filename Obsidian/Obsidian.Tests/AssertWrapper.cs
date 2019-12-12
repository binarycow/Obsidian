using NUnit.Framework;
using Obsidian.TestCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Obsidian.Tests
{
    public static class AssertWrapper
    {
        public static void TestTemplate(Item test)
        {
            TestRunner.TestTemplate(test, out var actualOutput, out var expectedOutput);
            NUnit.Framework.Assert.AreEqual(expectedOutput, actualOutput);
        }
    }
}
