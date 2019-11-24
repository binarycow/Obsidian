using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressionParser.Exceptions
{
    public class ParseException : Exception
    {
        public ParseException(string message, Exception innerException) : base(message, innerException)
        {

        }
        public ParseException(string message) : base(message)
        {

        }
        public ParseException() : base()
        {

        }
    }
}
