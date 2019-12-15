using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressionParser.Configuration
{
    public class KeywordDefinition
    {
        public KeywordDefinition(IEnumerable<string> names)
        {
            Names = names.ToArrayWithoutInstantiation();
        }
        public IEnumerable<string> Names { get; }
    }
}
