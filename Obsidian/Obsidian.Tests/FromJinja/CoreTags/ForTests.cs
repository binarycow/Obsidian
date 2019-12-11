using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Obsidian.TestCore;

namespace Obsidian.Tests.FromJinja.CoreTags
{
    public class ForTests
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
        public void TestSimple()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString("{% for item in seq %}{{ item }}{% endfor %}")
            );
            Assert.AreEqual("0123456789", template.Render(seq: Enumerable.Range(0, 10)));
        }


    }
}
