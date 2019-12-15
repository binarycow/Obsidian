using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Obsidian.TestCore;

namespace Obsidian.Tests
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated public classes", Justification = "<Pending>")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    public class Import : TestClass
    {

        [Test]
        public void Complete()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["Import"]["Complete"]);
        }

        [Test]
        public void Simple()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["Import"]["Simple"]);
        }
    }
}
