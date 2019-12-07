using NUnit.Framework;
using Obsidian.TestCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Obsidian.Tests
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "<Pending>")]
    internal class Filters : TestClass
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
        public void A_E()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["Filters"]["Filters - A-E"]);
        }
    }
}
