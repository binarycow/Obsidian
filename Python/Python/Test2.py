
from jinja2 import Environment, TemplateSyntaxError, TemplateRuntimeError, \
     UndefinedError, DictLoader, FileSystemLoader

envArgs = { 
}
env = Environment(
    loader=FileSystemLoader("C:\Source\Obsidian\Python\Python"), **envArgs
)

t = env.from_string("{% set caller = 42 %}{% macro test() %}{{ caller is not defined }}{% endmacro %}{{ test() }}")
results = t.render()
print(results)

t = env.from_string("{% set caller = 42 %}{% macro test() %}{{ caller is not defined }}{% endmacro %}{% call test() %}{% endcall %}")
results = t.render()
print(results)