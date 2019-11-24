using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obsidian.Tests.Utilities;
using NUnit.Framework;

namespace Obsidian.Tests
{
    public class FeatureTests : TestClass
    {

        [Test]
        public void NullMasterFallback_Standalone()
        {
            TestRunner.TestTemplate(TestRunner.TestItems["Feature Tests"]["Null Master Fallback"]["Standalone"]);
        }
        [Test]
        public void NullMasterFallback_Master()
        {
            TestRunner.TestTemplate(TestRunner.TestItems["Feature Tests"]["Null Master Fallback"]["Master"]);
        }
        [Test]
        public void ForLoopVariables()
        {
            TestRunner.TestTemplate(TestRunner.TestItems["Feature Tests"]["For Loop Variables"]);
        }
    }
}
