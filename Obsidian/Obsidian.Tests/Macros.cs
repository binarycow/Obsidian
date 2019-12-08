using NUnit.Framework;
using Obsidian.TestCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Obsidian.Tests
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "<Pending>")]
    internal class Macros : TestClass
    {

        [Test]
        internal void BasicMacro()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["Macros"]["Basic Macro"]);
        }
        [Test]
        internal void CallMacro()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["Macros"]["Call Macro"]);
        }
        [Test]
        internal void CallMacroWithParams()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["Macros"]["Call Macro With Params"]);
        }
    }
}
