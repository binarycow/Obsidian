using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Obsidian.Lexing;
using Obsidian.WhiteSpaceControl;

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
        public WhiteSpaceControlMode WhiteSpaceControlMode { get; set; }

        private string DebuggerDisplay => $"\"{this.ToString().Replace("\n", "\\n")}\" - Control: {WhiteSpaceControlMode}";
        public override string ToString()
        {
            return string.Join(string.Empty, Tokens.SelectMany(x => x.Value));
        }
    }
}
