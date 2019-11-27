using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ExpressionParser;
using Obsidian.AST;
using Obsidian.AST.Nodes;
using Obsidian.CommentRemover;
using Obsidian.Lexing;
using Obsidian.OutputCombiner;
using Obsidian.Parsing;
using Obsidian.Rendering.RenderObjects;
using Obsidian.Rendering.Visitors;
using Obsidian.Transforming;
using Obsidian.WhiteSpaceControl;
using Obsidian.AST.Nodes.Statements;
using ExpressionParser.Scopes;
using ExpressionToString;

namespace Obsidian
{
    public class Template
    {

        private Template(JinjaEnvironment environment, ExpressionData templateNode, string? templateName, string? templatePath)
        {
            TemplateNode = templateNode;
            Environment = environment;
            TemplateName = templateName;
            TemplatePath = templatePath;
        }

        public ExpressionData TemplateNode { get; }
        public JinjaEnvironment Environment { get; }
        public string? TemplateName { get; }
        public string? TemplatePath { get; }

        public string Render()
        {
            return Render(new Dictionary<string, object?>());
        }

        public bool Compiled { get; private set; }
        public void Compile()
        {
            Compile(new Dictionary<string, object?>());
        }
        public void Compile(IDictionary<string, object?> variables)
        {
            //var compiler = new ExpressionTreeTransformVisitor(Environment, variables);
            //var compiled = TemplateNode.Transform(compiler);
            throw new NotImplementedException();
        }
        private string RenderCompiled(IDictionary<string, object?> variables)
        {
            throw new NotImplementedException();
        }
        //internal static Template LoadTemplate(JinjaEnvironment environment, string templateText, IDictionary<string, object?> variableTemplate, string? templateName, string? templatePath)
        //{
        //    var expr = ToExpression(environment, templateText, variableTemplate, out var compiler);
        //    return new Template(environment, compiler.Compile(expr), templateName, templatePath);
        //}
        internal static Template LoadTemplate(JinjaEnvironment environment, string templateText, IScope scope, string? templateName, string? templatePath)
        {
            var expr = ToExpression(templateName, environment, templateText, scope);

            var debug = expr.ToString("C#");
            var test = new VariableSetterWalker();
            var x = test.Visit(expr);
            ;

            return new Template(environment, ExpressionData.CreateCompiled(expr, scope), templateName, templatePath);
        }

        internal static Template LoadTemplate(JinjaEnvironment environment, string templateText, IDictionary<string, object?> variableTemplate, string? templateName, string? templatePath)
        {
            var rootScope = Scope.CreateRootScope("GLOBALS", variableTemplate);
            var expr = ToExpression(templateName, environment, templateText, rootScope);

            var debug = expr.ToString("C#");
            var test = new VariableSetterWalker();
            var x = test.Visit(expr);
            ;

            return new Template(environment, ExpressionData.CreateCompiled(expr, rootScope), templateName, templatePath);
        }

        //internal static Expression ToExpression(JinjaEnvironment environment, string templateText, IDictionary<string, object?> variableTemplate, out ASTCompiler compiler)
        //{
        //    var lexer = new Lexer(environment);
        //    var tokens = lexer.Tokenize(templateText).ToArray();
        //    var parsed = Parser.Parse(tokens).ToArray();
        //    var environmentTrimmed = EnvironmentTrimming.EnvironmentTrim(parsed, environment.Settings).ToArray();
        //    var templateNode = ASTGenerator.ParseTemplate(environmentTrimmed);
        //    var commentsRemoved = templateNode.Transform(CommentRemoverTransformer.Instance);
        //    var controlledWhiteSpace = WhiteSpaceController.ControlWhiteSpace(commentsRemoved);
        //    var containerAssembled = controlledWhiteSpace.Transform(TemplateContainerAssembler.Instance);

        //    var finished = NewASTCompiler.ToExpression(environment, containerAssembled, variableTemplate, out var newcompiler);
        //    throw new NotImplementedException();
        //}
        internal static Expression ToExpression(string templateName, JinjaEnvironment environment, string templateText, IScope rootScope)
        {
            var lexer = new Lexer(environment);
            var tokens = lexer.Tokenize(templateText).ToArray();
            var parsed = Parser.Parse(tokens).ToArray();
            var environmentTrimmed = EnvironmentTrimming.EnvironmentTrim(parsed, environment.Settings).ToArray();
            var templateNode = ASTGenerator.ParseTemplate(environmentTrimmed);
            var commentsRemoved = templateNode.Transform(CommentRemoverTransformer.Instance);
            var controlledWhiteSpace = WhiteSpaceController.ControlWhiteSpace(commentsRemoved);
            var containerAssembled = controlledWhiteSpace.Transform(TemplateContainerAssembler.Instance);

            var finished = NewASTCompiler.ToExpression(templateName, environment, containerAssembled, out var newcompiler, rootScope);
            return finished;
        }

        internal static Template FromBlockNode(string templateName, JinjaEnvironment environment, BlockNode blockNode, IDictionary<string, object?> variableTemplate)
        {
            var rootScope = Scope.CreateRootScope("GLOBALS", variableTemplate);
            var expr = NewASTCompiler.ToExpression(templateName, environment, blockNode, out var newcompiler, rootScope);
            return new Template(environment, ExpressionData.CreateCompiled(expr, rootScope), blockNode.Name, null);
        }

        public string Render(IDictionary<string, object?> variables)
        {
            return TemplateNode.Evaluate(variables)?.ToString() ?? string.Empty;
        }

    }
}
