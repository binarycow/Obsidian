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
            languageDefinition.Validate();
            LanguageDefinition = languageDefinition;
            Lexer = lexer ?? new Lexer(LanguageDefinition);
            Parser = parser ?? new Parser(LanguageDefinition);
        }

        public ILanguageDefinition LanguageDefinition { get; }
        public Lexer Lexer { get; }
        public Parser Parser { get; }

        public object? EvaluateDynamic<TScope, TRootScope>(string expressionText, ScopeStack<TScope, TRootScope> scopeStack) 
            where TScope : DynamicScope
            where TRootScope : TScope
        {
            var tokens = Lexer.Tokenize(expressionText).ToArray();
            var astNode = Parser.Parse(tokens);
            var transformer = new DynamicTransformer<TScope, TRootScope>(scopeStack, LanguageDefinition);
            return astNode.Transform(transformer);
        }

        public Expression ToExpression(string expressionText, CompiledScope scope)
        {
            var tokens = Lexer.Tokenize(expressionText).ToArray();
            var astNode = Parser.Parse(tokens);
            var transformer = new ExpressionTreeTransformer(LanguageDefinition, scope);
            return astNode.Transform(transformer);
        }

        public bool IsLiteralValue(string expressionText)
        {
            switch(Lexer.TryReadOnlyOneToken(expressionText, out var firstToken))
            {
                case null:
                    return false;
                case false:
                    return false;
                case true:
                    return firstToken != null && firstToken.TokenType.IsLiteral();
            }
        }

        public ExpressionData Compile(string expressionText, CompiledScope scope)
        {
            var expression = ToExpression(expressionText, scope);
            return ExpressionData.CreateCompiled(expression, scope);
        }
        public ExpressionData Dynamic(string expressionText, CompiledScope scope)
        {
            var expression = ToExpression(expressionText, scope);
            return ExpressionData.CreateDynamic(expression, scope);
        }

        public T EvaluateAs<T>(string expressionText, IDictionary<string, object?> variables)
        {
            var rootScope = CompiledScope.CreateRootScope("root", variables);
            var data = Dynamic(expressionText, rootScope);
            return data.EvaluateAs<T>(variables);
        }
        public object? Evaluate(string expressionText, IDictionary<string, object?> variables)
        {
            var rootScope = CompiledScope.CreateRootScope("root", variables);
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
