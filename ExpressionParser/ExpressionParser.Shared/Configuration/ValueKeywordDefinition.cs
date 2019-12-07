using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser.Configuration
{
    internal class ValueKeywordDefinition : KeywordDefinition
    {
        public ValueKeywordDefinition(object? value, params string[] names) : base(names)
        {
            Value = value;
        }
        public object? Value { get; }
    }
}
