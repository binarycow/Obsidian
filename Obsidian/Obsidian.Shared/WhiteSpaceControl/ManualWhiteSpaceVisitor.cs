using System;
using System.Collections.Generic;
using System.Text;
using Common.Collections;
using Obsidian.AST;
using Obsidian.AST.Nodes;
using Obsidian.AST.Nodes.MiscNodes;
using Obsidian.AST.Nodes.Statements;
using Obsidian.Transforming;

namespace Obsidian.WhiteSpaceControl
{
    internal class ManualWhiteSpaceVisitor : IManualWhiteSpaceTransformVisitor
    {
        private static readonly Lazy<ManualWhiteSpaceVisitor> _Instance = new Lazy<ManualWhiteSpaceVisitor>(() => new ManualWhiteSpaceVisitor());
        internal static ManualWhiteSpaceVisitor Instance => _Instance.Value;

        readonly List<IWhiteSpace> _PendingWhiteSpace = new List<IWhiteSpace>();


        private WhiteSpaceMode _WhiteSpaceMode = WhiteSpaceMode.Default;


        private void TransformAll(IEnumerable<ASTNode> children)
        {
            foreach(var item in children)
            {
                item.Transform(this);
            }
        }

        private void SetTrim(WhiteSpaceMode? mode = null)
        {
            var modeToSet = mode ?? _WhiteSpaceMode;
            if (_WhiteSpaceMode != modeToSet)
            {
                foreach (var item in _PendingWhiteSpace)
                {
                    item.WhiteSpaceMode = modeToSet;
                }
            }
            _PendingWhiteSpace.Clear();
            _WhiteSpaceMode = WhiteSpaceMode.Default;
        }


        public void Transform(TemplateNode item, bool inner = false)
        {
            TransformAll(item.Children);
        }

        public void Transform(EmptyNode emptyNode, bool inner = false)
        {
            return;
        }

        public void Transform(ForNode item, bool inner = false)
        {
            SetTrim(item.WhiteSpaceControl.Start);
            item.PrimaryBlock.Transform(this, inner: true);
            item.ElseBlock?.Transform(this, inner: true);
            _WhiteSpaceMode = item.WhiteSpaceControl.End;
        }

        public void Transform(ContainerNode item, bool inner = false)
        {
            if(inner)
                _WhiteSpaceMode = item.WhiteSpaceControl.Start;
            else
                SetTrim(item.WhiteSpaceControl.Start);
            TransformAll(item.Children);
            if (inner)
                SetTrim(item.WhiteSpaceControl.End);
            else
                _WhiteSpaceMode = item.WhiteSpaceControl.End;
        }

        public void Transform(ExpressionNode item, bool inner = false)
        {
            SetTrim();
        }

        public void Transform(NewLineNode item, bool inner = false)
        {
            if (_WhiteSpaceMode != WhiteSpaceMode.Default)
            {
                item.WhiteSpaceMode = _WhiteSpaceMode;
                return;
            }
            _PendingWhiteSpace.Add(item);
        }

        public void Transform(OutputNode item, bool inner = false)
        {
            SetTrim();
        }

        public void Transform(WhiteSpaceNode item, bool inner = false)
        {
            if(_WhiteSpaceMode != WhiteSpaceMode.Default)
            {
                item.WhiteSpaceMode = _WhiteSpaceMode;
                return;
            }
            _PendingWhiteSpace.Add(item);
        }

        public void Transform(IfNode item, bool inner = false)
        {
            SetTrim();
        }

        public void Transform(ConditionalNode item, bool inner = false)
        {
            SetTrim();
        }

        public void Transform(CommentNode item, bool inner = false)
        {
            SetTrim();
        }

        public void Transform(BlockNode item, bool inner = false)
        {
            SetTrim();
        }

        public void Transform(ExtendsNode item, bool inner = false)
        {
            SetTrim();
        }

        public void Transform(RawNode item, bool inner = false)
        {
            SetTrim();
        }
        public void Transform(MacroNode item, bool inner = false)
        {
            SetTrim(item.WhiteSpaceControl.Start);
            item.Contents.Transform(this, inner: true);
            _WhiteSpaceMode = item.WhiteSpaceControl.End;
        }

        public void Transform(CallNode item, bool inner = false)
        {
            SetTrim();
        }

        public void Transform(FilterNode item, bool inner = false)
        {
            SetTrim();
        }

        public void Transform(SetNode item, bool inner = false)
        {
            SetTrim();
        }

        public void Transform(IncludeNode item, bool inner = false)
        {
            SetTrim();
        }
    }
}
