using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ExpressionParser;
using ExpressionParser.Scopes;
using Obsidian;
using Obsidian.Exceptions;
using Obsidian.ExpressionParserExt;

namespace Obsidian
{
    public class JinjaEnvironment
    {
        internal const string TRUE = "True";
        public JinjaEnvironment(BaseLoader? loader = null)
        {
            _Evaluation = new Lazy<ExpressionEval>(() => new ExpressionEval(LanguageDefinition, 
                lexer: new JinjaLexer(LanguageDefinition),
                parser: new JinjaParser(LanguageDefinition)
            ));
            Settings = new EnvironmentSettings();
            Loader = loader;
        }
        public EnvironmentSettings Settings { get; }
        public BaseLoader? Loader { get; set; }


        private Lazy<JinjaLanguageDefinition> _LanguageDefinition = new Lazy<JinjaLanguageDefinition>();
        public JinjaLanguageDefinition LanguageDefinition => _LanguageDefinition.Value;
        internal ExpressionEval Evaluation => _Evaluation.Value;
        private readonly Lazy<ExpressionEval> _Evaluation;

        internal Template GetTemplate(string templateName, TemplateInfo templateInfo, IDictionary<string, object?> variableTemplate)
        {
            return GetTemplate(templateInfo.Source, variableTemplate, templateName, templateInfo.Filename);
        }
        internal Template GetTemplate(string templateName, TemplateInfo templateInfo, IScope scope)
        {
            return GetTemplate(templateInfo.Source, scope, templateName, templateInfo.Filename);
        }
        internal Template GetTemplate(string templateText, IDictionary<string, object?> variableTemplate, string? templateName, string? templatePath)
        {
            Settings.IsReadOnly = true;
            return Template.LoadTemplate(this, templateText, variableTemplate, templateName, templatePath);
        }
        internal Template GetTemplate(string templateText, IScope scope, string? templateName, string? templatePath)
        {
            Settings.IsReadOnly = true;
            return Template.LoadTemplate(this, templateText, scope, templateName, templatePath);
        }
        public Template GetTemplate(string templateName, IDictionary<string, object?> variableTemplate)
        {
            if (Loader == null)
            {
                throw new LoaderNotDefinedException();
            }
            var templateInfo = Loader.GetSource(this, templateName);
            return GetTemplate(templateInfo.Source, variableTemplate, templateName, templateInfo.Filename);
        }
        public Template GetTemplate(string templateName, IScope scope)
        {
            if (Loader == null)
            {
                throw new LoaderNotDefinedException();
            }
            var templateInfo = Loader.GetSource(this, templateName);
            return GetTemplate(templateInfo.Source, scope, templateName, templateInfo.Filename);
        }

        public Expression GetTemplateExpression(string templateName, IScope scope)
        {
            if (Loader == null)
            {
                throw new LoaderNotDefinedException();
            }
            var templateInfo = Loader.GetSource(this, templateName);
            Settings.IsReadOnly = true;
            return Template.ToExpression(templateName, this, templateInfo.Source, scope);
        }
        public Template FromString(string templateText, IDictionary<string, object?> variableTemplate)
        {
            return GetTemplate(templateText, variableTemplate, null, null);
        }

    }
}
