import json
from pathlib import Path
from jinja2 import Environment, FileSystemLoader, select_autoescape



class Person:
    name = "John Smith"

    def getName(self):
        return "Jacob Smith"

dict = {
    "D": 68,
    "c": 67,
    "F": 70,
    "b": 66,
    "A": 65,
    "e": 69
};


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
        if(testCase['variablesFile'] != ''):
            variables = ReadJson(testCaseRootPath, testCase['variablesFile'])

    category = "/".join(parentCategories)

    envArgs = { }
    for attr in ['lstrip_blocks', 'trim_blocks']:
        if attr in testCase:
            envArgs[attr] = testCase[attr]

    print(f"Rendering Template: {category}/{testCase['testName']} with {len(variables.keys())} variables")
    env = Environment(
        loader=FileSystemLoader(testCaseRootPath.absolute().as_posix()), **envArgs #,
        #autoescape=select_autoescape(['html', 'xml'])
    )
    template = env.get_template(testCase['inputFile'])


    renderedResults = template.render(person = Person(), dict = dict, **variables)
    with open(testCaseRootPath.joinpath(testCase['expectedFile']), "w") as expectedFile:
        expectedFile.write(renderedResults)


def ProcessTestDictionary(rootPath, dictionary, parentCategories=[]):
    if 'categoryName' in dictionary:
        ProcessCategory(rootPath, dictionary, parentCategories)
    else:
        ProcessTest(rootPath, dictionary, parentCategories)



repoRootPath = Path(__file__).parents[0].parents[0].parents[0]
testDataPath = repoRootPath.joinpath("Obsidian\TestData")
rootPath = Path(testDataPath.absolute().as_posix())
testCases = GetTestCases(rootPath)
for dictionaryItem in testCases:
    ProcessTestDictionary(rootPath, dictionaryItem)
