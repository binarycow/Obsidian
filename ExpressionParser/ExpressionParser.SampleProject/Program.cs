using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ExpressionParser.Scopes;

namespace ExpressionParser.SampleProject
{
    class Program
    {


        //private static ExpressionEval _Eval = new ExpressionEval(LanguageDefinition.CSharp);
        //private static string _Expression = "a + b";
        //private static Dictionary<string, object> _Variables = new Dictionary<string, object>
        //{
        //    {"a", 5 },
        //    {"b", 10 },
        //};
        //private static Dictionary<string, object> _CompileVars = new Dictionary<string, object>
        //{
        //    {"a", 1 },
        //    {"b", 2 },
        //};
        //private static int _Count = 1;
        //private static TestData Test(string name, bool compiled)
        //{
        //    var stopwatch = Stopwatch.StartNew();
        //    ExpressionData compiledExpression = null!;
        //    Scope scope = null!;
        //    if (compiled)
        //        compiledExpression = _Eval.Compile(_Expression, _CompileVars, out scope);
        //    else
        //        compiledExpression = _Eval.Dynamic(_Expression, _CompileVars, out scope);
        //    stopwatch.Stop();
        //    scope.ResetVariables(_Variables);
        //    var compiledTime = stopwatch.Elapsed.Ticks;
        //    var trials = new long[_Count];
        //    for (int i = 0; i < _Count; ++i)
        //    {
        //        stopwatch.Start();
        //        var result = (int)compiledExpression.Evaluate(scope);
        //        stopwatch.Stop();
        //        trials[i] = stopwatch.Elapsed.Ticks;
        //        if (result != 15)
        //        {
        //            throw new NotImplementedException();
        //        }
        //    }
        //    return new TestData
        //    {
        //        CompileTime = compiledTime,
        //        Name = name,
        //        Trials = trials
        //    };
        //}
        //private static TestData TestCompiled()
        //{
        //    return Test("Compiled", true);
        //}
        //private static TestData TestDynamic()
        //{
        //    return Test("Dynamic", false);
        //}



        static void Main()
        {
            //var testResults = new[] { TestCompiled(), TestDynamic() };
            //foreach(var testResult in testResults)
            //{
            //    Console.WriteLine($"\t\t{testResult.Name}");
            //    Console.WriteLine("==============================================================");
            //    Console.WriteLine($"");


            //    Console.WriteLine($"{"Trials".PadLeft(20, ' ')} : {testResult.Trials.Length}");
            //    Console.WriteLine($"{"Compile Time".PadLeft(20, ' ')} : {FormatTicks(testResult.CompileTime)}");
            //    Console.WriteLine($"{"Average Time".PadLeft(20, ' ')} : {FormatTicks(testResult.Trials.Sum() / testResult.Trials.Length)}");
            //    Console.WriteLine($"{"Total Time".PadLeft(20, ' ')} : {FormatTicks(testResult.Trials.Sum())}");

            //    //for (int i=0;i<testResult.Trials.Length;++i)
            //    //{
            //    //    Console.WriteLine($"{$"Trial # {i + 1}".PadLeft(20,' ')} : {FormatTicks(testResult.Trials[i])}");
            //    //}
            //    Console.WriteLine();
            //    Console.WriteLine();
            //    Console.WriteLine();
            //}
            Console.ReadKey();
        }

        static string FormatTicks(long ticks)
        {
            return new TimeSpan(ticks).ToString();
            //return ToPrettyFormat(new TimeSpan(ticks));
        }
    }
    public class TestData
    {
        public string Name { get; set; }
        public long CompileTime { get; set; }
        public long[] Trials { get; set; }
    }

}
