using System;
using System.Collections.Generic;
using System.Text;
using Obsidian.AST.Nodes;
using Obsidian.AST.Nodes.MiscNodes;
using Obsidian.AST.Nodes.Statements;

namespace Obsidian.Transforming
{
    public interface ITransformVisitor<TOutput>
    {
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
    }
}
