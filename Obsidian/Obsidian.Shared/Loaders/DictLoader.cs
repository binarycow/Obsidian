using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Obsidian.Loaders
{
    public class DictLoader : BaseLoader
    {
        public DictLoader(IDictionary<string, string> templates)
        {
            _Templates = templates;
        }


        private readonly IDictionary<string, string> _Templates;

        internal override bool TryGetSource(JinjaEnvironment environment, string templateName, [NotNullWhen(true)] out TemplateInfo? templateInfo)
        {
            templateInfo = default;
            if(_Templates.TryGetValue(templateName, out var templateText) == false)
            {
                return false;
            }
            templateInfo = new TemplateInfo(templateText, templateName, true);
            return true;
        }
    }
}
