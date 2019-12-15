import json
from pathlib import Path
from jinja2 import Environment, FileSystemLoader, select_autoescape




class Person:
    name = "John Smith"

    def getName(self):
        return "Jacob Smith"

envArgs = { 
    #'trim_blocks': True
}
variables = {
    'seq': [ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 ],
    'dict': {
        'D': 68,
        'c': 67,
        'F': 70,
        'b': 66,
        'A': 65,
        'e': 69,
    },
    'person': Person()
}

env = Environment(
    loader=FileSystemLoader("C:\Source\Obsidian\Python\Python"), **envArgs #,
    #autoescape=select_autoescape(['html', 'xml'])
)
template = env.get_template("template.html")
output = template.render(**variables)
lines = output.splitlines()

for line in lines:
    print("|" + line + "|")