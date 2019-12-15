//using System;
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
//    internal class CommentRemoverTransformer : ITransformVisitor<ASTNode>
//    {

//        private IEnumerable<ASTNode> TransformAll(IEnumerable<ASTNode> items)
//        {
//            return items.Select(child => child.Transform(this));
//        }
//        private CommentRemoverTransformer()
//        {

//        }
//        internal static Lazy<CommentRemoverTransformer> _Instance = new Lazy<CommentRemoverTransformer>(() => new CommentRemoverTransformer());
//        internal static CommentRemoverTransformer Instance => _Instance.Value;
//        internal ASTNode Transform(ForNode item)
//        {
//            ContainerNode? elseBlock = null;
//            if(item.ElseBlock != null)
//            {
//                elseBlock = new ContainerNode(TransformAll(item.ElseBlock.Children));
//            }
//            var primaryBlock = new ContainerNode(TransformAll(item.PrimaryBlock.Children));
//            return new ForNode(primaryBlock, elseBlock, item.VariableNames, item.Expression);
//        }

//        internal ASTNode Transform(ContainerNode item)
//        {
//            return new ContainerNode(TransformAll(item.Children));
//        }

//        internal ASTNode Transform(ExpressionNode item)
//        {
//            return item;
//        }

//        internal ASTNode Transform(NewLineNode item)
//        {
//            return item;
//        }

//        internal ASTNode Transform(OutputNode item)
//        {
//            return item;
//        }

//        internal ASTNode Transform(WhiteSpaceNode item)
//        {
//            return item;
//        }

//        internal ASTNode Transform(IfNode item)
//        {
//            return item;
//        }

//        internal ASTNode Transform(ConditionalNode item)
//        {
//            return new ConditionalNode(item.Expression, TransformAll(item.Children));
//        }

//        internal ASTNode Transform(CommentNode item)
//        {
//            throw new NotImplementedException();
//        }

//        internal ASTNode Transform(BlockNode item)
//        {
//            var contents = new ContainerNode(TransformAll(item.BlockContents.Children));
//            return new BlockNode(item.Name, contents);
//        }
//        internal ASTNode Transform(ExtendsNode item)
//        {
//            return item;
//        }

//        internal ASTNode Transform(TemplateNode item)
//        {
//            return item;
//        }

//        internal ASTNode Transform(EmptyNode emptyNode)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
