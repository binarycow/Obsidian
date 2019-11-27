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
using Obsidian.Templates;

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

        internal ITemplate GetTemplate(string templateName, TemplateInfo templateInfo, IDictionary<string, object?> variableTemplate)
        {
            return GetTemplate(templateInfo.Source, variableTemplate, templateName, templateInfo.Filename);
        }
        internal ITemplate GetTemplate(string templateText, IDictionary<string, object?> variableTemplate, string? templateName, string? templatePath)
        {
            Settings.IsReadOnly = true;
            return Settings.DynamicTemplates ?
                (ITemplate)DynamicTemplate.LoadTemplate(this, templateText, variableTemplate, templateName, templatePath) :
                CompiledTemplate.LoadTemplate(this, templateText, variableTemplate, templateName, templatePath);
        }
        internal ITemplate GetTemplate<T>(string templateText, IScope<T> scope, string? templateName, string? templatePath) 
            where T : class, IScope<T>
        {
            Settings.IsReadOnly = true;

            if (Settings.DynamicTemplates)
            {
                if (!(scope is IDynamicScope dynamicScope)) throw new NotImplementedException();
                return DynamicTemplate.LoadTemplate(this, templateText, dynamicScope, templateName, templatePath);
            }

            if (!(scope is ICompiledScope compiledScope)) throw new NotImplementedException();
            return CompiledTemplate.LoadTemplate(this, templateText, compiledScope, templateName, templatePath);
        }
        public ITemplate GetTemplate(string templateName, IDictionary<string, object?> variableTemplate)
        {
            if (Loader == null)
            {
                throw new LoaderNotDefinedException();
            }
            var templateInfo = Loader.GetSource(this, templateName);
            return GetTemplate(templateInfo.Source, variableTemplate, templateName, templateInfo.Filename);
        }

        public Expression GetTemplateExpression(string templateName, ICompiledScope scope)
        {
            if (Loader == null)
            {
                throw new LoaderNotDefinedException();
            }
            var templateInfo = Loader.GetSource(this, templateName);
            Settings.IsReadOnly = true;
            return CompiledTemplate.ToExpression(templateName, this, templateInfo.Source, scope);
        }
        public ITemplate GetTemplate(string templateName, IDynamicScope scope)
        {
            if (Loader == null)
            {
                throw new LoaderNotDefinedException();
            }
            var templateInfo = Loader.GetSource(this, templateName);
            Settings.IsReadOnly = true;
            return GetTemplate(templateInfo.Source, scope, templateName, templateInfo.Filename);
        }

    }
}
