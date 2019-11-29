using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace Obsidian.TestCore
{
    public static class TestRunner
    {
        public static void Init(string jsonPath)
        {
            TestItems.Clear();
            var items = JsonConvert.DeserializeObject<List<Item>>(File.ReadAllText(jsonPath), new JsonSerializerSettings()
            {
                Converters = { new Converter() }
            });
            foreach(var item in items)
            {
                TestItems.Add(item.Name, item);
            }
        }

        public static Dictionary<string, Item> TestItems = new Dictionary<string, Item>();

        public static void TestTemplate(Item test, out string actualOutput, out string expectedOutput)
        {
            var testInfo = test as Test;
            var inputFile = Path.Combine(TestDataRoot, testInfo.RootPath, testInfo.InputFile);
            var actualFile = Path.Combine(TestDataRoot, testInfo.RootPath, testInfo.ActualFile);
            expectedOutput = File.ReadAllText(Path.Combine(TestDataRoot, testInfo.RootPath, testInfo.ExpectedFile));
            var variables = new Dictionary<string, object?>();
            if (testInfo.VariablesFile != null)
            {
                variables = VariableCreation.GetVariables(Path.Combine(TestDataRoot, testInfo.RootPath, testInfo.VariablesFile));
            }
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

        public static void CheckOriginalText(Item test, out string actualOutput, out string expectedOutput)
        {
            var testInfo = test as Test;
            var inputFile = Path.Combine(TestDataRoot, testInfo.RootPath, testInfo.InputFile);
            var actualFile = Path.Combine(TestDataRoot, testInfo.RootPath, testInfo.ActualFile);
            expectedOutput = File.ReadAllText(inputFile);
            var environment = new JinjaEnvironment
            {
                Loader = new FileSystemLoader(searchPath: Path.GetDirectoryName(inputFile)!)
            };
            actualOutput = environment.CheckOriginalText(Path.GetFileName(inputFile));
        }


        public static string TestDataRoot => Path.Combine(AssemblyLocation, "..", "..", "..", "..", "TestData");
        public static string TestFileName => Path.Combine(TestDataRoot, "Tests.json");
        public static string APIInfo_Expected => Path.Combine(TestDataRoot, "APIInfo_Expected.json");
        public static string APIInfo_Actual => Path.Combine(TestDataRoot, "APIInfo_Actual.json");
        public static string AssemblyLocation => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

    }
}
