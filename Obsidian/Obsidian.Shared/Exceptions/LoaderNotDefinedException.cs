using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obsidian.Exceptions
{
    public class LoaderNotDefinedException : Exception
    {
        public LoaderNotDefinedException() : base()
        {

        }
        public LoaderNotDefinedException(string message) : base(message)
        {
        }

        public LoaderNotDefinedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
