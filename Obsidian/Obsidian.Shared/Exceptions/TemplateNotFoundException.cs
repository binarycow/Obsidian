using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obsidian.Exceptions
{
    public class TemplateNotFoundException : Exception
    {
        public TemplateNotFoundException() : base()
        {
            TemplateName = string.Empty;
        }
        public TemplateNotFoundException(string message) : base(message)
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
