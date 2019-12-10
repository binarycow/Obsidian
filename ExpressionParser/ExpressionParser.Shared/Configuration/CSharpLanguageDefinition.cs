using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExpressionParser.Lexing;

namespace ExpressionParser.Configuration
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "<Pending>")]
    internal class CSharpLanguageDefinition : ILanguageDefinition
    {
        internal const string _STRING_TRUE = "true";
        internal const string _STRING_FALSE = "false";
        internal const string _STRING_NULL = "null";


        public IEnumerable<KeywordDefinition> Keywords => new[]
        {
            new ValueKeywordDefinition(true, _STRING_TRUE),
            new ValueKeywordDefinition(false, _STRING_FALSE),
            new ValueKeywordDefinition(null, _STRING_NULL),
        };

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
        public IEnumerable<OperatorDefinition> Operators => new[]
        {
                //MemberAccessOperatorDefinition.Create(".", 160),
                //FunctionCallOperatorDefinition.Create("(", ")", 160, FunctionCallOperatorDefinition.FunctionCallType.Method),
                //FunctionCallOperatorDefinition.Create("[", "]", 160, FunctionCallOperatorDefinition.FunctionCallType.Index),

                //OperatorDefinition.CreateUnary("+", 150, OperatorType.UnaryPlus, OperatorFix.Prefix),
                //OperatorDefinition.CreateUnary("-", 150, OperatorType.UnaryNegate, OperatorFix.Prefix),
                //OperatorDefinition.CreateUnary("!", 150, OperatorType.LogicalNot, OperatorFix.Prefix),
                //OperatorDefinition.CreateUnary("~", 150, OperatorType.BitwiseNot, OperatorFix.Prefix),
                //OperatorDefinition.CreateUnary("++", 150, OperatorType.Increment, OperatorFix.Prefix),
                //OperatorDefinition.CreateUnary("--", 150, OperatorType.Decrement, OperatorFix.Prefix),
                //OperatorDefinition.CreateUnary("^", 150, OperatorType.IndexFromEnd, OperatorFix.Prefix),

                OperatorDefinition.CreateBinary("..", 140, OperatorType.Range),

                OperatorDefinition.CreateBinary("*", 130, OperatorType.Multiply),
                OperatorDefinition.CreateBinary("/", 130, OperatorType.Divide),
                OperatorDefinition.CreateBinary("%", 130, OperatorType.Modulo),

                OperatorDefinition.CreateBinary("+", 120, OperatorType.Add),
                OperatorDefinition.CreateBinary("-", 120, OperatorType.Subtract),

                OperatorDefinition.CreateBinary("<<", 110, OperatorType.LeftShift),
                OperatorDefinition.CreateBinary(">>", 110, OperatorType.RightShift),

                OperatorDefinition.CreateBinary("<", 100, OperatorType.LessThan),
                OperatorDefinition.CreateBinary(">", 100, OperatorType.GreaterThan),
                OperatorDefinition.CreateBinary("<=", 100, OperatorType.LessThanOrEqual),
                OperatorDefinition.CreateBinary(">=", 100, OperatorType.GreaterThanOrEqual),
                OperatorDefinition.CreateBinary("is", 100, OperatorType.Is),
                OperatorDefinition.CreateBinary("as", 100, OperatorType.As),

                OperatorDefinition.CreateBinary("==", 90, OperatorType.Equal),
                OperatorDefinition.CreateBinary("!=", 90, OperatorType.NotEqual),
                OperatorDefinition.CreateBinary("&", 80, OperatorType.BitwiseAnd),
                OperatorDefinition.CreateBinary("^", 70, OperatorType.ExclusiveOr),
                OperatorDefinition.CreateBinary("|", 60, OperatorType.BitwiseOr),
                OperatorDefinition.CreateBinary("&&", 50, OperatorType.LogicalAnd),
                OperatorDefinition.CreateBinary("||", 40, OperatorType.LogicalOr),
                OperatorDefinition.CreateBinary("??", 30, OperatorType.NullCoalesce),
        };

        public IDictionary<char, TokenType> SingleCharTokens => LanguageDefinition.StandardSingleCharacterTokens;

        public bool AllowStringIndexersAsProperties => false;

        public IEnumerable<UserDefinedFunction> Functions => Enumerable.Empty<UserDefinedFunction>();

        IEnumerable<ScopedUserDefinedFunction> ILanguageDefinition.ScopedFunctions => throw new NotImplementedException();
    }
}
