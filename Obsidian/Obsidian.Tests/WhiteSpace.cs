using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obsidian.Tests.Utilities;
using NUnit.Framework;

namespace Obsidian.Tests
{
    public class WhiteSpace : TestClass
    {
        [Test]
        public void Defaults()
        {
            TestRunner.TestTemplate(TestRunner.TestItems["WhiteSpace"]["Defaults"]);
        }
        [Test]
        public void TrimBlocks()
        {
            TestRunner.TestTemplate(TestRunner.TestItems["WhiteSpace"]["TrimBlocks"]);
        }
        [Test]
        public void LStrip()
        {
            TestRunner.TestTemplate(TestRunner.TestItems["WhiteSpace"]["LStrip"]);
        }
        [Test]
        public void LStripAndTrim()
        {
            TestRunner.TestTemplate(TestRunner.TestItems["WhiteSpace"]["LStrip And Trim"]);
        }
        [Test]
        public void ManualStrip()
        {
            TestRunner.TestTemplate(TestRunner.TestItems["WhiteSpace"]["Manual Strip"]);
        }
    }
}
