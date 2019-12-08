using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obsidian.Exceptions
{
    public class LoaderNotDefinedException : Exception
    {
        internal LoaderNotDefinedException() : base()
        {

        }
        internal LoaderNotDefinedException(string message) : base(message)
        {
        }

        internal LoaderNotDefinedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
