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
        internal const string OPERATOR_SQUARE_BRACE_OPEN = "[";
        internal const string OPERATOR_PAREN_OPEN = "(";


        public KeywordDefinition[] Keywords => new KeywordDefinition[]
        {
            new ValueKeywordDefinition("True", true),
            new ValueKeywordDefinition("False", false),
            new ValueKeywordDefinition("None", null),
        };

        public OperatorDefinition[] Operators => new OperatorDefinition[]
        {
            OperatorDefinition.CreateMemberAccess(".", 160),
            OperatorDefinition.CreatePipeline("|", 160),
            OperatorDefinition.CreateMethod(OPERATOR_PAREN_OPEN, TokenType.Comma, TokenType.Paren_Close, 160),
            OperatorDefinition.CreateIndex(OPERATOR_SQUARE_BRACE_OPEN, TokenType.Comma, TokenType.SquareBrace_Close, 160),

            OperatorDefinition.CreateUnary("**", 80, OperatorType.Power),

            OperatorDefinition.CreateUnary("+", 70, OperatorType.UnaryPlus),
            OperatorDefinition.CreateUnary("-", 70, OperatorType.Negate),

            OperatorDefinition.CreateBinary("*", 60, OperatorType.Multiply),
            OperatorDefinition.CreateBinary("/", 60, OperatorType.DivideFloat),
            OperatorDefinition.CreateBinary("//", 60, OperatorType.DivideInteger),
            OperatorDefinition.CreateBinary("*", 60, OperatorType.Modulo),

            OperatorDefinition.CreateBinary("+", 50, OperatorType.Add),
            OperatorDefinition.CreateBinary("-", 50, OperatorType.Subtract),

            OperatorDefinition.CreateBinary("in", 40, OperatorType.In),
            OperatorDefinition.CreateBinary("not in", 40, OperatorType.NotIn),
            OperatorDefinition.CreateBinary("is", 40, OperatorType.Is),
            OperatorDefinition.CreateBinary("is not", 40, OperatorType.IsNot),
            OperatorDefinition.CreateBinary("<", 40, OperatorType.LessThan),
            OperatorDefinition.CreateBinary(">", 40, OperatorType.GreaterThan),
            OperatorDefinition.CreateBinary("<=", 40, OperatorType.LessThanOrEqual),
            OperatorDefinition.CreateBinary(">=", 40, OperatorType.GreaterThanOrEqual),
            OperatorDefinition.CreateBinary("!=", 40, OperatorType.Equal),
            OperatorDefinition.CreateBinary("==", 40, OperatorType.NotEqual),


            OperatorDefinition.CreateUnary("not", 30, OperatorType.LogicalNot),
            OperatorDefinition.CreateBinary("and", 20, OperatorType.LogicalAnd),
            OperatorDefinition.CreateBinary("or", 10, OperatorType.LogicalOr),

            OperatorDefinition.CreateBinary("=", 0, OperatorType.Assign),


        };

        public IDictionary<char, TokenType> SingleCharTokens => LanguageDefinition.StandardSingleCharacterTokens;

        public bool AllowStringIndexersAsProperties => true;

        public UserDefinedFunction[] Functions => new UserDefinedFunction[]
        {
            new UserDefinedFunction(declaration: new FunctionDeclaration(returnType: typeof(string), "super", Array.Empty<ParameterDeclaration>()), JinjaFunctions.Super),
            new UserDefinedFunction(declaration: new FunctionDeclaration(returnType: typeof(string), "e", new ParameterDeclaration[] {
                new ParameterDeclaration("s")
            }), JinjaFunctions.Escape),
            new UserDefinedFunction(declaration: new FunctionDeclaration(returnType: typeof(string), "upper", new ParameterDeclaration[] {
                new ParameterDeclaration("s")
            }), JinjaFunctions.Upper),
        };
    }
}
