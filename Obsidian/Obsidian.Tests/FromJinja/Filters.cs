using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Obsidian.TestCore;
using static Obsidian.Tests.AssertConfig;

namespace Obsidian.Tests.FromJinja
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]

    public class Filters
    {

        JinjaEnvironment _Environment = new JinjaEnvironment();



        [Test]
        public void TestAbs()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString("{{ -1|abs }}|{{ 1|abs }}")
            );
            MyAssert.AreEqual("1|1", template.Render());
        }
        [Test]
        public void TestAttributeMap()
        {
            var users = new[]
            {
                new { name = "john", },
                new { name = "jane", },
                new { name = "mike", },
            };

            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString("{{ users|map(attribute=\"name\")|join(\" | \") }}")
            );
            MyAssert.AreEqual("john|jane|mike", template.Render(users: users));
        }
        [Test]
        public void TestBatch()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString("{{ foo|batch(3)|list }}|{{ foo|batch(3, 'X')|list }}")
            );
            var output = template.Render(foo: Enumerable.Range(0, 10));
            MyAssert.AreEqual("[[0, 1, 2], [3, 4, 5], [6, 7, 8], [9]]|[[0, 1, 2], [3, 4, 5], [6, 7, 8], [9, 'X', 'X']]", output);
        }
        [Test]
        public void TestBlock()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString("{{ \"foo bar baz\"|wordcount }}")
            );
            MyAssert.AreEqual("3", template.Render());
        }
        [Test]
        public void TestBoolReject()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString("{{ [none, false, 0, 1, 2, 3, 4, 5]|reject|join(\" | \") }}")
            );
            MyAssert.AreEqual("None|False|0", template.Render());
        }
        [Test]
        public void TestBoolSelect()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString("{{ [none, false, 0, 1, 2, 3, 4, 5]|select|join(\" | \") }}")
            );
            MyAssert.AreEqual("1|2|3|4|5", template.Render());
        }
        [Test]
        public void TestCapitalize()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString("{{ \"foo bar\"|capitalize }}")
            );
            MyAssert.AreEqual("Foo bar", template.Render());
        }
        [Test]
        public void TestCenter()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString("{{ \"foo\"|center(9) }}")
            );
            MyAssert.AreEqual("   foo   ", template.Render());
        }
        [Test]
        public void TestChaining()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString("{{ ['<foo>', '<bar>']|first|upper|escape }}")
            );
            MyAssert.AreEqual("&lt;FOO&gt", template.Render());
        }


        [Test]
        public void TestDefault()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString("{{ missing|default('no') }}|{{ false|default('no') }}|{{ false|default('no', true) }}|{{ given|default('no') }}")
            );
            MyAssert.AreEqual("no|False|no|yes", template.Render(given: "yes"));
        }

























    }
}
