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
using ExpressionParser.Scopes;
using System.Diagnostics.CodeAnalysis;

namespace ExpressionParser
{
    [SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "<Pending>")]
    internal class ExpressionEval
    {
        internal ExpressionEval(ILanguageDefinition languageDefinition)
        {
            languageDefinition = languageDefinition ?? throw new ArgumentNullException(nameof(languageDefinition));
            LanguageDefinition = languageDefinition;
            Lexer = new Lexer(LanguageDefinition);
            Parser = new Parser(LanguageDefinition);
        }
        internal ExpressionEval(ILanguageDefinition languageDefinition, Lexer? lexer = null, Parser? parser = null)
        {
            LanguageDefinition = languageDefinition;
            Lexer = lexer ?? new Lexer(LanguageDefinition);
            Parser = parser ?? new Parser(LanguageDefinition);
        }

        internal ILanguageDefinition LanguageDefinition { get; }
        internal Lexer Lexer { get; }
        internal Parser Parser { get; }

        internal object? EvaluateDynamic<TScope, TRootScope>(string expressionText, ScopeStack<TScope, TRootScope> scopeStack) 
            where TScope : DynamicScope
            where TRootScope : TScope
        {
            var astNode = Parse(expressionText);
            var transformer = new DynamicTransformer<TScope, TRootScope>(scopeStack, LanguageDefinition);

            var result = astNode.Transform(transformer);
            while(result is IEvaluatable evaluatable)
            {
                result = evaluatable.Transform(transformer);
            }
            return result;
        }

        internal ASTNode Parse(string expressionText)
        {
            var tokens = Lexer.Tokenize(expressionText).ToArray();
            return Parser.Parse(tokens);
        }


        internal Expression ToExpression(string expressionText, CompiledScope scope)
        {
            var tokens = Lexer.Tokenize(expressionText).ToArray();
            var astNode = Parser.Parse(tokens);
            var transformer = new ExpressionTreeTransformer(LanguageDefinition, scope);
            return astNode.Transform(transformer);
        }

        internal bool IsLiteralValue(string expressionText)
        {
            switch(Lexer.TryReadOnlyOneToken(expressionText, out var firstToken))
            {
                case null:
                    return false;
                case false:
                    return false;
                case true:
                    return firstToken != null && firstToken.Value.TokenType.IsLiteral();
            }
        }

        internal ExpressionData Compile(string expressionText, CompiledScope scope)
        {
            var expression = ToExpression(expressionText, scope);
            return ExpressionData.CreateCompiled(expression, scope);
        }
        internal ExpressionData Dynamic(string expressionText, CompiledScope scope)
        {
            var expression = ToExpression(expressionText, scope);
            return ExpressionData.CreateDynamic(expression, scope);
        }

        internal T EvaluateAs<T>(string expressionText, IDictionary<string, object?> variables)
        {
            variables = variables ?? throw new ArgumentNullException(nameof(variables));
            var rootScope = CompiledScope.CreateRootScope("root", variables);
            var data = Dynamic(expressionText, rootScope);
            return data.EvaluateAs<T>(variables);
        }
        internal object? Evaluate(string expressionText, IDictionary<string, object?> variables)
        {
            variables = variables ?? throw new ArgumentNullException(nameof(variables));
            var rootScope = CompiledScope.CreateRootScope("root", variables);
            var data = Dynamic(expressionText, rootScope);
            return data.Evaluate(variables);
        }


        internal T EvaluateAs<T>(string expressionText)
        {
            return EvaluateAs<T>(expressionText, new Dictionary<string, object?>());
        }
        internal object? Evaluate(string expressionText)
        {
            return Evaluate(expressionText, new Dictionary<string, object?>());
        }

        internal bool TryParseFunctionDeclaration(string declarationText, [NotNullWhen(true)]out FunctionDeclaration? functionDeclaration)
        {
            var tokens = Lexer.Tokenize(declarationText).ToArray();
            var astNode = Parser.Parse(tokens);
            return FunctionDeclarationParser.TryParseFunctionDeclaration(astNode, out functionDeclaration);
        }
    }
}
