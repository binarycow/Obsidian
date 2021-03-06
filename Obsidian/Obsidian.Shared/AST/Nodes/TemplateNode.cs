using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obsidian.Lexing;
using Obsidian.Parsing;
using Obsidian.Transforming;

namespace Obsidian.AST.Nodes
{
    internal class TemplateNode : ASTNode
    {
        internal TemplateNode(IEnumerable<ASTNode> children) : base(null, children.SelectMany(child => child.ParsingNodes), null)
        {
            Children = children.ToArrayWithoutInstantiation();
        }

        internal ASTNode[] Children { get; }
        public override TOutput Transform<TOutput>(ITransformVisitor<TOutput> visitor)
        {
            return visitor.Transform(this);
        }

        public override void Transform(IManualWhiteSpaceTransformVisitor visitor, bool inner = false)
        {
            visitor.Transform(this, inner);
        }
        public override void Transform(ITransformVisitor visitor)
        {
            visitor.Transform(this);
        }
        public override TOutput Transform<TOutput>(IForceTransformVisitor<TOutput> visitor, bool force)
        {
            return visitor.Transform(this, force);
        }
    }
}
