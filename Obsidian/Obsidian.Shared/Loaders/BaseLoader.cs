using System;
using System.Collections.Generic;

namespace Obsidian
{
    public abstract class BaseLoader
    {
        internal abstract TemplateInfo GetSource(JinjaEnvironment environment, string templateName);

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
