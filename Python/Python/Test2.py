
from jinja2 import Environment, TemplateSyntaxError, TemplateRuntimeError, \
     UndefinedError, DictLoader



def test_env():
    env = Environment(loader=DictLoader(dict(
        module='{% macro test() %}[{{ foo }}|{{ bar }}]{% endmacro %}',
        header='[{{ foo }}|{{ 23 }}]',
        o_printer='({{ o }})'
    )))
    env.globals['bar'] = 23
    return env


testEnvironment = test_env()

t = testEnvironment.from_string('{% import "module" as m %}{{ m.test() }}')

print(t.render(foo=42) == '[|23]')