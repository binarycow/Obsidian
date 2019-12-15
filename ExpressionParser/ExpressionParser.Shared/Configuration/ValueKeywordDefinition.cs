using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser.Configuration
{
    internal class ValueKeywordDefinition : KeywordDefinition
    {
        internal ValueKeywordDefinition(object? value, params string[] names) : base(names)
        {
            Value = value;
        }
        internal object? Value { get; }
    }
}
