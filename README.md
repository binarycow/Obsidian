### Introduction:

Obsidian is a .NET library for using Jinja2 templates.

- Designed based on the [Jinja 2.10.x documentation](https://jinja.palletsprojects.com/en/2.10.x/). 
- .NET Standard 2.0
- C# 8, using the following features:
  - Nullable Reference Types (with attributes)
  
If you're interested in contribution, please see [Contributing.md](Contributing.md)

### Usage:

##### Render a template, with no variables

```
using Obsidian;
var environment = new JinjaEnvironment
{
    Loader = new FileSystemLoader(searchPath: Directory.GetCurrentDirectory()),
};
var template = environment.GetTemplate("BasicTemplate.html");
Console.WriteLine(template.Render());
```

##### Render a template, with variables

```
var vars = new Dictionary<string, object?>
{
    { "name", "John Smith" },
    { "navigation", 34 }
};

using Obsidian;
var environment = new JinjaEnvironment
{
    Loader = new FileSystemLoader(searchPath: Directory.GetCurrentDirectory()),
};
var template = environment.GetTemplate("BasicTemplate.html");
Console.WriteLine(template.Render(vars));
```

### Version Information:

**Current Version:** 0.0.1

While I am building the core aspects of the library, the public API may not be as stable as I'd like.  If you decide to use this library before then, be aware you may need to adapt to changes.

### Compatibility:

The goal of this project is to have the capability to ingest any Jinja2 compatible template in a .NET project.  The current state of compatibility of this project can be found on [Compatibility.md](Compatibility.md)

To that end, I have implemented the below compatibility testing methodology:

- Python Project `JinjaTestGeneration`
    - Python 3.7, 64-bit
    - Jinja2, version 2.10.3
    - Given input variables and templates, generates output files
- C# Project `Obsidian.Tests`
    - NUnit Test Project
    - Given input variables and templates, generates output files
    - Compares the output from `Obsidian.Tests` and `JinjaTestGeneration` to ensure they provide the same output.
    - *Note:* Currently, Obsidian outputs windows style newlines, while the python library outputs linux style newlines.  The test project currently replaces `\r\n` with `\n` before it performs its comparison.

The test data is located [in a JSON file in the Jinja2Sharp folder](https://github.com/Jinja2Sharp/Jinja2Sharp/blob/master/Jinja2Sharp/TestData/Tests.json).

----

Below is the current test status for the `master` branch:

**Legend:**

- ✅ Passing tests
- ⚠ Failing tests with minor issues
- ⛔ Failing tests
- ℹ Unknown status

| Status | Test Category  | Test Name | Notes |
| :----: | -------------- | --------- | ----- |
| ✅ | Basic Tests | Basic Template |   |
| ✅ | Basic Tests    | Inheritance |  |
| ⛔ | Feature Tests | Null Master Fallback - Standalone |  |
| ⛔ | Feature Tests | Null Master Fallback - Master |  |
