using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Obsidian
{
    public class ArgumentNameCollection : IEnumerable<string>
    {
        public ArgumentNameCollection(IEnumerable<string> source)
        {
            _Contents = source;
        }
        private readonly IEnumerable<string> _Contents;

        public IEnumerator<string> GetEnumerator() => _Contents.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _Contents.GetEnumerator();
    }
}
