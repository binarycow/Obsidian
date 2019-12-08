using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obsidian.TestCore;
using NUnit.Framework;

namespace Obsidian.Tests
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "<Pending>")]
    internal class FeatureTests : TestClass
    {

        [Test]
        internal void NullMasterFallback_Standalone()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["Feature Tests"]["Null Master Fallback"]["Standalone"]);
        }
        [Test]
        internal void NullMasterFallback_Master()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["Feature Tests"]["Null Master Fallback"]["Master"]);
        }
        [Test]
        internal void ForLoopVariables()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["Feature Tests"]["For Loop Variables"]);
        }
        [Test]
        internal void Set()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["Feature Tests"]["Set"]);
        }
    }
}
