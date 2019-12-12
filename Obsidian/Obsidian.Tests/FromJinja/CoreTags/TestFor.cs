using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Obsidian.TestCore;
using static Obsidian.Tests.AssertConfig;

namespace Obsidian.Tests.FromJinja.CoreTags
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]

    public class TestFor
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
        private readonly object[] _RecursiveVars = new[]
        {
            new Dictionary<string, object>
            {
                { "a", 1 },
                { "b", new []{
                    new Dictionary<string, object> { { "a", 1 } },
                    new Dictionary<string, object> { { "a", 2 } },
                } }
            },
            new Dictionary<string, object>
            {
                { "a", 2 },
                { "b", new []{
                    new Dictionary<string, object> { { "a", 1 } },
                    new Dictionary<string, object> { { "a", 2 } },
                } }
            },
            new Dictionary<string, object>
            {
                { "a", 3 },
                { "b", new []{
                    new Dictionary<string, object> { { "a", "a" } },
                } }
            },
        };


        [Test]
        public void TestSimple()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString("{% for item in seq %}{{ item }}{% endfor %}")
            );
            MyAssert.AreEqual("0123456789", template.Render(seq: Enumerable.Range(0, 10).ToList()));
        }

        [Test]
        public void TestElse()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString("{% for item in seq %}XXX{% else %}...{% endfor %}")
            );
            MyAssert.AreEqual("...", template.Render());
        }

        [Test]
        public void TestElseScopingItem()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString("{% for item in [] %}{% else %}{{ item }}{% endfor %}")
            );
            MyAssert.AreEqual("42", template.Render(item: 42));
        }
        [Test]
        public void TestEmptyBlocks()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString("<{% for item in seq %}{% else %}{% endfor %}>")
            );
            MyAssert.AreEqual("<>", template.Render());
        }
        [Test]
        public void TestContextVars()
        {
            throw new TestNotFinishedException(@"
        slist = [42, 24]
        for seq in [slist, iter(slist), reversed(slist), (_ for _ in slist)]:
            tmpl = env.from_string('''{% for item in seq -%}
            {{ loop.index }}|{{ loop.index0 }}|{{ loop.revindex }}|{{
                loop.revindex0 }}|{{ loop.first }}|{{ loop.last }}|{{
               loop.length }}###{% endfor %}''')
            one, two, _ = tmpl.render(seq=seq).split('###')
            (one_index, one_index0, one_revindex, one_revindex0, one_first,
             one_last, one_length) = one.split('|')
            (two_index, two_index0, two_revindex, two_revindex0, two_first,
             two_last, two_length) = two.split('|')

            MyAssert int(one_index) == 1 and int(two_index) == 2
            MyAssert int(one_index0) == 0 and int(two_index0) == 1
            MyAssert int(one_revindex) == 2 and int(two_revindex) == 1
            MyAssert int(one_revindex0) == 1 and int(two_revindex0) == 0
            MyAssert one_first == 'True' and two_first == 'False'
            MyAssert one_last == 'False' and two_last == 'True'
            MyAssert one_length == two_length == '2'");

        }
        [Test]
        public void TestCycling()
        {
            throw new TestNotFinishedException(@"
        tmpl = env.from_string('''{% for item in seq %}{{
            loop.cycle('<1>', '<2>') }}{% endfor %}{%
            for item in seq %}{{ loop.cycle(*through) }}{% endfor %}''')
        output = tmpl.render(seq=list(range(4)), through=('<1>', '<2>'))
        MyAssert output == '<1><2>' * 4
");

        }
        [Test]
        public void TestLookaround()
        {
            throw new TestNotFinishedException(@"
        tmpl = env.from_string('''{% for item in seq -%}
            {{ loop.previtem|default('x') }}-{{ item }}-{{
            loop.nextitem|default('x') }}|
        {%- endfor %}''')
        output = tmpl.render(seq=list(range(4)))
        MyAssert output == 'x-0-1|0-1-2|1-2-3|2-3-x|'");
        }
        [Test]
        public void TestChanged()
        {
            throw new TestNotFinishedException(@"
        tmpl = env.from_string('''{% for item in seq -%}
            {{ loop.changed(item) }},
        {%- endfor %}''')
        output = tmpl.render(seq=[None, None, 1, 2, 2, 3, 4, 4, 4])
        MyAssert output == 'True,False,True,True,False,True,True,False,False,'
");
        }

        [Test]
        public void TestScope()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString("{% for item in seq %}{% endfor %}{{ item }}")
            );
            MyAssert.AreEqual(string.Empty, template.Render(seq: Enumerable.Range(0, 10).ToList()));
        }
        [Test]
        public void TestVarLen()
        {
            static IEnumerable<int> Iter()
            {
                for (var i = 0; i < 5; ++i)
                    yield return i;
            }
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString("{% for item in iter %}{{ item }}{% endfor %}")
            );
            MyAssert.AreEqual("01234", template.Render(iter: Iter()));
        }
        [Test]
        public void TestNonIter()
        {
            throw new TestNotFinishedException(@"
        tmpl = env.from_string('{% for item in none %}...{% endfor %}')
        pytest.raises(TypeError, tmpl.render)");
        }

        [Test]
        public void TestRecursive()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString(@"{% for item in seq recursive -%}
            [{{ item.a }}{% if item.b %}<{{ loop(item.b) }}>{% endif %}]
        {%- endfor %}")
            );
            MyAssert.AreEqual("[1<[1][2]>][2<[1][2]>][3<[a]>]", template.Render(seq: _RecursiveVars));
        }


        [Test]
        public void TestRecursiveLookaround()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString(@"{% for item in seq recursive -%}
            [{{ loop.previtem.a if loop.previtem is defined else 'x' }}.{{
            item.a }}.{{ loop.nextitem.a if loop.nextitem is defined else 'x'
            }}{% if item.b %}<{{ loop(item.b) }}>{% endif %}]
        {%- endfor %}")
            );
            MyAssert.AreEqual("[x.1.2<[x.1.2][1.2.x]>][1.2.3<[x.1.2][1.2.x]>][2.3.x<[x.a.x]>]", template.Render(seq: _RecursiveVars));
        }


        [Test]
        public void TestRecursiveDepth0()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString(@"{% for item in seq recursive -%}
            [{{ loop.depth0 }}:{{ item.a }}{% if item.b %}<{{ loop(item.b) }}>{% endif %}]
        {%- endfor %}")
            );
            MyAssert.AreEqual("[0:1<[1:1][1:2]>][0:2<[1:1][1:2]>][0:3<[1:a]>]", template.Render(seq: _RecursiveVars));
        }

        [Test]
        public void TestRecursiveDepth()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString(@"{% for item in seq recursive -%}
            [{{ loop.depth }}:{{ item.a }}{% if item.b %}<{{ loop(item.b) }}>{% endif %}]
        {%- endfor %}")
            );
            MyAssert.AreEqual("[0:1<[1:1][1:2]>][0:2<[1:1][1:2]>][0:3<[1:a]>]", template.Render(seq: _RecursiveVars));
        }



        [Test]
        public void TestLoopLoop()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString(@"{% for row in table %}
            {%- set rowloop = loop -%}
            {% for cell in row -%}
                [{{ rowloop.index }}|{{ loop.index }}]
            {%- endfor %}
        {%- endfor %}")
            );
            MyAssert.AreEqual("[1|1][1|2][2|1][2|2]", template.Render(table: new[] { "ab", "cd" }));
        }

        [Test]
        public void TestLoopErrors()
        {
            throw new TestNotFinishedException(@"
    def test_loop_errors(self, env):
        tmpl = env.from_string('''{% for item in [1] if loop.index
                                      == 0 %}...{% endfor %}''')
        pytest.raises(UndefinedError, tmpl.render)
        tmpl = env.from_string('''{% for item in [] %}...{% else
            %}{{ loop }}{% endfor %}''')
        MyAssert tmpl.render() == ''
");
        }


        [Test]
        public void TestLoopFilter()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString("{% for item in range(10) if item ",
                "is even %}[{{ item }}]{% endfor %}")
            );
            MyAssert.AreEqual("[0][2][4][6][8]", template.Render());
            template = new DynamicTemplateRenderer(
                 _Environment.FromString(@"
            {%- for item in range(10) if item is even %}[{{
                loop.index }}:{{ item }}]{% endfor %}")
             );
            MyAssert.AreEqual("[1:0][2:2][3:4][4:6][5:8]", template.Render());
        }
        [Test]
        public void TestLoopUnassignable()
        {
            throw new TestNotFinishedException(@"
        pytest.raises(TemplateSyntaxError, env.from_string,
                      '{% for loop in seq %}...{% endfor %}')");
        }
        [Test]
        public void TestScopedSpecialVar()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString("{% for s in seq %}[{{ loop.first }}{% for c in s %}",
                "|{{ loop.first }}{% endfor %}]{% endfor %}")
            );
            MyAssert.AreEqual("[True|True|False][False|True|False]", template.Render(seq: Tuple.Create("ab", "cd")));
        }
        [Test]
        public void TestScopedLoopVar()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString("{% for x in seq %}{{ loop.first }}",
                "{% for y in seq %}{% endfor %}{% endfor %}")
            );
            MyAssert.AreEqual("TrueFalse", template.Render(seq: "ab"));
            template = new DynamicTemplateRenderer(
                 _Environment.FromString("{% for x in seq %}{% for y in seq %}",
                 "{{ loop.first }}{% endfor %}{% endfor %}")
             );
            MyAssert.AreEqual("TrueFalseTrueFalse", template.Render(seq: "ab"));
        }
        [Test]
        public void TestRecursiveEmptyLoop()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString("{%- for item in foo recursive -%}{%- endfor -%}")
            );
            MyAssert.AreEqual(string.Empty, template.Render(foo: Array.Empty<int>()));
        }

        [Test]
        public void TestCallInLoop()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString(@"
        {%- macro do_something() -%}
            [{{ caller() }}]
        {%- endmacro %}

        {%- for i in [1, 2, 3] %}
            {%- call do_something() -%}
                {{ i }}
            {%- endcall %}
        {%- endfor -%}
")
            );
            MyAssert.AreEqual("[1][2][3]", template.Render());
        }

        [Test]
        public void TestScopingBug()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString(@"
{%- for item in foo %}...{{ item }}...{% endfor %}
        {%- macro item(a) %}...{{ a }}...{% endmacro %}
        {{- item(2) -}}
")
            );
            MyAssert.AreEqual("...1......2...", template.Render(foo: Tuple.Create(1)));
        }
        [Test]
        public void TestUnpacking()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString("{% for a, b, c in [[1, 2, 3]] %}",
                "{{ a }}|{{ b }}|{{ c }}{% endfor %}")
            );
            MyAssert.AreEqual("1|2|3", template.Render());
        }
        [Test]
        public void IntendedScopingWithSet()
        {
            dynamic template = new DynamicTemplateRenderer(
                _Environment.FromString("{% for item in seq %}{{ x }}",
                    "{% set x = item %}{{ x }}{% endfor %}")
            );
            MyAssert.AreEqual("010203", template.Render(x: 0, seq: new[] { 1, 2, 3 }));
            template = new DynamicTemplateRenderer(
                _Environment.FromString("{% set x = 9 %}{% for item in seq %}{{ x }}",
                    "{% set x = item %}{{ x }}{% endfor %}")
            );
            MyAssert.AreEqual("919293", template.Render(x: 0, seq: new[] { 1, 2, 3 }));
        }

    }
}
