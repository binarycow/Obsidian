using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Obsidian.Tests
{
    public class NUnitAsserter : IAsserter
    {
        public bool AreEqual(object? expected, object? actual)
        {
            NUnit.Framework.Assert.AreEqual(expected, actual);
            return true;
        }

        public void Mike()
        {
            throw new NotImplementedException();
        }
    }
}
