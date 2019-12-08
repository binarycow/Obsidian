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
        internal void Batch()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["Filters"]["Filters - Batch"]);
        }

        [Test]
        internal void Basic()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["Filters"]["Filters - Basic"]);
        }
        [Test]
        internal void A_E()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["Filters"]["Filters - A-E"]);
        }
    }
}
