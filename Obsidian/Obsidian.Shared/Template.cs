using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ExpressionParser;
using Obsidian.AST;
using Obsidian.AST.Nodes;
using Obsidian.CommentRemover;
using Obsidian.CompiledAST;
using Obsidian.Lexing;
using Obsidian.OutputCombiner;
using Obsidian.Parsing;
using Obsidian.Rendering.RenderObjects;
using Obsidian.Rendering.Visitors;
using Obsidian.Transforming;
using Obsidian.WhiteSpaceControl;
using Obsidian.AST.Nodes.Statements;

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
        internal static Template LoadTemplate(JinjaEnvironment environment, string templateText, IDictionary<string, object?> variableTemplate, string? templateName, string? templatePath)
        {
            var lexer = new Lexer(environment);
            var tokens = lexer.Tokenize(templateText).ToArray();
            var parsed = Parser.Parse(tokens).ToArray();
            var environmentTrimmed = EnvironmentTrimming.EnvironmentTrim(parsed, environment.Settings).ToArray();
            var templateNode = ASTGenerator.ParseTemplate(environmentTrimmed);
            var commentsRemoved = templateNode.Transform(CommentRemoverTransformer.Instance);
            var controlledWhiteSpace = WhiteSpaceController.ControlWhiteSpace(commentsRemoved);
           // var outputCombined = controlledWhiteSpace.Transform(OutputCombineVisitor.Instance);
            var compiled = ASTCompiler.Compile(environment, controlledWhiteSpace, variableTemplate);
            return new Template(environment, compiled, templateName, templatePath);
        }


        public string Render(IDictionary<string, object?> variables)
        {
            return TemplateNode.Evaluate(variables)?.ToString() ?? string.Empty;
            //if (Compiled)
            //{
            //    return RenderCompiled(variables);
            //}
            //var visitor = new StringRenderVisitor();
            //ITransformVisitor<IEnumerable<RenderObj>> renderVisitor = new ReflectionVisitor(Environment, variables);
            //var rendered = TemplateNode.Transform(renderVisitor).ToArray();
            //var strings = rendered.SelectMany(renderObj => renderObj.Render(visitor)).ToArray();
            //return string.Join(string.Empty, strings);
        }

    }
}
