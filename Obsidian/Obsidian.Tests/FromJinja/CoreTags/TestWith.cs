using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Obsidian.TestCore;

namespace Obsidian.Tests.FromJinja.CoreTags
{
    public class TestWith
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
        public void TestWithNormal()
        {
            throw new TestNotFinishedException();
        }
        [Test]
        public void TestWithArgumentScoping()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString(@"
        {%- with a=1, b=2, c=b, d=e, e=5 -%}
            {{ a }}|{{ b }}|{{ c }}|{{ d }}|{{ e }}
        {%- endwith -%}
")
            );
            Assert.AreEqual("1|2|3|4|5", template.Render(b: 3, e: 4));
        }
    }
}
