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
        }
        [Test]
        public void TestBlockEscaping()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString("{% set foo %}<em>{{ test }}</em>{% endset %}foo: {{ foo }}")
            );
            Assert.AreEqual("foo: <em>&lt;unsafe&gt;</em>", template.Render(test: "<unsafe>"));
        }
        [Test]
        public void TestSetInvalid()
        {
            throw new TestNotFinishedException();
        }
        [Test]
        public void TestNamespaceRedefined()
        {
            throw new TestNotFinishedException();
        }
        [Test]
        public void TestNamespace()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString("{% set ns = namespace() %}{% set ns.bar = \"42\" %}{{ ns.bar }}")
            );
            Assert.AreEqual("42", template.Render());
        }
        [Test]
        public void TestNamespaceBlock()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString("{% set ns = namespace() %}{% set ns.bar %}42{% endset %}{{ ns.bar }}")
            );
            Assert.AreEqual("42", template.Render());
        }
        [Test]
        public void TestInitNamespace()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString("{% set ns = namespace(d, self=37) %}{% set ns.b = 42 %}{{ ns.a }}|{{ ns.self }}|{{ ns.b }}")
            );
            Assert.AreEqual("13|37|42", template.Render(d: new Dictionary<string, object?>
            {
                { "a", 13 }
            }));
        }
        [Test]
        public void TestNamespaceLoop()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString(
                    "{% set ns = namespace(found=false) %}",
                    "{% for x in range(4) %}",
                    "{% if x == v %}",
                    "{% set ns.found = true %}",
                    "{% endif %}",
                    "{% endfor %}",
                    "{{ ns.found }}"
                )
            );
            Assert.AreEqual("True", template.Render(v: 3));
            Assert.AreEqual("False", template.Render(v: 4));
        }

        [Test]
        public void TestNamespaceMacro()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString(
                    "{% set ns = namespace() %}",
                    "{% set ns.a = 13 %}",
                    "{% macro magic(x) %}",
                    "{% set x.b = 37 %}",
                    "{% endmacro %}",
                    "{{ magic(ns) }}",
                    "{{ ns.a }}|{{ ns.b }}"
                )
            );
            Assert.AreEqual("13|37", template.Render());
        }

        [Test]
        public void TestBlockEscapingFiltered()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString(
                    "{% set foo | trim %}<em>{{ test }}</em>    ",
                    "{% endset %}foo: {{ foo }}"
                )
            );
            Assert.AreEqual("foo: <em>&lt;unsafe&gt;</em>", template.Render(test: "<unsafe>"));
        }
        [Test]
        public void TestBlockFiltered()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString(
                    "{% set foo | trim | length | string %} 42    {% endset %}",
                    "{{ foo }}"
                )
            );
            Assert.AreEqual("2", template.Render());
        }
        [Test]
        public void TestBlockFilteredSet()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString(
                    "{% set a = \" xxx \" %}",
                    "{% set foo | myfilter(a) | trim | length | string %}",
                    " {% set b = \" yy \" %} 42 {{ a }}{{ b }}   ",
                    "{% endset %}",
                    "{{ foo }}"
                )
            );
            Assert.AreEqual("11", template.Render());
        }
    }
}
