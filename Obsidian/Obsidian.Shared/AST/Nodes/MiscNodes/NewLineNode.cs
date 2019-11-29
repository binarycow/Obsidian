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
    public class NewLineNode : ASTNode
    {
        public NewLineNode(ParsingNode parsingNode, WhiteSpaceControlMode controlMode) : base(parsingNode)
        {
            ControlMode = controlMode;
        }
        public WhiteSpaceControlMode ControlMode { get; set; }

        private string DebuggerDisplay => $"{nameof(NewLineNode)} : \"{ToString(debug: true)}\" Control: {ControlMode}";
        public override TOutput Transform<TOutput>(ITransformVisitor<TOutput> visitor)
        {
            return visitor.Transform(this);
        }

        public override TOutput Transform<TOutput>(IForceTransformVisitor<TOutput> visitor, bool force)
        {
            return visitor.Transform(this, force);
        }
    }
}
