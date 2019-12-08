using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace System
{
    internal static class CharExtensions
    {
        internal static bool IsDigit(this char c)
        {
            return char.IsDigit(c);
        }
        internal static bool IsLetter(this char c)
        {
            return char.IsLetter(c);
        }
        internal static bool IsWhiteSpace(this char c)
        {
            return char.IsWhiteSpace(c);
        }
        internal static char ToUpper(this char c)
        {
            return char.ToUpper(c, CultureInfo.InvariantCulture);
        }
        internal static string Concat(this char c, string strValue)
        {
            return new string(c, 1) + strValue;
        }

        internal static bool IsValidEscapedChar(this char c)
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
        internal static char Escape(this char c)
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
        internal static string HTMLEscape(this char c)
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
