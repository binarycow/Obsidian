using System;
using System.Collections.Generic;
using System.Text;
using ExpressionParser.Scopes;
using Obsidian.AST;
using Obsidian.AST.Nodes;
using Obsidian.Transforming;

namespace Obsidian.Templates
{
    internal class DynamicTemplate : ITemplate
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
        internal TemplateNode TemplateNode { get; }

        public string Render()
        {
            return Render(new Dictionary<string, object?>());
        }
        public string Render(IDictionary<string, object?> variables)
        {
            var renderer = new StringBuilderTransformer(Environment, variables);
            TemplateNode.Transform(renderer);
            return renderer.StringBuilder.ToString();
        }

        internal static DynamicTemplate LoadTemplate(JinjaEnvironment environment, string templateText, string? templateName, string? templatePath)
        {
            var node = ASTNode.GetTemplateNode(environment, templateText);
            if(node is TemplateNode templateNode)
            {
                return new DynamicTemplate(environment, templateNode, templateName, templatePath);
            }
            throw new NotImplementedException();
        }

        public bool Validate(IDictionary<string, object?> variables)
        {
            throw new NotImplementedException();
        }
    }
}
