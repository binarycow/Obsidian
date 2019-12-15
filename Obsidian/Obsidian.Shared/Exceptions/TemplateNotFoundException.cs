using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obsidian.Exceptions
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "<Pending>")]
    public class TemplateNotFoundException : Exception
    {
        public TemplateNotFoundException() : base()
        {
            TemplateName = string.Empty;
        }
        public TemplateNotFoundException(string templateName, Exception? inner = null) : base($"Template {templateName} could not be found.", inner)
        {
            TemplateName = templateName;
        }

        public string TemplateName { get; }
    }
}
