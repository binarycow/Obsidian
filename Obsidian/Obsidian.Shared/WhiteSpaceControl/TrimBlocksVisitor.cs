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
        private static readonly Lazy<TrimBlocksVisitor> _Instance = new Lazy<TrimBlocksVisitor>();
        public static TrimBlocksVisitor Instance => _Instance.Value;


        public bool TrimNewLine { get; private set; } = false;

        public void Transform(TemplateNode item)
        {
            foreach(var child in item.Children)
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
            if(TrimNewLine)
            {
                item.WhiteSpaceMode = WhiteSpaceMode.Trim;
            }
            TrimNewLine = false;
        }

        public void Transform(OutputNode item)
        {
            return;
        }

        public void Transform(WhiteSpaceNode item)
        {
            return;
        }

        public void Transform(IfNode item)
        {
            foreach (var condition in item.Conditions)
            {
                TrimNewLine = true;
                condition.Transform(this);
            }
            TrimNewLine = true;
        }

        public void Transform(ConditionalNode item)
        {
            TrimNewLine = true;
            foreach (var child in item.Children)
            {
                child.Transform(this);
            }
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

        public void Transform(RawNode item)
        {
            throw new NotImplementedException();
        }
        public void Transform(MacroNode item)
        {
            throw new NotImplementedException();
        }

        public void Transform(CallNode item)
        {
            throw new NotImplementedException();
        }

        public void Transform(FilterNode item)
        {
            throw new NotImplementedException();
        }

        public void Transform(SetNode item)
        {
            throw new NotImplementedException();
        }
    }
}
