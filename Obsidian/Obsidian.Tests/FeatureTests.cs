using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obsidian.TestCore;
using NUnit.Framework;

namespace Obsidian.Tests
{
    public class FeatureTests : TestClass
    {

        [Test]
        public void NullMasterFallback_Standalone()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["Feature Tests"]["Null Master Fallback"]["Standalone"]);
        }
        [Test]
        public void NullMasterFallback_Master()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["Feature Tests"]["Null Master Fallback"]["Master"]);
        }
        [Test]
        public void ForLoopVariables()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["Feature Tests"]["For Loop Variables"]);
        }
        [Test]
        public void Set()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["Feature Tests"]["Set"]);
        }
    }
}
