using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Obsidian.TestCore;
using NUnit.Framework;

namespace Obsidian.Tests
{
    public abstract class TestClass
    {
        [SetUp]
        public void Init()
        {
            TestRunner.Init(TestRunner.TestFileName);
        }
    }
}
