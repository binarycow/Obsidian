using NUnit.Framework;
using Obsidian.TestCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Obsidian.Tests.FromJinja.CoreTags
{
    public class TestSet
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
        public void TestNormal()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString("{% set foo = 1 %}{{ foo }}")
            );
            Assert.AreEqual("1", template.Render());
        }
        [Test]
        public void TestBlock()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString("{% set foo %}42{% endset %}{{ foo }}")
            );
            Assert.AreEqual("42", template.Render());
            Assert.AreEqual()
        }
    }
}
