using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ExpressionParser.Scopes;
using Obsidian.AST;
using Obsidian.AST.Nodes;
using Obsidian.AST.Nodes.Statements;
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

        private IDictionary<string, JinjaUserDefinedFunction> _UserDefinedFunctions = new Dictionary<string, JinjaUserDefinedFunction>();

        public JinjaUserDefinedFunction? this[string name]
        {
            get
            {
                if(_UserDefinedFunctions.TryGetValue(name, out var function))
                {
                    return function;
                }
                return default;
            }
        }

        internal void AddUserDefinedFunction(JinjaUserDefinedFunction func)
        {
            _UserDefinedFunctions.Add(func.name, func);
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
