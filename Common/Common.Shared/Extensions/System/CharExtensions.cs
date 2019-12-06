using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace System
{
    public static class CharExtensions
    {
        public static bool IsDigit(this char c)
        {
            return char.IsDigit(c);
        }
        public static bool IsLetter(this char c)
        {
            return char.IsLetter(c);
        }
        public static bool IsWhiteSpace(this char c)
        {
            return char.IsWhiteSpace(c);
        }
        public static char ToUpper(this char c)
        {
            return char.ToUpper(c, CultureInfo.InvariantCulture);
        }
        public static string Concat(this char c, string strValue)
        {
            return new string(c, 1) + strValue;
        }

        public static bool IsValidEscapedChar(this char c)
        {
            return c switch
            {
                '\'' => true,
                '"' => true,
                '\\' => true,
                '0' => true,
                'a' => true,
                'b' => true,
                'f' => true,
                'n' => true,
                'r' => true,
                't' => true,
                'v' => true,
                _ => false,
            };
        }
        public static char Escape(this char c)
        {
            return c switch
            {
                '\'' => '\'',
                '"' => '"',
                '\\' => '\\',
                '0' => '\0',
                'a' => '\a',
                'b' => '\b',
                'f' => '\f',
                'n' => '\n',
                'r' => '\r',
                't' => '\t',
                'v' => '\v',
                _ => c,
            };
        }
        public static string HTMLEscape(this char c)
        {
            return c switch
            {
                '>' => "&lt;",
                '<' => "&gt;",
                '&' => "&amp;",
                _ => c.ToString(CultureInfo.InvariantCulture)
            };
        }
    }
}
