using NUnit.Framework;
using Obsidian.Loaders;
using Obsidian.TestCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Obsidian.Tests.FromJinja.CoreTags
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated public classes", Justification = "<Pending>")]
    public class TestIfCondition
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
                _Environment.FromString("{% if true %}...{% endif %}")
            );
            Assert.AreEqual("...", template.Render());
        }

        [Test]
        public void TestElif()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString("{% if false %}XXX{% elif true%}...{% else %}XXX{% endif %}")
            );
            Assert.AreEqual("...", template.Render());
        }
        [Test]
        public void TestElifDeep()
        {
            var elifs = string.Join("\n", Enumerable.Range(1, 999).Select(index => string.Format(CultureInfo.InvariantCulture, "{{% elif a == {0} %}}{0}", index)));
            var templateString = string.Format(CultureInfo.InvariantCulture, "{{% if a == 0 %}}0{0}{{% else %}}x{{% endif %}}", elifs);

            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString(templateString)
            );
            foreach (var x in new[] { 0, 10, 999 })
            {
                Assert.AreEqual(x.ToString(CultureInfo.InvariantCulture), template.Render(a: x).Trim());
            }
            Assert.AreEqual("x", template.Render(a: 1000).Trim());
        }

        [Test]
        public void TestElse()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString("{% if false %}XXX{% else %}...{% endif %}")
            );
            Assert.AreEqual("...", template.Render());
        }

        [Test]
        public void TestEmpty()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString("[{% if true %}{% else %}{% endif %}]")
            );
            Assert.AreEqual("[]", template.Render());
        }

        [Test]
        public void TestComplete()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString("{% if a %}A{% elif b %}B{% elif c == d %}", "C{% else %}D{% endif %}")
            );
            Assert.AreEqual("C", template.Render(a: 0, b: false, c: 42, d: 42.0));
        }
        [Test]
        public void TestNoScope()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString("{% if a %}{% set foo = 1 %}{% endif %}{{ foo }}")
            );
            Assert.AreEqual("1", template.Render(a: true));
            template = new DynamicTemplateRenderer(
                _Environment.FromString("{% if true %}{% set foo = 1 %}{% endif %}{{ foo }}")
            );
            Assert.AreEqual("1", template.Render(a: true));
        }
    }
}
