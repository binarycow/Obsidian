using System.Collections.Generic;

namespace Obsidian
{
    public abstract class BaseLoader
    {
        public abstract TemplateInfo GetSource(JinjaEnvironment environment, string templateName);

        private Dictionary<string, ITemplate> _TemplateCache = new Dictionary<string, ITemplate>();

        public ITemplate Load(JinjaEnvironment environment, string name, IDictionary<string, object?> variableTemplate)
        {
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
