using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser.Configuration
{
    public class KeywordDefinition
    {
        public KeywordDefinition(string text)
        {
            Text = text;
        }
        public string Text { get; }
    }
}
