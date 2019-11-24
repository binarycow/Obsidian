﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obsidian.AST;
using Obsidian.AST.Nodes;
using Obsidian.AST.Nodes.MiscNodes;
using Obsidian.AST.Nodes.Statements;
using Obsidian.Transforming;

namespace Obsidian.CommentRemover
{
    public class CommentRemoverTransformer : ITransformVisitor<ASTNode>
    {

        private IEnumerable<ASTNode> TransformExceptComment(IEnumerable<ASTNode> items)
        {
            foreach(var item in items)
            {
                if(item is CommentNode _)
                {
                    continue;
                }
                yield return item.Transform(this);
            }
        }
        private CommentRemoverTransformer()
        {

        }
        public static Lazy<CommentRemoverTransformer> _Instance = new Lazy<CommentRemoverTransformer>(() => new CommentRemoverTransformer());
        public static CommentRemoverTransformer Instance => _Instance.Value;
        public ASTNode Transform(ForNode item)
        {
            ContainerNode? elseBlock = null;
            if(item.ElseBlock != null)
            {
                elseBlock = new ContainerNode(TransformExceptComment(item.ElseBlock.Children), item.ElseBlock.StartWhiteSpace, item.ElseBlock.EndWhiteSpace);
            }
            var primaryBlock = new ContainerNode(TransformExceptComment(item.PrimaryBlock.Children), item.PrimaryBlock.StartWhiteSpace, item.PrimaryBlock.EndWhiteSpace);
            return new ForNode(primaryBlock, elseBlock, item.VariableNames, item.Expression, item.StartWhiteSpace, item.EndWhiteSpace);
        }

        public ASTNode Transform(ContainerNode item)
        {
            return new ContainerNode(TransformExceptComment(item.Children), item.StartWhiteSpace, item.EndWhiteSpace);
        }

        public ASTNode Transform(ExpressionNode item)
        {
            return item;
        }

        public ASTNode Transform(NewLineNode item)
        {
            return item;
        }

        public ASTNode Transform(OutputNode item)
        {
            return item;
        }

        public ASTNode Transform(WhiteSpaceNode item)
        {
            return item;
        }

        public ASTNode Transform(IfNode item)
        {
            return item;
        }

        public ASTNode Transform(ConditionalNode item)
        {
            return new ConditionalNode(item.Expression, TransformExceptComment(item.Children), item.StartWhiteSpace, item.EndWhiteSpace);
        }

        public ASTNode Transform(CommentNode item)
        {
            throw new NotImplementedException();
        }
    }
}