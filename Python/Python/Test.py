import json
from pathlib import Path
from jinja2 import Environment, FileSystemLoader, select_autoescape


envArgs = { }
variables = { 'standalone': False }

env = Environment(
    loader=FileSystemLoader("C:\Source\Obsidian\Obsidian\Obsidian.SampleProject"), **envArgs #,
    #autoescape=select_autoescape(['html', 'xml'])
)
template = env.get_template("NullMasterChild.html")

print("========================== Standalone: False ==========================")
print()
print(template.render(**variables))
print()
print("========================== Standalone: True ===========================")
print()
variables['standalone'] = True
print(template.render(**variables))
print()
print("=======================================================================")