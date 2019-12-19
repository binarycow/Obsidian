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
    internal class BaseASTTransformer : ITransformVisitor<ASTNode>
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
            if(item.Expression != null)
            {
                return new ForNode(primaryBlock, elseBlock, item.VariableNames, item.Expression, item.Filter, item.Recursive, item.EndParsingNode, item.WhiteSpaceControl);
            }
            else
            {
                return new ForNode(primaryBlock, elseBlock, item.VariableNames, item.AlreadyEvaluatedObject, item.Filter, item.Recursive, item.EndParsingNode, item.WhiteSpaceControl);
            }

        }

        public virtual ASTNode Transform(ContainerNode item)
        {
            return new ContainerNode(item.StartParsingNode, TransformAll(item.Children), item.EndParsingNode);
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
            var x = item.Conditions.Select(
                cond => new ConditionalNode(cond.StartParsingNode, cond.Expression, TransformAll(cond.Children), cond.EndParsingNode)
            );
            return new IfNode(item.StartParsingNode, x, item.EndParsingNode);
        }

        public virtual ASTNode Transform(ConditionalNode item)
        {
            return new ConditionalNode(item.StartParsingNode, item.Expression, TransformAll(item.Children), item.EndParsingNode);
        }

        public virtual ASTNode Transform(CommentNode item)
        {
            return item;
        }

        public virtual ASTNode Transform(BlockNode item)
        {
            var container = new ContainerNode(item.BlockContents.StartParsingNode, TransformAll(item.BlockContents.Children), item.BlockContents.EndParsingNode);
            return new BlockNode(item.StartParsingNode, item.Name, container, item.EndParsingNode);
        }

        public virtual ASTNode Transform(ExtendsNode item)
        {
            return item;
        }

        public ASTNode Transform(EmptyNode item)
        {
            return item;
        }

        public ASTNode Transform(RawNode item)
        {
            throw new NotImplementedException();
        }

        public ASTNode Transform(MacroNode item)
        {
            throw new NotImplementedException();
        }

        public ASTNode Transform(CallNode item)
        {
            throw new NotImplementedException();
        }

        public ASTNode Transform(FilterNode item)
        {
            throw new NotImplementedException();
        }

        public ASTNode Transform(SetNode item)
        {
            throw new NotImplementedException();
        }

        public ASTNode Transform(IncludeNode item)
        {
            throw new NotImplementedException();
        }

        public ASTNode Transform(ImportNode item)
        {
            throw new NotImplementedException();
        }

        public ASTNode Transform(FromNode item)
        {
            throw new NotImplementedException();
        }
    }
}
