using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace System
{
    internal static class StringExtensions
    {

        internal static string CapitalizeFirstLetter(this string str)
        {
            if (str.Length == 0) return str;
            if (str.Length == 1) return str.ToUpper(CultureInfo.InvariantCulture);
            return str[0].ToUpper().Concat(str.Substring(1));
        }
        internal static string CapitalizeFirstLetterOfEachWord(this string str)
        {
            return string.Join(" ", str.Split(' ').Select(part => part.CapitalizeFirstLetter()));
        }

        internal static string[] Split(this string str, string seperator)
        {
            return str.Split(new string[] { seperator }, StringSplitOptions.None);
        }
        internal static string WhiteSpaceEscape(this string str)
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

        internal static string HTMLEscape(this string str)
        {
            return string.Join(string.Empty, str.ToCharArray().Select(CharExtensions.HTMLEscape));
        }
    }
}
