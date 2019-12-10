using System;
using System.Collections.Generic;
using System.Text;
using Obsidian.AST.Nodes;
using Obsidian.AST.Nodes.MiscNodes;
using Obsidian.AST.Nodes.Statements;

namespace Obsidian.Transforming
{
    internal interface IManualWhiteSpaceTransformVisitor
    {
        void Transform(TemplateNode item, bool inner = false);
        void Transform(EmptyNode emptyNode, bool inner = false);
        void Transform(ForNode item, bool inner = false);
        void Transform(ContainerNode item, bool inner = false);
        void Transform(ExpressionNode item, bool inner = false);
        void Transform(NewLineNode item, bool inner = false);
        void Transform(OutputNode item, bool inner = false);
        void Transform(WhiteSpaceNode item, bool inner = false);
        void Transform(IfNode item, bool inner = false);
        void Transform(ConditionalNode item, bool inner = false);
        void Transform(CommentNode item, bool inner = false);
        void Transform(BlockNode item, bool inner = false);
        void Transform(ExtendsNode item, bool inner = false);
        void Transform(RawNode item, bool inner = false);
        void Transform(MacroNode item, bool inner = false);
        void Transform(CallNode item, bool inner = false);
        void Transform(FilterNode item, bool inner = false);
        void Transform(SetNode item, bool inner = false);
        void Transform(IncludeNode item, bool inner = false);
    }
}
