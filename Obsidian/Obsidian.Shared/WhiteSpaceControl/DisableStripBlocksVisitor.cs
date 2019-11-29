using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Obsidian.AST;
using Obsidian.AST.Nodes;
using Obsidian.AST.Nodes.MiscNodes;
using Obsidian.AST.Nodes.Statements;
using Obsidian.Transforming;

namespace Obsidian.WhiteSpaceControl
{
    public class DisableStripBlocksVisitor : ITransformVisitor<ASTNode>
    {
        public static Lazy<DisableStripBlocksVisitor> _Instance = new Lazy<DisableStripBlocksVisitor>();
        public static DisableStripBlocksVisitor Instance => _Instance.Value;
        public ASTNode Transform(ForNode item)
        {
            return item;
        }


        public ASTNode Transform(TemplateNode item)
        {
            // TODO: See if we can fix all these switch statements...
            var children = new Queue<ASTNode>();
            var pendingWhiteSpace = new Queue<WhiteSpaceNode>();
            foreach (var child in item.Children)
            {
                switch (child)
                {
                    case CommentNode _:
                        break;
                    case NewLineNode _:
                        children.Enqueue(pendingWhiteSpace);
                        pendingWhiteSpace.Clear();
                        children.Enqueue(child);
                        break;
                    case WhiteSpaceNode whiteSpaceNode:
                        pendingWhiteSpace.Enqueue(whiteSpaceNode);
                        break;
                    case ContainerNode _:
                    case OutputNode _:
                    case ExtendsNode _:
                        children.Enqueue(pendingWhiteSpace);
                        pendingWhiteSpace.Clear();
                        children.Enqueue(child);
                        break;
                    case AbstractContainerNode containerNode:
                        if (containerNode.StartWhiteSpace == WhiteSpaceControlMode.Keep)
                        {
                            children.Enqueue(pendingWhiteSpace.Select(ws =>
                            {
                                ws.WhiteSpaceControlMode = WhiteSpaceControlMode.Keep;
                                return ws;
                            }));
                        }
                        children.Enqueue(pendingWhiteSpace);
                        pendingWhiteSpace.Clear();
                        children.Enqueue(containerNode.Transform(this));
                        break;
                    case StatementNode statementNode:
                        if (statementNode.StartWhiteSpace == WhiteSpaceControlMode.Keep)
                        {
                            children.Enqueue(pendingWhiteSpace.Select(ws =>
                            {
                                ws.WhiteSpaceControlMode = WhiteSpaceControlMode.Keep;
                                return ws;
                            }));
                        }
                        children.Enqueue(pendingWhiteSpace);
                        pendingWhiteSpace.Clear();
                        children.Enqueue(statementNode.Transform(this));
                        break;
                    case ExpressionNode expressionNode:
                        if (expressionNode.StartWhiteSpace == WhiteSpaceControlMode.Keep)
                        {
                            children.Enqueue(pendingWhiteSpace.Select(ws =>
                            {
                                ws.WhiteSpaceControlMode = WhiteSpaceControlMode.Keep;
                                return ws;
                            }));
                        }
                        children.Enqueue(pendingWhiteSpace);
                        pendingWhiteSpace.Clear();
                        children.Enqueue(expressionNode.Transform(this));
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            return new TemplateNode(children);
        }

        public ASTNode Transform(ContainerNode item)
        {
            // TODO: See if we can fix all these switch statements...
            var children = new Queue<ASTNode>();
            var pendingWhiteSpace = new Queue<WhiteSpaceNode>();
            foreach(var child in item.Children)
            {
                switch(child)
                {
                    case NewLineNode _:
                        children.Enqueue(pendingWhiteSpace);
                        pendingWhiteSpace.Clear();
                        children.Enqueue(child);
                        break;
                    case WhiteSpaceNode whiteSpaceNode:
                        pendingWhiteSpace.Enqueue(whiteSpaceNode);
                        break;
                    case ContainerNode _:
                    case OutputNode _:
                    case ExtendsNode _:
                        children.Enqueue(pendingWhiteSpace);
                        pendingWhiteSpace.Clear();
                        children.Enqueue(child);
                        break;
                    case AbstractContainerNode containerNode:
                        if (containerNode.StartWhiteSpace == WhiteSpaceControlMode.Keep)
                        {
                            children.Enqueue(pendingWhiteSpace.Select(ws =>
                            {
                                ws.WhiteSpaceControlMode = WhiteSpaceControlMode.Keep;
                                return ws;
                            }));
                        }
                        children.Enqueue(pendingWhiteSpace);
                        pendingWhiteSpace.Clear();
                        children.Enqueue(containerNode.Transform(this));
                        break;
                    case StatementNode statementNode:
                        if (statementNode.StartWhiteSpace == WhiteSpaceControlMode.Keep)
                        {
                            children.Enqueue(pendingWhiteSpace.Select(ws =>
                            {
                                ws.WhiteSpaceControlMode = WhiteSpaceControlMode.Keep;
                                return ws;
                            }));
                        }
                        children.Enqueue(pendingWhiteSpace);
                        pendingWhiteSpace.Clear();
                        children.Enqueue(statementNode.Transform(this));
                        break;
                    case ExpressionNode expressionNode:
                        if (expressionNode.StartWhiteSpace == WhiteSpaceControlMode.Keep)
                        {
                            children.Enqueue(pendingWhiteSpace.Select(ws =>
                            {
                                ws.WhiteSpaceControlMode = WhiteSpaceControlMode.Keep;
                                return ws;
                            }));
                        }
                        children.Enqueue(pendingWhiteSpace);
                        pendingWhiteSpace.Clear();
                        children.Enqueue(expressionNode.Transform(this));
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            return new ContainerNode(children, item.StartWhiteSpace, item.EndWhiteSpace);
        }

        public ASTNode Transform(BlockNode item)
        {
            return item;
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
            return item;
        }

        public ASTNode Transform(CommentNode item)
        {
            return item;
        }
        public ASTNode Transform(ExtendsNode item)
        {
            throw new NotImplementedException();
        }

        public ASTNode Transform(EmptyNode emptyNode)
        {
            throw new NotImplementedException();
        }
    }
}
