
from jinja2 import Environment, TemplateSyntaxError, TemplateRuntimeError, \
     UndefinedError, DictLoader, FileSystemLoader

envArgs = { 
}
env = Environment(
    loader=FileSystemLoader("C:\Source\Obsidian\Python\Python"), **envArgs
)

t = env.from_string('''<ul class="sitemap">
{%- for item in sitemap recursive if item.title != "Google" %}
    <li><a href="{{ item.href|e }}">{{ item.title }}</a>
    {%- if item.children -%}
        <ul class="submenu">{{ loop(item.children) }}</ul>
    {%- endif %}</li>
{%- endfor %}
</ul>''')

sitemap = [
    {
        "href": "https://www.google.com",
        "title": "Google",
        "children": [
            {
                "href": "https://mail.google.com",
                "title": "Gmail"
            },
            {
                "href": "https://hangouts.google.com",
                "title": "Hangouts"
            }
        ]
    },
    {
        "href": "https://www.reddit.com",
        "title": "Reddit",
        "children": [
            {
                "href": "https://reddit.com/r/networking",
                "title": "Networking"
            },
            {
                "href": "https://reddit.com/r/sysadmin",
                "title": "Sysadmin"
            }
        ]
    }
]

results = t.render(sitemap = sitemap)
print(results)