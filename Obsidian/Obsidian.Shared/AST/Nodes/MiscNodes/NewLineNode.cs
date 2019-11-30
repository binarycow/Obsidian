using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Obsidian.Parsing;
using Obsidian.Transforming;
using Obsidian.WhiteSpaceControl;

namespace Obsidian.AST.Nodes.MiscNodes
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class NewLineNode : ASTNode, IWhiteSpace
    {
        public NewLineNode(ParsingNode parsingNode) : base(parsingNode)
        {
        }
        public WhiteSpaceMode WhiteSpaceMode { get; set; }

        private string DebuggerDisplay => $"{nameof(NewLineNode)} : \"{ToString(debug: true)}\"";
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
    }
}
