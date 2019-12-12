using System;
using System.Collections.Generic;
using System.Text;

namespace Obsidian.Tests
{
    public interface IAsserter
    {
        public bool AreEqual(object? expected, object? actual);
        public void Mike();
    }
}
