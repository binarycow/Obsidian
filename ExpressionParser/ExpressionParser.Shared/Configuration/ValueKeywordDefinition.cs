using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser.Configuration
{
    public class ValueKeywordDefinition : KeywordDefinition
    {
        public ValueKeywordDefinition(string text, object? value) : base(text)
        {
            Value = value;
        }
        public object? Value { get; }
    }
}
