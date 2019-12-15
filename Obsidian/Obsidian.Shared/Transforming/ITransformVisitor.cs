using System;
using System.Collections.Generic;
using System.Text;
using Obsidian.AST.Nodes;
using Obsidian.AST.Nodes.MiscNodes;
using Obsidian.AST.Nodes.Statements;

namespace Obsidian.Transforming
{
    internal interface ITransformVisitor<TOutput>
    {
        TOutput Transform(TemplateNode item);
        TOutput Transform(EmptyNode emptyNode);
        TOutput Transform(ForNode item);
        TOutput Transform(ContainerNode item);
        TOutput Transform(ExpressionNode item);
        TOutput Transform(NewLineNode item);
        TOutput Transform(OutputNode item);
        TOutput Transform(WhiteSpaceNode item);
        TOutput Transform(IfNode item);
        TOutput Transform(ConditionalNode item);
        TOutput Transform(CommentNode item);
        TOutput Transform(BlockNode item);
        TOutput Transform(ExtendsNode item);
        TOutput Transform(RawNode item);
        TOutput Transform(MacroNode item);
        TOutput Transform(CallNode item);
        TOutput Transform(FilterNode item);
        TOutput Transform(SetNode item);
        TOutput Transform(IncludeNode item);
        TOutput Transform(ImportNode item);
    }

    internal interface ITransformVisitor
    {
        void Transform(TemplateNode item);
        void Transform(EmptyNode emptyNode);
        void Transform(ForNode item);
        void Transform(ContainerNode item);
        void Transform(ExpressionNode item);
        void Transform(NewLineNode item);
        void Transform(OutputNode item);
        void Transform(WhiteSpaceNode item);
        void Transform(IfNode item);
        void Transform(ConditionalNode item);
        void Transform(CommentNode item);
        void Transform(BlockNode item);
        void Transform(ExtendsNode item);
        void Transform(RawNode item);
        void Transform(MacroNode item);
        void Transform(CallNode item);
        void Transform(FilterNode item);
        void Transform(SetNode item);
        void Transform(IncludeNode item);
        void Transform(ImportNode item);
    }
}
