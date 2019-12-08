using System.Collections.Generic;
using Obsidian.TestCore;
using NUnit.Framework;
using System.IO;
using System.Reflection;
using System;

namespace Obsidian.Tests
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "<Pending>")]
    internal class BasicTests : TestClass
    {

        [Test]
        internal void BasicTemplate()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["Basic Tests"]["Basic Template"]);
        }
        [Test]
        internal void Inheritance()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["Basic Tests"]["Inheritance"]);
        }
        [Test]
        internal void Raw()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["Basic Tests"]["Raw"]);
        }
    }
}