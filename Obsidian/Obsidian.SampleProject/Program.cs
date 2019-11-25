using System;
using System.Collections.Generic;
using System.IO;
using ExpressionParser;
using Obsidian.ExpressionParserExt;

namespace Obsidian.SampleProject
{
    public class Item
    {
        public int[] Sequence { get; set; } = new[] { 1, 2, 3, 4, 5 };
    }

    class Program
    {

//        {
//  "a_variable": "Hello, World!",
//  "navigation": [
//    {
//      "href": "www.google.com",
//      "caption": "Google"
//    },
//    {
//      "href": "www.yahoo.com",
//      "caption": "Yahoo"
//    },
//    {
//      "href": "www.bing.com",
//      "caption": "Bing"
//    }
//  ]
//}
        static Dictionary<string, object> _Variables = new Dictionary<string, object>
        {
            { "a_variable", "Hello, World!" },
            { "navigation", new[]
            {
                new
                {
                    href = "www.google.com",
                    caption = "Google"
                },
                new
                {
                    href = "www.yahoo.com",
                    caption = "Yahoo"
                },
                new
                {
                    href = "www.bing.com",
                    caption = "Bing"
                },
            } }
        };

        static void Main(string[] args)
        {
            Test(false, false);
            // Test(true, false);
            // Test(false, true);
            // Test(true, true);


            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Done.");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.ReadKey();
        }


        static void Test(bool lstripBlocks, bool trimBlocks)
        {
            Console.WriteLine($"LStripBlocks : {lstripBlocks}");
            Console.WriteLine($"TrimBlocks : {trimBlocks}");
            Console.WriteLine("");
            var loader = new FileSystemLoader(".");
            var environment = new JinjaEnvironment(loader: loader);
            environment.Settings.LStripBlocks = lstripBlocks;
            environment.Settings.TrimBlocks = trimBlocks;
            var template = environment.GetTemplate("Child.html", _Variables);
            //template.Compile();

            var result = template.Render(_Variables);
            Console.WriteLine($"{result}");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
