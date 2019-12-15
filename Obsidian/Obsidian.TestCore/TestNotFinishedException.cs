using System;
using System.Collections.Generic;
using System.Text;

namespace Obsidian.TestCore
{
    public class TestNotFinishedException : Exception
    {
        public TestNotFinishedException() : base()
        {

        }
        public TestNotFinishedException(string message) : base(message)
        {
        }

        public TestNotFinishedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
