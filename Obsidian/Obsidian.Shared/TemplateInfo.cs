using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obsidian
{
    public readonly struct TemplateInfo
    {
        public TemplateInfo(string source, string filename, bool upToDate)
        {
            Source = source;
            Filename = filename;
            UpToDate = upToDate;
        }
        public string Source { get; }
        public string Filename { get; }
        public bool UpToDate { get; }
    }
}
