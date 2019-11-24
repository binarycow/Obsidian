using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Obsidian.Tests.Utilities
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

        public static void TestTemplate(Item test)
        {
            var testInfo = test as Test;
            var inputFile = Path.Combine(TestDataRoot, testInfo.RootPath, testInfo.InputFile);
            var actualFile = Path.Combine(TestDataRoot, testInfo.RootPath, testInfo.ActualFile);
            var expectedOutput = File.ReadAllText(Path.Combine(TestDataRoot, testInfo.RootPath, testInfo.ExpectedFile));
            var variables = new Dictionary<string, object?>();
            if(testInfo.VariablesFile != null)
            {
                variables = VariableCreation.GetVariables(Path.Combine(TestDataRoot, testInfo.RootPath, testInfo.VariablesFile));
            }
            var environment = new JinjaEnvironment
            {
                Loader = new FileSystemLoader(searchPath: Path.GetDirectoryName(inputFile)!)
            };
            environment.Settings.TrimBlocks = testInfo.trim_blocks;
            environment.Settings.LStripBlocks = testInfo.lstrip_blocks;
            var template = environment.GetTemplate(Path.GetFileName(inputFile), variables);
            var actualOutput = template.Render(variables);
            expectedOutput = expectedOutput.Replace("\r\n", "\n");
            File.WriteAllText(actualFile, actualOutput);
            Assert.AreEqual(expectedOutput, actualOutput);
        }


        public static string TestDataRoot => Path.Combine(AssemblyLocation, "..", "..", "..", "..", "TestData");
        public static string TestFileName => Path.Combine(TestDataRoot, "Tests.json");
        public static string AssemblyLocation => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

    }
}
