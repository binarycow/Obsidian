﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Obsidian.AST;
//using Obsidian.AST.Nodes;
//using Obsidian.AST.Nodes.MiscNodes;
//using Obsidian.AST.Nodes.Statements;
//using Obsidian.Transforming;

//namespace Obsidian.CommentRemover
//{
//    public class CommentRemoverTransformer : ITransformVisitor<ASTNode>
//    {

//        private IEnumerable<ASTNode> TransformAll(IEnumerable<ASTNode> items)
//        {
//            return items.Select(child => child.Transform(this));
//        }
//        private CommentRemoverTransformer()
//        {

//        }
//        public static Lazy<CommentRemoverTransformer> _Instance = new Lazy<CommentRemoverTransformer>(() => new CommentRemoverTransformer());
//        public static CommentRemoverTransformer Instance => _Instance.Value;
//        public ASTNode Transform(ForNode item)
//        {
//            ContainerNode? elseBlock = null;
//            if(item.ElseBlock != null)
//            {
//                elseBlock = new ContainerNode(TransformAll(item.ElseBlock.Children));
//            }
//            var primaryBlock = new ContainerNode(TransformAll(item.PrimaryBlock.Children));
//            return new ForNode(primaryBlock, elseBlock, item.VariableNames, item.Expression);
//        }

//        public ASTNode Transform(ContainerNode item)
//        {
//            return new ContainerNode(TransformAll(item.Children));
//        }

//        public ASTNode Transform(ExpressionNode item)
//        {
//            return item;
//        }

//        public ASTNode Transform(NewLineNode item)
//        {
//            return item;
//        }

//        public ASTNode Transform(OutputNode item)
//        {
//            return item;
//        }

//        public ASTNode Transform(WhiteSpaceNode item)
//        {
//            return item;
//        }

//        public ASTNode Transform(IfNode item)
//        {
//            return item;
//        }

//        public ASTNode Transform(ConditionalNode item)
//        {
//            return new ConditionalNode(item.Expression, TransformAll(item.Children));
//        }

//        public ASTNode Transform(CommentNode item)
//        {
//            throw new NotImplementedException();
//        }

//        public ASTNode Transform(BlockNode item)
//        {
//            var contents = new ContainerNode(TransformAll(item.BlockContents.Children));
//            return new BlockNode(item.Name, contents);
//        }
//        public ASTNode Transform(ExtendsNode item)
//        {
//            return item;
//        }

//        public ASTNode Transform(TemplateNode item)
//        {
//            return item;
//        }

//        public ASTNode Transform(EmptyNode emptyNode)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
