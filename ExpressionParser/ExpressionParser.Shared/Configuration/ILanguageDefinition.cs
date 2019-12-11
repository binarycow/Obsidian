using System;
using System.Collections.Generic;
using System.Text;
using ExpressionParser.Lexing;

namespace ExpressionParser.Configuration
{
    public interface ILanguageDefinition
    {
        IEnumerable<KeywordDefinition> Keywords { get; }
        IEnumerable<OperatorDefinition> Operators { get; }
        IDictionary<char,TokenType> SingleCharTokens { get; }
        bool AllowStringIndexersAsProperties { get; }
        bool RequireNonDefaultArguments { get; }
        IEnumerable<UserDefinedFunction> Functions { get; }
        internal IEnumerable<ScopedUserDefinedFunction> ScopedFunctions { get; }
    }
}
