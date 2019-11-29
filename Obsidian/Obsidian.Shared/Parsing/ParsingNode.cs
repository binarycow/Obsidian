using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Obsidian.Lexing;

namespace Obsidian.Parsing
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class ParsingNode
    {
        public ParsingNode(ParsingNodeType nodeType, IEnumerable<Token> tokens)
        {
            NodeType = nodeType;
            Tokens = tokens.ToArrayWithoutInstantiation();
        }

        public ParsingNodeType NodeType { get; }
        public IEnumerable<Token> Tokens { get; }

        private string DebuggerDisplay => $"\"{this.ToString().Replace("\n", "\\n")}\"";
        public override string ToString()
        {
            return string.Join(string.Empty, Tokens.SelectMany(x => x.Value));
        }


        public virtual void ToOriginalText(StringBuilder stringBuilder)
        {
            foreach (var token in Tokens)
            {
                token.ToOriginalText(stringBuilder);
            }
        }
    }
}
