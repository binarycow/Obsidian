using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obsidian.AST.Nodes;
using Obsidian.AST.Nodes.MiscNodes;
using Obsidian.AST.Nodes.Statements;

namespace Obsidian.Transforming
{
    internal interface IForceTransformVisitor<TOutput>
    {
        TOutput Transform(TemplateNode item, bool force);
        TOutput Transform(ForNode item, bool force);
        TOutput Transform(ContainerNode item, bool force);
        TOutput Transform(ExpressionNode item, bool force);
        TOutput Transform(EmptyNode emptyNode, bool force);
        TOutput Transform(NewLineNode item, bool force);
        TOutput Transform(OutputNode item, bool force);
        TOutput Transform(WhiteSpaceNode item, bool force);
        TOutput Transform(IfNode item, bool force);
        TOutput Transform(ConditionalNode item, bool force);
        TOutput Transform(CommentNode item, bool force);
        TOutput Transform(BlockNode item, bool force);
        TOutput Transform(ExtendsNode item, bool force);
        TOutput Transform(RawNode item, bool force);
        TOutput Transform(MacroNode item, bool force);
        TOutput Transform(CallNode item, bool force);
        TOutput Transform(FilterNode item, bool force);
        TOutput Transform(SetNode item, bool force);
        TOutput Transform(IncludeNode item, bool force);
        TOutput Transform(ImportNode item, bool force);
        TOutput Transform(FromNode item, bool force);
    }
}
