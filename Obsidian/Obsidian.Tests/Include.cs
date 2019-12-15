using NUnit.Framework;
using Obsidian.TestCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Obsidian.Tests
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated public classes", Justification = "<Pending>")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    public class Include : TestClass
    {

        [Test]
        public void Basic()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["Include"]["Basic"]);
        }
        [Test]
        public void IgnoreMissing_ActuallyMissing()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["Include"]["Ignore Missing"]["Actually Missing"]);
        }
        [Test]
        public void IgnoreMissing_ActuallyPresent()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["Include"]["Ignore Missing"]["Actually Present"]);
        }
        [Test]
        public void MultipleTemplates_FirstMissing()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["Include"]["Multiple Templates"]["First Missing"]);
        }
        [Test]
        public void MultipleTemplates_FirstPresent()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["Include"]["Multiple Templates"]["First Present"]);
        }
        [Test]
        public void WithContext()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["Include"]["Context"]["With Context"]);
        }
        [Test]
        public void WithoutContext()
        {
            AssertWrapper.TestTemplate(TestRunner.TestItems["Include"]["Context"]["Without Context"]);
        }
    }
}
