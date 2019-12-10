using Obsidian.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Obsidian
{
    public abstract class BaseLoader
    {
        internal abstract bool TryGetSource(JinjaEnvironment environment, string templateName, [NotNullWhen(true)]out TemplateInfo? templateInfo);
        internal TemplateInfo GetSource(JinjaEnvironment environment, string templateName)
        {
            if(TryGetSource(environment, templateName, out var templateInfo) == false || templateInfo == null)
            {
                throw new TemplateNotFoundException(templateName);
            }
            return templateInfo.Value;
        }

        private readonly Dictionary<string, ITemplate> _TemplateCache = new Dictionary<string, ITemplate>();

        public virtual ITemplate Load(JinjaEnvironment environment, string name, IDictionary<string, object?> variableTemplate)
        {
            environment = environment ?? throw new ArgumentNullException(nameof(environment));
            name = name ?? throw new ArgumentNullException(nameof(name));
            variableTemplate = variableTemplate ?? throw new ArgumentNullException(nameof(variableTemplate));
            var templateInfo = GetSource(environment, name);
            if(_TemplateCache.ContainsKey(name) == false || templateInfo.UpToDate == false)
            {
                var template = environment.GetTemplate(name, templateInfo, variableTemplate);
                _TemplateCache.Upsert(name, template);
            }
            return _TemplateCache[name];
        }
    }
}
