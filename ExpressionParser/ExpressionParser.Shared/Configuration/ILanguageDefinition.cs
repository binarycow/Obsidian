using System;
using System.Collections.Generic;
using System.Text;
using ExpressionParser.Lexing;

namespace ExpressionParser.Configuration
{
    public interface ILanguageDefinition
    {
        KeywordDefinition[] Keywords { get; }
        OperatorDefinition[] Operators { get; }
        IDictionary<char,TokenType> SingleCharTokens { get; }

        public bool AllowStringIndexersAsProperties { get; }
    }
}
