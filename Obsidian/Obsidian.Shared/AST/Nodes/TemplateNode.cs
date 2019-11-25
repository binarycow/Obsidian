using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obsidian.Transforming;

namespace Obsidian.AST.Nodes
{
    public class TemplateNode : ASTNode
    {
        public TemplateNode(IEnumerable<ASTNode> children) : base(children.SelectMany(child => child.ParsingNodes))
        {
            Children = children.ToArrayWithoutInstantiation();
        }

        public ASTNode[] Children { get; }
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
