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
        internal void Defaults()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["WhiteSpace"]["Defaults"]);
        }
        [Test]
        internal void TrimBlocks()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["WhiteSpace"]["TrimBlocks"]);
        }
        [Test]
        internal void LStrip()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["WhiteSpace"]["LStrip"]);
        }
        [Test]
        internal void LStripAndTrim()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["WhiteSpace"]["LStrip And Trim"]);
        }
        [Test]
        internal void ManualStrip()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["WhiteSpace"]["Manual Strip"]);
        }
    }
}
