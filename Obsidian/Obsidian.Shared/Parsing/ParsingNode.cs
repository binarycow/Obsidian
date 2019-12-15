using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Obsidian.Lexing;

namespace Obsidian.Parsing
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class ParsingNode
    {
        internal ParsingNode(ParsingNodeType nodeType, IEnumerable<Token> tokens)
        {
            NodeType = nodeType;
            Tokens = tokens.ToArrayWithoutInstantiation();
        }

        internal ParsingNodeType NodeType { get; }
        internal IEnumerable<Token> Tokens { get; }

        private string DebuggerDisplay => $"\"{this.ToString().Replace("\n", "\\n")}\"";
        public override string ToString()
        {
            return string.Join(string.Empty, Tokens.SelectMany(x => x.Value));
        }


        internal virtual void ToOriginalText(StringBuilder stringBuilder)
        {
            foreach (var token in Tokens)
            {
                token.ToOriginalText(stringBuilder);
            }
        }
    }
}
