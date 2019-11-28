using Obsidian.AST;
using Obsidian.AST.Nodes;
using Obsidian.AST.Nodes.MiscNodes;
using Obsidian.AST.Nodes.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obsidian.Transforming
{
    public class BaseASTTransformer : ITransformVisitor<ASTNode>
    {
        protected virtual IEnumerable<ASTNode> TransformAll(IEnumerable<ASTNode> nodes)
        {
            return nodes.Select(node => node.Transform(this));
        }


        public virtual ASTNode Transform(TemplateNode item)
        {
            return new TemplateNode(TransformAll(item.Children));
        }

        public virtual ASTNode Transform(ForNode item)
        {
            var primaryBlock = item.PrimaryBlock.Transform(this) as ContainerNode;
            if (primaryBlock == default) throw new NotImplementedException();
            ContainerNode? elseBlock = null;
            if(item.ElseBlock != null)
            {
                elseBlock = item.ElseBlock.Transform(this) as ContainerNode;
                if (elseBlock == default) throw new NotImplementedException();
            }
            return new ForNode(primaryBlock, elseBlock, item.VariableNames, item.Expression, item.StartWhiteSpace, item.EndWhiteSpace);
        }

        public virtual ASTNode Transform(ContainerNode item)
        {
            return new ContainerNode(TransformAll(item.Children), item.StartWhiteSpace, item.EndWhiteSpace);
        }

        public virtual ASTNode Transform(ExpressionNode item)
        {
            return item;
        }

        public virtual ASTNode Transform(NewLineNode item)
        {
            return item;
        }

        public virtual ASTNode Transform(OutputNode item)
        {
            return item;
        }

        public virtual ASTNode Transform(WhiteSpaceNode item)
        {
            return item;
        }

        public virtual ASTNode Transform(IfNode item)
        {
            var x = item.Conditions.Select(cond => new ConditionalNode(cond.Expression, TransformAll(cond.Children), cond.StartWhiteSpace, cond.EndWhiteSpace));
            return new IfNode(x, item.StartWhiteSpace, item.EndWhiteSpace);
        }

        public virtual ASTNode Transform(ConditionalNode item)
        {
            return new ConditionalNode(item.Expression, TransformAll(item.Children), item.StartWhiteSpace, item.EndWhiteSpace);
        }

        public virtual ASTNode Transform(CommentNode item)
        {
            return item;
        }

        public virtual ASTNode Transform(BlockNode item)
        {
            var container = new ContainerNode(TransformAll(item.BlockContents.Children), item.StartWhiteSpace, item.EndWhiteSpace);
            return new BlockNode(item.Name, container, item.StartWhiteSpace, item.EndWhiteSpace);
        }

        public virtual ASTNode Transform(ExtendsNode item)
        {
            return item;
        }

        public ASTNode Transform(EmptyNode item)
        {
            return item;
        }
    }
}
