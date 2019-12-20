using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Obsidian.TestCore;
using static Obsidian.Tests.AssertConfig;

namespace Obsidian.Tests
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated public classes", Justification = "<Pending>")]
    public class MiscTests
    {
        [SetUp]
        public void CreateEnvironment()
        {
            _Environment = new JinjaEnvironment(settings: new EnvironmentSettings
            {
                TrimBlocks = true
            });
        }
        private JinjaEnvironment _Environment = new JinjaEnvironment();

        [Test]
        public void TestNegNumber()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString("{{ ( -10 ) }}")
            );
            MyAssert.AreEqual("-10", template.Render());
        }
        [Test]
        public void TestOneItemTuple()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString("{{ (1 , ) }}")
            );
            MyAssert.AreEqual("(1)", template.Render());
        }
        [Test]
        public void TestTwoItemTuple()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString("{{ ( 1 , 2 ) }}")
            );
            MyAssert.AreEqual("(1, 2)", template.Render());
        }
        [Test]
        public void TestThreeItemTuple()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString("{{ ( 1 , 2 , 3 ) }}")
            );
            MyAssert.AreEqual("(1, 2, 3)", template.Render());
        }
    }
}
