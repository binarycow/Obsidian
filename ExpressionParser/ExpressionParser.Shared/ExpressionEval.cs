using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ExpressionParser.Lexing;
using ExpressionParser.Configuration;
using ExpressionParser.Parsing;
using ExpressionParser.Transforming;
using ExpressionParser.Transforming.Nodes;
using ExpressionParser.VariableManagement;
using ExpressionParser.Scopes;

namespace ExpressionParser
{
    public class ExpressionEval
    {
        public ExpressionEval(ILanguageDefinition languageDefinition, Lexer? lexer = null, Parser? parser = null)
        {
            LanguageDefinition = languageDefinition;
            Lexer = lexer ?? new Lexer(LanguageDefinition);
            Parser = parser ?? new Parser(LanguageDefinition);
        }

        public ILanguageDefinition LanguageDefinition { get; }
        public Lexer Lexer { get; }
        public Parser Parser { get; }


        public Expression ToExpression(string expressionText, Scope scope)
        {
            var tokens = Lexer.Tokenize(expressionText).ToArray();
            var astNode = Parser.Parse(tokens);
            var transformer = new ExpressionTreeTransformer(LanguageDefinition, scope);
            return astNode.Transform(transformer);
        }



        public ExpressionData Compile(string expressionText, Scope scope)
        {
            var expression = ToExpression(expressionText, scope);
            return ExpressionData.CreateCompiled(expression, scope);
        }
        public ExpressionData Dynamic(string expressionText, RootScope scope)
        {
            var expression = ToExpression(expressionText, scope);
            return ExpressionData.CreateDynamic(expression, scope);
        }



        public T EvaluateAs<T>(string expressionText, IDictionary<string, object?> variables)
        {
            var rootScope = RootScope.CreateRootScope(variables);
            var data = Dynamic(expressionText, rootScope);
            return data.EvaluateAs<T>(variables);
        }
        public object? Evaluate(string expressionText, IDictionary<string, object?> variables)
        {
            var rootScope = RootScope.CreateRootScope(variables);
            var data = Dynamic(expressionText, rootScope);
            return data.Evaluate(variables);
        }


        public T EvaluateAs<T>(string expressionText)
        {
            return EvaluateAs<T>(expressionText, new Dictionary<string, object?>());
        }
        public object? Evaluate(string expressionText)
        {
            return Evaluate(expressionText, new Dictionary<string, object?>());
        }
    }
}