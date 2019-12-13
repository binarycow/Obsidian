using System;
using System.Collections.Generic;
using System.Text;

namespace Obsidian
{
    public interface ITemplate
    {
        public JinjaEnvironment Environment { get; }
        public string? TemplateName { get; }
        public string? TemplatePath { get; }
        public string Render(IDictionary<string, object?> variables);

        public bool Validate(IDictionary<string, object?> variables);
    }
}
