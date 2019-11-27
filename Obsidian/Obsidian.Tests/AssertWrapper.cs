﻿using NUnit.Framework;
using Obsidian.TestCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Obsidian.Tests
{
    public static class AssertWrapper
    {
        public static void TestTemplate(Item test)
        {
            TestRunner.TestTemplate(test, out var actualOutput, out var expectedOutput);
            Assert.AreEqual(expectedOutput, actualOutput);
        }
    }
}
