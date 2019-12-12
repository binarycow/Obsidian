
from jinja2 import Environment, TemplateSyntaxError, TemplateRuntimeError, \
     UndefinedError, DictLoader, FileSystemLoader

envArgs = { 
}
env = Environment(
    loader=FileSystemLoader("C:\Source\Obsidian\Python\Python"), **envArgs
)

t = env.from_string("{% for a, b, c, d in [[1, 2, 3]] %}{{ a }}|{{ b }}|{{ c }}{% endfor %}")

results = t.render()
print(results)