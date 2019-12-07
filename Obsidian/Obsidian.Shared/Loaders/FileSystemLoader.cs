using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Obsidian.Exceptions;

namespace Obsidian
{
    public class FileSystemLoader : BaseLoader
    {
        private readonly Dictionary<string, string> _TemplateContentsCache = new Dictionary<string, string>();

        public FileSystemLoader(IEnumerable<string> searchPaths, Encoding encoding)
        {
            SearchPaths = new List<string>(searchPaths);
            Encoding = encoding;
        }
        public FileSystemLoader(IEnumerable<string> searchPaths) : this(searchPaths, Encoding.UTF8) { }
        public FileSystemLoader(string searchPath, Encoding encoding) : this(Enumerable.Repeat(searchPath, 1), encoding) { }
        public FileSystemLoader(string searchPath) : this(Enumerable.Repeat(searchPath, 1), Encoding.UTF8) { }

        public List<string> SearchPaths { get; }
        public Encoding Encoding { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private FileInfo? FindFile(string filename)
        {
            var searchPaths = new Stack<DirectoryInfo>();
            foreach(var searchPath in ((IEnumerable<string>)SearchPaths).Reverse())
            {
                try
                {
                    searchPaths.Push(new DirectoryInfo(searchPath));
                }
                catch
                {
                    // Eat the exception; ignore this directory.
                }
            }

            while(searchPaths.Count > 0)
            {
                var directory = searchPaths.Pop();
                foreach(var item in directory.GetFileSystemInfos())
                {
                    if(item is FileInfo fileInfo)
                    {
                        if(fileInfo.Name.ToUpperInvariant() == filename.ToUpperInvariant())
                        {
                            return fileInfo;
                        }
                    }
                    else if(item is DirectoryInfo directoryInfo)
                    {
                        searchPaths.Push(directoryInfo);
                    }
                }
            }
            return null;
        }

        public override TemplateInfo GetSource(JinjaEnvironment environment, string templateName)
        {
            var upToDate = false;
            var path = FindFile(templateName);
            if (path == null)
            {
                throw new TemplateNotFoundException(templateName);
            }
            var templateContents = File.ReadAllText(path.FullName, Encoding);
            if(_TemplateContentsCache.ContainsKey(templateName))
            {
                if(_TemplateContentsCache[templateName] == templateContents)
                {
                    upToDate = true;
                }
                else
                {
                    _TemplateContentsCache[templateName] = templateContents;
                }
            }
            else
            {
                _TemplateContentsCache.Add(templateName, templateContents);
            }
            return new TemplateInfo(templateContents, path.FullName, upToDate);
        }
    }
}
