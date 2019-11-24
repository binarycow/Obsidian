using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressionParser.Exceptions
{
    public class LexingException : Exception
    {
        public LexingException(string message, Exception innerException) : base(message, innerException)
        {

        }
        public LexingException(string message) : base(message)
        {

        }
        public LexingException() : base()
        {

        }
    }
}
