using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obsidian.TestCore;
using NUnit.Framework;

namespace Obsidian.Tests
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "<Pending>")]
    internal class WhiteSpace : TestClass
    {
        [Test]
        public void Defaults()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["WhiteSpace"]["Defaults"]);
        }
        [Test]
        public void TrimBlocks()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["WhiteSpace"]["TrimBlocks"]);
        }
        [Test]
        public void LStrip()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["WhiteSpace"]["LStrip"]);
        }
        [Test]
        public void LStripAndTrim()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["WhiteSpace"]["LStrip And Trim"]);
        }
        [Test]
        public void ManualStrip()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["WhiteSpace"]["Manual Strip"]);
        }
    }
}
