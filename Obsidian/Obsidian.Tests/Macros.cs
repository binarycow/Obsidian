using NUnit.Framework;
using Obsidian.TestCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Obsidian.Tests
{
    public class Macros : TestClass
    {

        [Test]
        public void BasicMacro()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["Macros"]["Basic Macro"]);
        }
        [Test]
        public void CallMacro()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["Macros"]["Call Macro"]);
        }
        [Test]
        public void CallMacroWithParams()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["Macros"]["Call Macro With Params"]);
        }
    }
}
