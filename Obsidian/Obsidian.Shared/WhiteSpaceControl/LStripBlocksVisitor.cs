using System;
using System.Collections.Generic;
using System.Text;
using Obsidian.AST.Nodes;
using Obsidian.AST.Nodes.MiscNodes;
using Obsidian.AST.Nodes.Statements;
using Obsidian.Transforming;

namespace Obsidian.WhiteSpaceControl
{
    internal class LStripBlocksVisitor : ITransformVisitor
    {
        private static readonly Lazy<LStripBlocksVisitor> _Instance = new Lazy<LStripBlocksVisitor>();
        internal static LStripBlocksVisitor Instance => _Instance.Value;

        internal bool Strip { get; private set; }

        private readonly Queue<WhiteSpaceNode> pendingWhiteSpace = new Queue<WhiteSpaceNode>();

        public LStripBlocksVisitor()
        {
        }

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
            Strip = true;
        }

        public void Transform(OutputNode item)
        {
            pendingWhiteSpace.Clear();
        }

        public void Transform(WhiteSpaceNode item)
        {
            if(Strip)
            {
                pendingWhiteSpace.Enqueue(item);
            }
        }

        public void Transform(IfNode item)
        {
            foreach (var condition in item.Conditions)
            {
                StripWhiteSpace();
                condition.Transform(this);
            }
            StripWhiteSpace();
        }

        public void Transform(ConditionalNode item)
        {
            StripWhiteSpace();
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

        private void StripWhiteSpace()
        {
            while(pendingWhiteSpace.Count > 0)
            {
                var ws = pendingWhiteSpace.Dequeue();
                ws.WhiteSpaceMode = WhiteSpaceMode.Trim;
            }
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

        public void Transform(IncludeNode item)
        {
            throw new NotImplementedException();
        }

        public void Transform(ImportNode item)
        {
            throw new NotImplementedException();
        }

        public void Transform(FromNode item)
        {
            throw new NotImplementedException();
        }
    }
}
