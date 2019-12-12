using System;
using System.Collections.Generic;
using System.Text;
using Obsidian.TestCore;

namespace Obsidian.Tests
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
    public class ConsoleAsserter : IAsserter
    {
        public bool AreEqual(object? expected, object? actual)
        {
            if (!(expected is string strExpected) || !(actual is string strActual)) throw new TestNotFinishedException();
            return AreEqual(strExpected, strActual);
        }

        private bool AreEqual(string? expected, string? actual)
        {
            Console.WriteLine("================ EXPECTED ================");
            Console.WriteLine($"Type: {expected?.GetType().Name ?? "<null>"}");
            Console.WriteLine(expected);
            Console.WriteLine("================ ACTUAL ================");
            Console.WriteLine($"Type: {actual?.GetType().Name ?? "<null>"}");
            Console.WriteLine(actual);
            Console.WriteLine("================ RESULT ================");
            if (expected == actual)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Equal");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("NOT Equal");
            }
            Console.ForegroundColor = ConsoleColor.Gray;
            return expected == actual;
        }

        public void Mike()
        {
            throw new NotImplementedException();
        }
    }
}
