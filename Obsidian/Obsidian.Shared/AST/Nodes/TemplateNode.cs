using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obsidian.Lexing;
using Obsidian.Parsing;
using Obsidian.Transforming;

namespace Obsidian.AST.Nodes
{
    public class TemplateNode : ASTNode
    {
        public TemplateNode(IEnumerable<ASTNode> children) : base(null, children.SelectMany(child => child.ParsingNodes), null)
        {
            Children = children.ToArrayWithoutInstantiation();
        }

        public ASTNode[] Children { get; }
        public override TOutput Transform<TOutput>(ITransformVisitor<TOutput> visitor)
        {
            return visitor.Transform(this);
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
