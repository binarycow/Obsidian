using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Obsidian.TestCore
{
    public static class TestRunner
    {
        public static void Init(string jsonFilename, string? rootPath = null)
        {
            if (rootPath != null) TestDataRoot = rootPath;

            TestItems.Clear();
            var items = JsonConvert.DeserializeObject<List<Item>>(File.ReadAllText(Path.Combine(TestDataRoot, jsonFilename)), new JsonSerializerSettings()
            {
                Converters = { new Converter() }
            });
            if (items == null) return;
            foreach(var item in items)
            {
                TestItems.Add(item.Name, item);
            }
        }

        public static void Save(string jsonFilename)
        {
            var json = JsonConvert.SerializeObject(TestItems.Values, new JsonSerializerSettings()
            {
                Converters = { new Converter() },
                DefaultValueHandling = DefaultValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            json = JValue.Parse(json).ToString(Formatting.Indented);
            File.WriteAllText(Path.Combine(TestDataRoot, jsonFilename), json);
        }


        public static Dictionary<string, Item> TestItems { get; } = new Dictionary<string, Item>();

        public static void TestTemplate(Item test, out string actualOutput, out string expectedOutput)
        {
            TestTemplate(test, out actualOutput, out expectedOutput, out _);
        }
        public static void TestTemplate(Item test, out string actualOutput, out string expectedOutput, out string templateText)
        {
            test = test ?? throw new ArgumentNullException(nameof(test));
            if(!(test is Test testInfo))
            {
                throw new ArgumentException($"Item provided is of type {test.GetType().Name}, which is not {nameof(Test)}", nameof(test));
            }
            var inputFile = Path.Combine(TestDataRoot, testInfo.RootPath, testInfo.InputFile);
            var actualFile = Path.Combine(TestDataRoot, testInfo.RootPath, testInfo.ActualFile);
            expectedOutput = File.ReadAllText(Path.Combine(TestDataRoot, testInfo.RootPath, testInfo.ExpectedFile));
            var variables = new Dictionary<string, object?>();
            if (string.IsNullOrWhiteSpace(testInfo.VariablesFile) == false)
            {
                variables = VariableCreation.GetVariables(Path.Combine(TestDataRoot, testInfo.RootPath, testInfo.VariablesFile));
            }
            variables.Add("person", new Person());
            variables.Add("dict", new Dictionary<string, int>
            {
                { "D", 68 },
                { "c", 67 },
                { "F", 70 },
                { "b", 66 },
                { "A", 65 },
                { "e", 69 },
            });

            templateText = File.ReadAllText(Path.Combine(Path.GetDirectoryName(inputFile), Path.GetFileName(inputFile)));

            var environment = new JinjaEnvironment
            {
                Loader = new FileSystemLoader(searchPath: Path.GetDirectoryName(inputFile)!)
            };
            environment.Settings.TrimBlocks = testInfo.trim_blocks;
            environment.Settings.LStripBlocks = testInfo.lstrip_blocks;
            environment.Settings.DynamicTemplates = true;
            var template = environment.GetTemplate(Path.GetFileName(inputFile), variables);
            actualOutput = template.Render(variables);
            expectedOutput = expectedOutput.Replace("\r\n", "\n");
            File.WriteAllText(actualFile, actualOutput);
        }

        public static string TestDataRoot { get; set; } = Path.Combine(AssemblyLocation, "..", "..", "..", "..", "TestData");
        public static string TestFileName => "Tests.json";
        public static string AssemblyLocation => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

    }
}
