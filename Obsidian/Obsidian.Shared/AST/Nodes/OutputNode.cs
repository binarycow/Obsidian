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
    internal class OutputNode : ASTNode
    {
        internal OutputNode(ParsingNode parsingNode) : base(parsingNode)
        {
            Value = string.Join(string.Empty, parsingNode.Tokens.Select(token => token.Value));
        }
        internal string Value { get; }

        private string DebuggerDisplay => $"{nameof(OutputNode)} : \"{ToString(debug: true)}\"";
        public override TOutput Transform<TOutput>(ITransformVisitor<TOutput> visitor)
        {
            return visitor.Transform(this);
        }

        public override TOutput Transform<TOutput>(IForceTransformVisitor<TOutput> visitor, bool force)
        {
            return visitor.Transform(this, force);
        }
        public override void Transform(ITransformVisitor visitor)
        {
            visitor.Transform(this);
        }

        internal static OutputNode FromString(string @string)
        {
            return new OutputNode(new ParsingNode(ParsingNodeType.Output, new[] { new Token(TokenType.Unknown, @string) }));
        }
    }
}
