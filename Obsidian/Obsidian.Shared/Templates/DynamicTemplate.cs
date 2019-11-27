using System;
using System.Collections.Generic;
using System.Text;
using ExpressionParser.Scopes;
using Obsidian.AST;
using Obsidian.AST.Nodes;
using Obsidian.Transforming;

namespace Obsidian.Templates
{
    public class DynamicTemplate : ITemplate
    {

        private DynamicTemplate(JinjaEnvironment environment, TemplateNode templateNode, string? templateName, string? templatePath)
        {
            TemplateNode = templateNode;
            Environment = environment;
            TemplateName = templateName;
            TemplatePath = templatePath;
        }
        public JinjaEnvironment Environment { get; }

        public string? TemplateName { get; }

        public string? TemplatePath { get; }
        public TemplateNode TemplateNode { get; }

        public string Render(IDictionary<string, object?> variables)
        {
            var renderer = new StringRenderTransformer(Environment, variables);
            return TemplateNode.Transform(renderer);
        }

        internal static DynamicTemplate LoadTemplate(JinjaEnvironment environment, string templateText, IDictionary<string, object?> variableTemplate, string? templateName, string? templatePath)
        {
            var node = ASTNode.GetTemplateNode(environment, templateText);
            if(node is TemplateNode templateNode)
            {
                return new DynamicTemplate(environment, templateNode, templateName, templatePath);
            }
            throw new NotImplementedException();
        }

        internal static DynamicTemplate LoadTemplate(JinjaEnvironment environment, string templateText, IDynamicScope scope, string? templateName, string? templatePath)
        {
            var node = ASTNode.GetTemplateNode(environment, templateText);
            if (node is TemplateNode templateNode)
            {
                return new DynamicTemplate(environment, templateNode, templateName, templatePath);
            }
            throw new NotImplementedException();
        }
    }
}
