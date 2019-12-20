using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExpressionParser;
using ExpressionParser.Configuration;
using ExpressionParser.Lexing;

namespace Obsidian
{
    internal class JinjaLanguageDefinition : ILanguageDefinition
    {
        private const string _STRING_MEMBERACCESS = ".";
        private const string _STRING_PIPELINE = "|";
        private const string _STRING_EXPONENT = "**";
        private const string _STRING_PLUS = "+";
        private const string _STRING_MINUS = "-";


        private JinjaLanguageDefinition()
        {

        }
        private static readonly Lazy<JinjaLanguageDefinition> _Instance = new Lazy<JinjaLanguageDefinition>(() => new JinjaLanguageDefinition());
        internal static JinjaLanguageDefinition Instance => _Instance.Value;


        internal const string _OPERATOR_SQUARE_BRACE_OPEN = "[";
        internal const string _OPERATOR_PAREN_OPEN = "(";

        public IEnumerable<KeywordDefinition> Keywords => new KeywordDefinition[]
        {
            new ValueKeywordDefinition(true, "True", "true"),
            new ValueKeywordDefinition(false, "False", "false"),
            new ValueKeywordDefinition(null, "None", "none"),
        };

        public IEnumerable<OperatorDefinition> Operators => new OperatorDefinition[]
        {

            OperatorDefinition.CreateUnary(_STRING_PLUS, 170, OperatorType.UnaryPlus),
            OperatorDefinition.CreateUnary(_STRING_MINUS, 170, OperatorType.Negate),

            OperatorDefinition.CreateMemberAccess(_STRING_MEMBERACCESS, 160),
            OperatorDefinition.CreatePipeline(_STRING_PIPELINE, 160),
           OperatorDefinition.CreateMethod(_OPERATOR_PAREN_OPEN, TokenType.ParenOpen, TokenType.Comma, TokenType.ParenClose, 160),
            OperatorDefinition.CreateIndex(_OPERATOR_SQUARE_BRACE_OPEN, TokenType.ParenClose, TokenType.Comma, TokenType.SquareBraceClose, 160),

            OperatorDefinition.CreateUnary(_STRING_EXPONENT, 80, OperatorType.Power),

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
            OperatorDefinition.CreateBinary("!=", 40, OperatorType.NotEqual),
            OperatorDefinition.CreateBinary("==", 40, OperatorType.Equal),


            OperatorDefinition.CreateUnary("not", 30, OperatorType.LogicalNot),
            OperatorDefinition.CreateBinary("and", 20, OperatorType.LogicalAnd),
            OperatorDefinition.CreateBinary("or", 10, OperatorType.LogicalOr),

            OperatorDefinition.CreateBinary("=", 0, OperatorType.Assign),


        };

        public IDictionary<char, TokenType> SingleCharTokens => LanguageDefinition.StandardSingleCharacterTokens;

        public bool AllowStringIndexersAsProperties => true;
        public bool ReturnNullOnNonExistantProperties => true;

        public IEnumerable<UserDefinedFunction> Functions => new UserDefinedFunction[]
        {
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
            new UserDefinedFunction(declaration: new FunctionDeclaration(returnType: typeof(string), "escape", new ParameterDeclaration[] {
                new ParameterDeclaration("s")
            }, aliases: new []{ "e" }), JinjaFunctions.Escape),

            new UserDefinedFunction(declaration: new FunctionDeclaration(returnType: typeof(string), "filesizeformat", new ParameterDeclaration[] {
                new ParameterDeclaration("value"),
                new ParameterDeclaration("binary", false),
            }), JinjaFunctions.FilesizeFormat),
            new UserDefinedFunction(declaration: new FunctionDeclaration(returnType: typeof(string), "first", new ParameterDeclaration[] {
                new ParameterDeclaration("seq"),
            }), JinjaFunctions.First),
            new UserDefinedFunction(declaration: new FunctionDeclaration(returnType: typeof(string), "float", new ParameterDeclaration[] {
                new ParameterDeclaration("value"),
                new ParameterDeclaration("default", 0.0),
            }), JinjaFunctions.Float),
            new UserDefinedFunction(declaration: new FunctionDeclaration(returnType: typeof(string), "forceescape", new ParameterDeclaration[] {
                new ParameterDeclaration("s"),
            }), JinjaFunctions.ForceEscape),
            new UserDefinedFunction(declaration: new FunctionDeclaration(returnType: typeof(string), "format", new ParameterDeclaration[] {
                new ParameterDeclaration("value"),
                new ParameterDeclaration("args", null),
                new ParameterDeclaration("kwargs", null),
            }), JinjaFunctions.Format),



            new UserDefinedFunction(declaration: new FunctionDeclaration(returnType: typeof(string), "join", new ParameterDeclaration[] {
                new ParameterDeclaration("value"),
                new ParameterDeclaration("d", ""),
                new ParameterDeclaration("attribute", null),
            }), JinjaFunctions.Join),

            new UserDefinedFunction(declaration: new FunctionDeclaration(returnType: typeof(string), "upper", new ParameterDeclaration[] {
                new ParameterDeclaration("s")
            }), JinjaFunctions.Upper),
            #endregion Filters

            #region Functions 
            new UserDefinedFunction(declaration: new FunctionDeclaration(returnType: typeof(string), "range", new ParameterDeclaration[] {
                new ParameterDeclaration("start"),
                new ParameterDeclaration("stop", -1),
                new ParameterDeclaration("step", 1)
            }), JinjaFunctions.Range),
            #endregion Functions
        };

        IEnumerable<ScopedUserDefinedFunction> ILanguageDefinition.ScopedFunctions => new ScopedUserDefinedFunction[]
        {
            new ScopedUserDefinedFunction(declaration: new FunctionDeclaration(returnType: typeof(string), "super", Array.Empty<ParameterDeclaration>()), JinjaFunctions.Super),
        };

        public bool RequireNonDefaultArguments => false;

        public IEnumerable<UserDefinedTest> Tests => new UserDefinedTest[]
        {
            new UserDefinedTest(declaration: new FunctionDeclaration<bool>("even", new ParameterDeclaration[] {
                new ParameterDeclaration("value")
            }), JinjaFunctions.Even),
            new UserDefinedTest(declaration: new FunctionDeclaration<bool>("defined", new ParameterDeclaration[] {
                new ParameterDeclaration("value")
            }), JinjaFunctions.Defined)
        };
    }
}
