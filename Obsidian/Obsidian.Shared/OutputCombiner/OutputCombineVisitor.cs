using System;
using System.Collections.Generic;
using System.Text;
using Common;
using Obsidian.AST;
using Obsidian.AST.Nodes;
using Obsidian.AST.Nodes.MiscNodes;
using Obsidian.AST.Nodes.Statements;
using Obsidian.Transforming;

namespace Obsidian.OutputCombiner
{
    public class OutputCombineVisitor : ITransformVisitor<ASTNode>
    {
        public static Lazy<OutputCombineVisitor> _Instance = new Lazy<OutputCombineVisitor>();
        public static OutputCombineVisitor Instance => _Instance.Value;

        public ASTNode Transform(ForNode item)
        {
            var primaryBlock = item.PrimaryBlock.Transform(this) as ContainerNode ?? throw new NotImplementedException();
            var elseBlock = item.ElseBlock?.Transform(this) as ContainerNode;

            return new ForNode(primaryBlock, elseBlock, item.VariableNames, item.Expression, item.StartWhiteSpace, item.EndWhiteSpace);
        }

        public ASTNode Transform(ContainerNode item)
        {
            using var checkoutItem = StringBuilderPool.Instance.Checkout();
            var stringBuilder = checkoutItem.CheckedOutObject;

            var children = new Queue<ASTNode>();
            var pendingOutput = new Queue<ASTNode>();
            foreach(var child in item.Children)
            {
                switch(child)
                {
                    case NewLineNode _:
                    case OutputNode _:
                    case WhiteSpaceNode _:
                        pendingOutput.Enqueue(child);
                        break;
                    case ForNode _:
                    case ExpressionNode _:
                    case IfNode _:
                    case ConditionalNode _:
                    case BlockNode _:
                    case ContainerNode _:
                        if (pendingOutput.Count > 0)
                        {
                            do
                            {
                                stringBuilder.Append(pendingOutput.Dequeue().ToString());
                            } while (pendingOutput.Count > 0);
                            children.Enqueue(OutputNode.FromString(stringBuilder.ToString()));
                            stringBuilder.Clear();
                        }
                        children.Enqueue(child.Transform(this));
                        break;
                    case CommentNode _:
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            return new ContainerNode(children, item.StartWhiteSpace, item.EndWhiteSpace);
        }

        public ASTNode Transform(ExpressionNode item)
        {
            return item;
        }

        public ASTNode Transform(NewLineNode item)
        {
            throw new NotImplementedException();
        }

        public ASTNode Transform(OutputNode item)
        {
            throw new NotImplementedException();
        }

        public ASTNode Transform(WhiteSpaceNode item)
        {
            throw new NotImplementedException();
        }

        public ASTNode Transform(IfNode item)
        {
            throw new NotImplementedException();
        }

        public ASTNode Transform(ConditionalNode item)
        {
            throw new NotImplementedException();
        }

        public ASTNode Transform(CommentNode item)
        {
            throw new NotImplementedException();
        }

        public ASTNode Transform(BlockNode item)
        {
            using var checkoutItem = StringBuilderPool.Instance.Checkout();
            var stringBuilder = checkoutItem.CheckedOutObject;

            var children = new Queue<ASTNode>();
            var pendingOutput = new Queue<ASTNode>();
            foreach (var child in item.Children)
            {
                switch (child)
                {
                    case NewLineNode _:
                    case OutputNode _:
                    case WhiteSpaceNode _:
                        pendingOutput.Enqueue(child);
                        break;
                    case ForNode _:
                    case ExpressionNode _:
                    case IfNode _:
                    case ConditionalNode _:
                    case BlockNode _:
                    case ContainerNode _:
                        if (pendingOutput.Count > 0)
                        {
                            do
                            {
                                stringBuilder.Append(pendingOutput.Dequeue().ToString());
                            } while (pendingOutput.Count > 0);
                            children.Enqueue(OutputNode.FromString(stringBuilder.ToString()));
                            stringBuilder.Clear();
                        }
                        children.Enqueue(child.Transform(this));
                        break;
                    case CommentNode _:
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            return new ContainerNode(children, item.StartWhiteSpace, item.EndWhiteSpace);
        }

        public ASTNode Transform(ExtendsNode item)
        {
            throw new NotImplementedException();
        }

        public ASTNode Transform(TemplateNode item)
        {
            throw new NotImplementedException();
        }

        public ASTNode Transform(EmptyNode emptyNode)
        {
            throw new NotImplementedException();
        }
    }
}
