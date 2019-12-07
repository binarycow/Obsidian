using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    internal static class StringExtensions
    {

        public static string[] Split(this string str, string seperator)
        {
            return str.Split(new string[] { seperator }, StringSplitOptions.None);
        }
        public static string WhiteSpaceEscape(this string str)
        {
            var sb = new StringBuilder();
            foreach (var c in str)
            {
                switch (c)
                {
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    case ' ':
                        sb.Append("\\s");
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }
            return sb.ToString();
        }

        public static string HTMLEscape(this string str)
        {
            return string.Join(string.Empty, str.ToCharArray().Select(CharExtensions.HTMLEscape));
        }
    }
}
