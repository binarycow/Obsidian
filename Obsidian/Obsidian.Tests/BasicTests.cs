using System.Collections.Generic;
using Obsidian.TestCore;
using NUnit.Framework;
using System.IO;
using System.Reflection;
using System;

namespace Obsidian.Tests
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated public classes", Justification = "<Pending>")]
    public class BasicTests : TestClass
    {

        [Test]
        public void BasicTemplate()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["Basic Tests"]["Basic Template"]);
        }
        [Test]
        public void Inheritance()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["Basic Tests"]["Inheritance"]);
        }
        [Test]
        public void Raw()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["Basic Tests"]["Raw"]);
        }
    }
}