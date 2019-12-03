using System;
using System.Collections.Generic;
using System.IO;
using ExpressionParser;
using Obsidian.ExpressionParserExt;
using Obsidian.TestCore;

namespace Obsidian.SampleProject
{
    class Program
    {
        static Dictionary<string, object> _Variables = new Dictionary<string, object>
        {
            { "standalone", false },
        };

        static void Main(string[] args)
        {
            TestRunner.Init(TestRunner.TestFileName);
            //AutomaticTest(TestRunner.TestItems["Basic Tests"]["Basic Template"]);
            //AutomaticTest(TestRunner.TestItems["Basic Tests"]["Raw"]);
            //AutomaticTest(TestRunner.TestItems["Basic Tests"]["Inheritance"]);
            //AutomaticTest(TestRunner.TestItems["Macros"]["Basic Macro"]);
            //AutomaticTest(TestRunner.TestItems["Macros"]["Call Macro"]);
            //AutomaticTest(TestRunner.TestItems["Macros"]["Call Macro With Params"]);
            //AutomaticTest(TestRunner.TestItems["Feature Tests"]["Null Master Fallback"]["Standalone"]);
            //AutomaticTest(TestRunner.TestItems["Feature Tests"]["Null Master Fallback"]["Master"]);
            //AutomaticTest(TestRunner.TestItems["Feature Tests"]["For Loop Variables"]);
            //AutomaticTest(TestRunner.TestItems["Feature Tests"]["Set"]);
            //AutomaticTest(TestRunner.TestItems["Other Tests"]["Test1"]);
            //AutomaticTest(TestRunner.TestItems["Other Tests"]["Test2"]);
            //AutomaticTest(TestRunner.TestItems["WhiteSpace"]["Defaults"]);
            //AutomaticTest(TestRunner.TestItems["WhiteSpace"]["TrimBlocks"]);
            //AutomaticTest(TestRunner.TestItems["WhiteSpace"]["LStrip"]);
            //AutomaticTest(TestRunner.TestItems["WhiteSpace"]["LStrip And Trim"]);
            //AutomaticTest(TestRunner.TestItems["WhiteSpace"]["Manual Strip"]);
            AutomaticTest(TestRunner.TestItems["Filters"]["Filters - Basic"]);
            //AutomaticTest(TestRunner.TestItems["Filters"]["Filters - Batch"]);

            //ManualTest(false, false);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Done.");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.ReadKey();
        }


        static void AutomaticTest(Item test, bool outputStartEndMarkers = true)
        {
            TestRunner.TestTemplate(test, out var actualOutput, out var expectedOutput);
            //TestRunner.CheckOriginalText(test, out var actualOutput, out var expectedOutput);


            Console.WriteLine("==================================== ACTUAL =====================================");
            WriteLines(actualOutput);
            Console.WriteLine("=================================== EXPECTED ====================================");
            WriteLines(expectedOutput);
            Console.WriteLine("=================================================================================");
            Console.WriteLine();
            WriteMatchResultInteger("Character Count", actualOutput.Length, expectedOutput.Length);
            WriteMatchResultInteger("     Line Count", LineCount(actualOutput), LineCount(expectedOutput));
            Console.WriteLine();
            WriteMatchResultOverall(actualOutput, expectedOutput);
            Console.WriteLine();
            Console.WriteLine("=================================================================================");


            void WriteLines(string text)
            {
                foreach(var line in text.Split('\n'))
                {
                    Console.WriteLine($"{(outputStartEndMarkers ? "|" : "")}{line}{(outputStartEndMarkers ? "|" : "")}");
                }
            }

            int LineCount(string str)
            {
                return str.Split('\n').Length;
            }

            void WriteMatchResultOverall(string actual, string expected)
            {
                if(actual == expected)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("MATCH!");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("NOT A MATCH!");
                }
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            void WriteMatchResultInteger(string description, int actual, int expected)
            {
                if(actual == expected)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{description} | MATCH | {actual}");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{description} | NOT A MATCH | Actual: {actual} | Expected: {expected}");
                }
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }




        static void ManualTest(bool lstripBlocks, bool trimBlocks)
        {
            Console.WriteLine($"LStripBlocks : {lstripBlocks}");
            Console.WriteLine($"TrimBlocks : {trimBlocks}");
            Console.WriteLine("");
            var loader = new FileSystemLoader(".");
            var environment = new JinjaEnvironment(loader: loader);
            environment.Settings.LStripBlocks = lstripBlocks;
            environment.Settings.TrimBlocks = trimBlocks;
            environment.Settings.DynamicTemplates = true;
            var template = environment.GetTemplate("Template.html", _Variables);

            Console.WriteLine("========================== Standalone: False ==========================");
            Console.WriteLine();
            var result = template.Render(_Variables);
            Console.WriteLine($"{result}");
            Console.WriteLine();
        }
    }
}
