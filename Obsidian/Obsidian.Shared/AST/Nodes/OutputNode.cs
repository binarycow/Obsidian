using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Obsidian.Lexing;
using Obsidian.Parsing;
using Obsidian.Transforming;

namespace Obsidian.AST.Nodes
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class OutputNode : ASTNode
    {
        public OutputNode(ParsingNode parsingNode) : base(parsingNode)
        {
            Value = string.Join(string.Empty, parsingNode.Tokens.Select(token => token.Value));
        }
        public string Value { get; }

        private string DebuggerDisplay => $"{nameof(OutputNode)} : \"{ToString(debug: true)}\"";
        public override TOutput Transform<TOutput>(ITransformVisitor<TOutput> visitor)
        {
            return visitor.Transform(this);
        }

        public override TOutput Transform<TOutput>(IForceTransformVisitor<TOutput> visitor, bool force)
        {
            return visitor.Transform(this, force);
        }

        internal static OutputNode FromString(string @string)
        {
            return new OutputNode(new ParsingNode(ParsingNodeType.Output, new[] { new Token(TokenTypes.Unknown, @string) }));
        }
    }
}
