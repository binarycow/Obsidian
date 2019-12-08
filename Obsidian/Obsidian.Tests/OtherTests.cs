using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obsidian.TestCore;
using NUnit.Framework;

namespace Obsidian.Tests
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated public classes", Justification = "<Pending>")]
    public class OtherTests : TestClass
    {

        [Test]
        public void Test1()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["Other Tests"]["Test1"]);
        }
        [Test]
        public void Test2()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["Other Tests"]["Test2"]);
        }
    }
}
