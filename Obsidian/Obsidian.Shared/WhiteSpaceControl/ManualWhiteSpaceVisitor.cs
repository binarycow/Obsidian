using System;
using System.Collections.Generic;
using System.Text;
using Obsidian.AST.Nodes;
using Obsidian.AST.Nodes.MiscNodes;
using Obsidian.AST.Nodes.Statements;
using Obsidian.Transforming;

namespace Obsidian.WhiteSpaceControl
{
    internal class ManualWhiteSpaceVisitor : ITransformVisitor
    {
        private static Lazy<ManualWhiteSpaceVisitor> _Instance = new Lazy<ManualWhiteSpaceVisitor>();
        public static ManualWhiteSpaceVisitor Instance => _Instance.Value;

        public void Transform(TemplateNode item)
        {
            foreach (var child in item.Children)
            {
                child.Transform(this);
            }
        }

        public void Transform(EmptyNode emptyNode)
        {
            throw new NotImplementedException();
        }

        public void Transform(ForNode item)
        {
            // TODO: Figure out how it works in Jinja2 if I have different white space control on the 'else' block than I do on the primary block.
            throw new NotImplementedException();
        }

        public void Transform(ContainerNode item)
        {
            throw new NotImplementedException();
        }

        public void Transform(ExpressionNode item)
        {
            throw new NotImplementedException();
        }

        public void Transform(NewLineNode item)
        {
            throw new NotImplementedException();
        }

        public void Transform(OutputNode item)
        {
            throw new NotImplementedException();
        }

        public void Transform(WhiteSpaceNode item)
        {
            throw new NotImplementedException();
        }

        public void Transform(IfNode item)
        {
            throw new NotImplementedException();
        }

        public void Transform(ConditionalNode item)
        {
            throw new NotImplementedException();
        }

        public void Transform(CommentNode item)
        {
            throw new NotImplementedException();
        }

        public void Transform(BlockNode item)
        {
            throw new NotImplementedException();
        }

        public void Transform(ExtendsNode item)
        {
            throw new NotImplementedException();
        }
    }
}
