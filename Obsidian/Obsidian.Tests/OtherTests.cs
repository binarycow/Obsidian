using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obsidian.TestCore;
using NUnit.Framework;

namespace Obsidian.Tests
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "<Pending>")]
    internal class OtherTests : TestClass
    {

        [Test]
        internal void Test1()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["Other Tests"]["Test1"]);
        }
        [Test]
        internal void Test2()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["Other Tests"]["Test2"]);
        }
    }
}
