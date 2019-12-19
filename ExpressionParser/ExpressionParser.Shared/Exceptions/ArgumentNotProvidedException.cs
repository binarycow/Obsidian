using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser.Exceptions
{
    public class ArgumentNotProvidedException : ArgumentException
    {
        public ArgumentNotProvidedException(string argumentName) : base("Argument not provided", argumentName)
        {

        }
        public ArgumentNotProvidedException(string argumentName, Exception innerException) : base("Argument not provided", argumentName, innerException)
        {

        }

        public ArgumentNotProvidedException() : base()
        {
        }
    }
}
