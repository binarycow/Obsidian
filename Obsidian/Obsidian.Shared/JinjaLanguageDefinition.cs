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
        private const string STRING_MEMBERACCESS = ".";
        private const string STRING_PIPELINE = "|";
        private const string STRING_EXPONENT = "**";
        private const string STRING_PLUS = "+";
        private const string STRING_MINUS = "-";


        private JinjaLanguageDefinition()
        {

        }
        private static Lazy<JinjaLanguageDefinition> _Instance = new Lazy<JinjaLanguageDefinition>(() => new JinjaLanguageDefinition());
        public static JinjaLanguageDefinition Instance => _Instance.Value;


        internal const string OPERATOR_SQUARE_BRACE_OPEN = "[";
        internal const string OPERATOR_PAREN_OPEN = "(";

        public IEnumerable<KeywordDefinition> Keywords => new KeywordDefinition[]
        {
            new ValueKeywordDefinition(true, "True", "true"),
            new ValueKeywordDefinition(false, "False", "false"),
            new ValueKeywordDefinition(null, "None", "none"),
        };

        public IEnumerable<OperatorDefinition> Operators => new OperatorDefinition[]
        {
            OperatorDefinition.CreateMemberAccess(STRING_MEMBERACCESS, 160),
            OperatorDefinition.CreatePipeline(STRING_PIPELINE, 160),
            OperatorDefinition.CreateMethod(OPERATOR_PAREN_OPEN, TokenType.ParenOpen, TokenType.Comma, TokenType.ParenClose, 160),
            OperatorDefinition.CreateIndex(OPERATOR_SQUARE_BRACE_OPEN, TokenType.ParenClose, TokenType.Comma, TokenType.SquareBraceClose, 160),

            OperatorDefinition.CreateUnary(STRING_EXPONENT, 80, OperatorType.Power),

            OperatorDefinition.CreateUnary(STRING_PLUS, 70, OperatorType.UnaryPlus),
            OperatorDefinition.CreateUnary(STRING_MINUS, 70, OperatorType.Negate),

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

        public IEnumerable<UserDefinedFunction> Functions => new UserDefinedFunction[]
        {
            new UserDefinedFunction(declaration: new FunctionDeclaration(returnType: typeof(string), "super", Array.Empty<ParameterDeclaration>()), JinjaFunctions.Super),


            #region Filters
            new UserDefinedFunction(declaration: new FunctionDeclaration(returnType: typeof(int), "abs", new ParameterDeclaration[] {
                new ParameterDeclaration("x")
            }), JinjaFunctions.Abs),
            new UserDefinedFunction(declaration: new FunctionDeclaration(returnType: typeof(object), "attr", new ParameterDeclaration[] {
                new ParameterDeclaration("obj"),
                new ParameterDeclaration("name"),
            }), JinjaFunctions.Attr),
            new UserDefinedFunction(declaration: new FunctionDeclaration(returnType: typeof(IEnumerable<IEnumerable<object>>), "batch", new ParameterDeclaration[] {
                new ParameterDeclaration("value"),
                new ParameterDeclaration("linecount"),
                new ParameterDeclaration("fill_with", null),
            }), JinjaFunctions.Batch),
            new UserDefinedFunction(declaration: new FunctionDeclaration(returnType: typeof(string), "capitalize", new ParameterDeclaration[] {
                new ParameterDeclaration("s")
            }), JinjaFunctions.Capitalize),
            new UserDefinedFunction(declaration: new FunctionDeclaration(returnType: typeof(string), "center", new ParameterDeclaration[] {
                new ParameterDeclaration("s"),
                new ParameterDeclaration("width", 80)
            }), JinjaFunctions.Center),
            new UserDefinedFunction(declaration: new FunctionDeclaration(returnType: typeof(string), "default", new ParameterDeclaration[] {
                new ParameterDeclaration("value"),
                new ParameterDeclaration("default_value", string.Empty),
                new ParameterDeclaration("boolean", false)
            }, aliases: new []{ "d" }), JinjaFunctions.Default),
            new UserDefinedFunction(declaration: new FunctionDeclaration(returnType: typeof(Dictionary<,>), "dictsort", new ParameterDeclaration[] {
                new ParameterDeclaration("value"),
                new ParameterDeclaration("case_sensitive", false),
                new ParameterDeclaration("by", "key"),
                new ParameterDeclaration("reverse", false)
            }), JinjaFunctions.DictSort),
            new UserDefinedFunction(declaration: new FunctionDeclaration(returnType: typeof(string), "e", new ParameterDeclaration[] {
                new ParameterDeclaration("s")
            }, aliases: new []{ "e" }), JinjaFunctions.Escape),



            new UserDefinedFunction(declaration: new FunctionDeclaration(returnType: typeof(string), "upper", new ParameterDeclaration[] {
                new ParameterDeclaration("s")
            }), JinjaFunctions.Upper),
            #endregion Filters
        };
    }
}
