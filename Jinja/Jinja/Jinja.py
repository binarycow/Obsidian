import json
from pathlib import Path
from jinja2 import Environment, FileSystemLoader, select_autoescape


def GetTestCases(rootPath):
    with open(rootPath.joinpath('Tests.json'), "r") as testFile:
        return json.load(testFile)

def ReadJson(path, filename):
    with open(path.joinpath(filename), "r") as file:
        return json.load(file)

def ProcessCategory(rootPath, category, parentCategories=[]):
    parentCategories.append(category['categoryName'])
    for child in category['children']:
        ProcessTestDictionary(rootPath, child, parentCategories)

def ProcessTest(rootPath, testCase, parentCategories=[]):
    variables = {}
    testCaseRootPath = rootPath.joinpath(testCase['rootPath'])
    if 'variablesFile' in testCase:
        variables = ReadJson(testCaseRootPath, testCase['variablesFile'])

    category = "/".join(parentCategories)

    envArgs = { }
    if 'trim_blocks' in testCase:
        envArgs['trim_blocks'] = testCase['trim_blocks']

    print(f"Rendering Template: {category}/{testCase['testName']} with {len(variables.keys())} variables")
    env = Environment(
        loader=FileSystemLoader(testCaseRootPath.absolute().as_posix()), **envArgs #,
        #autoescape=select_autoescape(['html', 'xml'])
    )
    template = env.get_template(testCase['inputFile'])
    renderedResults = template.render(**variables)
    with open(testCaseRootPath.joinpath(testCase['expectedFile']), "w") as expectedFile:
        expectedFile.write(renderedResults)


def ProcessTestDictionary(rootPath, dictionary, parentCategories=[]):
    if 'categoryName' in dictionary:
        ProcessCategory(rootPath, dictionary, parentCategories)
    else:
        ProcessTest(rootPath, dictionary, parentCategories)



repoRootPath = Path(__file__).parents[0].parents[0].parents[0]
testDataPath = repoRootPath.joinpath("Templating\TestData")
rootPath = Path(testDataPath.absolute().as_posix())
testCases = GetTestCases(rootPath)
for dictionaryItem in testCases:
    ProcessTestDictionary(rootPath, dictionaryItem)
