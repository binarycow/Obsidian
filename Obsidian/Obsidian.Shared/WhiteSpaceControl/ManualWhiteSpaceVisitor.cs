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
    internal class ManualWhiteSpaceVisitor : ITransformVisitor
    {
        private static Lazy<ManualWhiteSpaceVisitor> _Instance = new Lazy<ManualWhiteSpaceVisitor>(() => new ManualWhiteSpaceVisitor());
        public static ManualWhiteSpaceVisitor Instance => _Instance.Value;

        List<IWhiteSpace> _PendingWhiteSpace = new List<IWhiteSpace>();

        private void SetTrim(WhiteSpaceMode mode)
        {
            if (mode == WhiteSpaceMode.Default) return;
            foreach(var ws in _PendingWhiteSpace)
            {
                ws.WhiteSpaceMode = mode;
            }
            _PendingWhiteSpace.Clear();
        }

        private void TransformAll(IEnumerable<ASTNode> children)
        {
            foreach(var child in children)
            {
                child.Transform(this);
            }
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
            return;
        }

        public void Transform(ForNode item)
        {
            SetTrim(item.WhiteSpaceControl.Start);
            item.PrimaryBlock.Transform(this);
            item.ElseBlock?.Transform(this);
            SetTrim(item.WhiteSpaceControl.End);
        }

        public void Transform(ContainerNode item)
        {
            SetTrim(item.WhiteSpaceControl.Start);
            TransformAll(item.Children);
            SetTrim(item.WhiteSpaceControl.End);
        }

        public void Transform(ExpressionNode item)
        {
            return;
        }

        public void Transform(NewLineNode item)
        {
            _PendingWhiteSpace.Add(item);
        }

        public void Transform(OutputNode item)
        {
            return;
        }

        public void Transform(WhiteSpaceNode item)
        {
            _PendingWhiteSpace.Add(item);
        }

        public void Transform(IfNode item)
        {
            SetTrim(item.WhiteSpaceControl.Start);
            TransformAll(item.Conditions);
            SetTrim(item.WhiteSpaceControl.End);
        }

        public void Transform(ConditionalNode item)
        {
            SetTrim(item.WhiteSpaceControl.Start);
            TransformAll(item.Children);
            SetTrim(item.WhiteSpaceControl.End);
        }

        public void Transform(CommentNode item)
        {
            return;
        }

        public void Transform(BlockNode item)
        {
            return;
        }

        public void Transform(ExtendsNode item)
        {
            return;
        }

        public void Transform(RawNode item)
        {
            return;
        }
        public void Transform(MacroNode item)
        {
            SetTrim(item.WhiteSpaceControl.Start);
            item.Contents.Transform(this);
            SetTrim(item.WhiteSpaceControl.End);
        }
    }
}
