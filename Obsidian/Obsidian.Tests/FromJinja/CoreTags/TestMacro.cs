using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Obsidian.Loaders;
using Obsidian.TestCore;
using static Obsidian.Tests.AssertConfig;

namespace Obsidian.Tests.FromJinja.CoreTags
{
    public class TestMacro
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
                _Environment.FromString(@"{% macro say_hello(name) %}Hello {{ name }}!{% endmacro %}{{ say_hello('Peter') }}")
            );
            MyAssert.AreEqual("Hello Peter!", template.Render());
        }

        [Test]
        public void TestScoping()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString(@"{% macro level1(data1) %}{% macro level2(data2) %}{{ data1 }}|{{ data2 }}{% endmacro %}{{ level2('bar') }}{% endmacro %}{{ level1('foo') }}")
            );
            MyAssert.AreEqual("foo|bar", template.Render());
        }
        [Test]
        public void TestArguments()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString(@"{% macro m(a, b, c='c', d='d') %}{{ a }}|{{ b }}|{{ c }}|{{ d }}{% endmacro %}{{ m() }}|{{ m('a') }}|{{ m('a', 'b') }}|{{ m(1, 2, 3) }}")
            );
            MyAssert.AreEqual("||c|d|a||c|d|a|b|c|d|1|2|3|d", template.Render());
        }
        [Test]
        public void TestArgumentsDefaultsNonsense()
        {
            throw new TestNotFinishedException();
        }
        [Test]
        public void TestCallerDefaultsNonsense()
        {
            throw new TestNotFinishedException();
        }
        [Test]
        public void TestVarArgs()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString(@"{% macro test() %}{{ varargs|join('|') }}{% endmacro %}{{ test(1, 2, 3) }}")
            );
            MyAssert.AreEqual("1|2|3", template.Render());
        }
        [Test]
        public void TestSimpleCall()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString(@"{% macro test() %}[[{{ caller() }}]]{% endmacro %}{% call test() %}data{% endcall %}")
            );
            MyAssert.AreEqual("[[data]]", template.Render());
        }
        [Test]
        public void TestComplexCall()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString(@"{% macro test() %}[[{{ caller('data') }}]]{% endmacro %}{% call(data) test() %}{{ data }}{% endcall %}")
            );
            MyAssert.AreEqual("[[data]]", template.Render());
        }
        [Test]
        public void TestCallerUndefined()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString(@"{% set caller = 42 %}{% macro test() %}{{ caller is not defined }}{% endmacro %}{{ test() }}")
            );
            MyAssert.AreEqual("True", template.Render());
        }
        [Test]
        public void TestInclude()
        {
            var environment = new JinjaEnvironment(new DictLoader(new Dictionary<string, string>
            {
                { "include", "{% macro test(foo) %}[{{ foo }}]{% endmacro %}" }
            }));
            dynamic template = new DynamicTemplateRenderer(
                environment.FromString("{% from \"include\" import test %}{{ test(\"foo\") }}")
            );
            MyAssert.AreEqual("[foo]", template.Render());
        }

        [Test]
        public void TestMacroAPI()
        {
            throw new TestNotFinishedException();
        }

        [Test]
        public void TestCallSelf()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString(@"{% macro foo(x) %}{{ x }}{% if x > 1 %}|",
                                    "{{ foo(x - 1) }}{% endif %}{% endmacro %}",
                                    "{{ foo(5) }}")
            );
            MyAssert.AreEqual("5|4|3|2|1", template.Render());
        }

        [Test]
        public void TestMacroDefaultsSelfRef()
        {
            throw new TestNotFinishedException();
        }

    }
}
