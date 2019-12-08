using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressionParser.Exceptions
{
    public class LexingException : Exception
    {
        internal LexingException(string message, Exception innerException) : base(message, innerException)
        {

        }
        internal LexingException(string message) : base(message)
        {

        }
        internal LexingException() : base()
        {

        }
    }
}
