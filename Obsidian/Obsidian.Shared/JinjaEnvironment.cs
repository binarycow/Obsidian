using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExpressionParser;
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
        internal Template GetTemplate(string templateText, IDictionary<string, object?> variableTemplate, string? templateName, string? templatePath)
        {
            Settings.IsReadOnly = true;
            return Template.LoadTemplate(this, templateText, variableTemplate, templateName, templatePath);
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
        public Template FromString(string templateText, IDictionary<string, object?> variableTemplate)
        {
            return GetTemplate(templateText, variableTemplate, null, null);
        }

    }
}
