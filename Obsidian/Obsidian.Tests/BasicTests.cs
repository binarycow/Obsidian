using System.Collections.Generic;
using Obsidian.Tests.Utilities;
using NUnit.Framework;
using System.IO;
using System.Reflection;
using System;

namespace Obsidian.Tests
{
    public class BasicTests : TestClass
    {

        [Test]
        public void BasicTemplate()
        {
            TestRunner.TestTemplate(TestRunner.TestItems["Basic Tests"]["Basic Template"]);
        }
        [Test]
        public void Inheritance()
        {
            TestRunner.TestTemplate(TestRunner.TestItems["Basic Tests"]["Inheritance"]);
        }
    }
}