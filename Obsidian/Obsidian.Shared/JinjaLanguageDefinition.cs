using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExpressionParser;
using ExpressionParser.Configuration;
using ExpressionParser.Lexing;

namespace Obsidian
{
    public class JinjaLanguageDefinition : ILanguageDefinition
    {
        public KeywordDefinition[] Keywords => new KeywordDefinition[]
        {
            new ValueKeywordDefinition("True", true),
            new ValueKeywordDefinition("False", false),
            new ValueKeywordDefinition("None", null),
        };

        public OperatorDefinition[] Operators => new OperatorDefinition[]
        {
            OperatorDefinition.CreateMemberAccess(".", 160),
            OperatorDefinition.CreateMethod("(", ")", 160),
            OperatorDefinition.CreateIndex("[", "]", 160),
            OperatorDefinition.CreateBinary("+", 10, OperatorType.Add),
        };

        public IDictionary<char, TokenType> SingleCharTokens => LanguageDefinition.StandardSingleCharacterTokens;

        public bool AllowStringIndexersAsProperties => true;
    }
}
