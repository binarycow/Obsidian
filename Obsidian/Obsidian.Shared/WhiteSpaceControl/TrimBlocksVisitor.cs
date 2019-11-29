using System;
using System.Collections.Generic;
using System.Text;
using Obsidian.AST;
using Obsidian.AST.Nodes;
using Obsidian.AST.Nodes.MiscNodes;
using Obsidian.AST.Nodes.Statements;
using Obsidian.Transforming;

namespace Obsidian.WhiteSpaceControl
{
    internal class TrimBlocksVisitor : ITransformVisitor
    {
        public TrimBlocksVisitor(JinjaEnvironment environment)
        {
            Environment = environment;
        }

        public JinjaEnvironment Environment { get; }

        public void Transform(TemplateNode item)
        {
            throw new NotImplementedException();
        }

        public void Transform(EmptyNode emptyNode)
        {
            throw new NotImplementedException();
        }

        public void Transform(ForNode item)
        {
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
