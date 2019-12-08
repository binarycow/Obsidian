using NUnit.Framework;
using Obsidian.TestCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Obsidian.Tests
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated public classes", Justification = "<Pending>")]
    public class Filters : TestClass
    {

        [Test]
        public void Batch()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["Filters"]["Filters - Batch"]);
        }

        [Test]
        public void Basic()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["Filters"]["Filters - Basic"]);
        }
        [Test]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
        public void A_E()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["Filters"]["Filters - A-E"]);
        }
    }
}
