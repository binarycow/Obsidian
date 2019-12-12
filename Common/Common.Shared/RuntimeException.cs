using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class RuntimeException : Exception
    {
        public RuntimeException() : base()
        {

        }
        public RuntimeException(string message) : base(message)
        {
        }

        public RuntimeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
