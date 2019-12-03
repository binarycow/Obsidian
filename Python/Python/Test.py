import json
from pathlib import Path
from jinja2 import Environment, FileSystemLoader, select_autoescape


envArgs = { 
    'trim_blocks': True
}
variables = {
    'seq': [ 1, 2, 3 ]
}

env = Environment(
    loader=FileSystemLoader("C:\Source\Obsidian\Python\Python"), **envArgs #,
    #autoescape=select_autoescape(['html', 'xml'])
)
template = env.get_template("Test.txt")
output = template.render(**variables)
lines = output.splitlines()

for line in lines:
    #print("|" + line + "|")
    print(line)