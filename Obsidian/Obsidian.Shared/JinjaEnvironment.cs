using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ExpressionParser;
using ExpressionParser.Scopes;
using Obsidian;
using Obsidian.AST;
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


        internal static JinjaLanguageDefinition LanguageDefinition => JinjaLanguageDefinition.Instance;
        internal ExpressionEval Evaluation => _Evaluation.Value;
        private readonly Lazy<ExpressionEval> _Evaluation;

#if DEBUG
        internal string CheckOriginalText(string templateName)
        {
            var templateInfo = GetTemplateInfo(templateName);
            return ASTNode.CheckOriginalText(this, templateInfo.Source);
        }
#endif

        internal ITemplate GetTemplate(string templateName, TemplateInfo templateInfo, IDictionary<string, object?> variableTemplate)
        {
            return GetTemplate(templateInfo.Source, variableTemplate, templateName, templateInfo.Filename);
        }
        internal ITemplate GetTemplate(string templateText, IDictionary<string, object?> variableTemplate, string templateName, string? templatePath)
        {
            Settings.IsReadOnly = true;
            return Settings.DynamicTemplates ?
                (ITemplate)DynamicTemplate.LoadTemplate(this, templateText, templateName, templatePath) :
                CompiledTemplate.LoadTemplate(this, templateText, variableTemplate, templateName, templatePath);
        }
        internal DynamicTemplate GetDynamicTemplate(string templateText, string? templateName, string? templatePath)
        {
            Settings.IsReadOnly = true;
            if (Settings.DynamicTemplates == false)
            {
                throw new NotImplementedException();
            }
            return DynamicTemplate.LoadTemplate(this, templateText, templateName, templatePath);
        }
        internal CompiledTemplate GetTemplate<T>(string templateText, CompiledScope scope, string templateName, string? templatePath)
        {
            Settings.IsReadOnly = true;
            if (Settings.DynamicTemplates)
            {
                throw new NotImplementedException();
            }
            return CompiledTemplate.LoadTemplate(this, templateText, scope, templateName, templatePath);
        }
        public ITemplate GetTemplate(string templateName, IDictionary<string, object?> variableTemplate)
        {
            var templateInfo = GetTemplateInfo(templateName);
            return GetTemplate(templateInfo.Source, variableTemplate, templateName, templateInfo.Filename);
        }

        private TemplateInfo GetTemplateInfo(string templateName)
        {
            if (Loader == null)
            {
                throw new LoaderNotDefinedException();
            }
            Settings.IsReadOnly = true;
            return Loader.GetSource(this, templateName);
        }

        internal Expression GetTemplateExpression(string templateName, CompiledScope scope)
        {
            var templateInfo = GetTemplateInfo(templateName);
            return CompiledTemplate.ToExpression(templateName, this, templateInfo.Source, scope);
        }
        internal DynamicTemplate GetDynamicTemplate(string templateName)
        {
            var templateInfo = GetTemplateInfo(templateName);
            return GetDynamicTemplate(templateInfo.Source, templateName, null);
        }

    }
}
