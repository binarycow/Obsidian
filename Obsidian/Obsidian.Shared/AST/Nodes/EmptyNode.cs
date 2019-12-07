using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obsidian.Lexing;
using Obsidian.Parsing;
using Obsidian.Transforming;

namespace Obsidian.AST.Nodes
{
    internal class EmptyNode : ASTNode
    {
        private static readonly Lazy<EmptyNode> _Instance = new Lazy<EmptyNode>(() => new EmptyNode());
        public static EmptyNode Instance => _Instance.Value;
        private EmptyNode() : base(new ParsingNode(ParsingNodeType.Empty, Enumerable.Empty<Token>()))
        {

        }


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
