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
    public class ManualTrimAfterVisitor : ITransformVisitor<ASTNode>
    {
        public static Lazy<ManualTrimAfterVisitor> _Instance = new Lazy<ManualTrimAfterVisitor>();
        public static ManualTrimAfterVisitor Instance => _Instance.Value;
        public ASTNode Transform(ForNode item)
        {
            if (item.PrimaryBlock.Transform(this).TryConvert<ContainerNode>(out var primaryBlock) == false)
            {
                throw new NotImplementedException();
            }
            var elseBlock = item.ElseBlock?.Transform(this) as ContainerNode;
            return new ForNode(primaryBlock, elseBlock, item.VariableNames, item.Expression, item.StartWhiteSpace, item.EndWhiteSpace);
        }

        public ASTNode Transform(TemplateNode item)
        {
            var children = new Queue<ASTNode>();
            var pending = new Queue<ASTNode>();
            foreach (var child in item.Children)
            {
                switch (child)
                {
                    case NewLineNode _:
                    case WhiteSpaceNode _:
                        pending.Enqueue(child);
                        break;
                    default:
                        children.Enqueue(pending);
                        children.Enqueue(child.Transform(this));
                        break;
                }
            }
            return new TemplateNode(children);
        }

        public ASTNode Transform(ContainerNode item)
        {
            if(item.EndWhiteSpace != WhiteSpaceControlMode.Trim)
            {
                return new ContainerNode(item.Children.Select(child => child.Transform(this)), item.StartWhiteSpace, item.EndWhiteSpace);
            }

            var children = new Queue<ASTNode>();
            var pending = new Queue<ASTNode>();
            foreach(var child in item.Children)
            {
                switch(child)
                {
                    case NewLineNode _:
                    case WhiteSpaceNode _:
                        pending.Enqueue(child);
                        break;
                    default:
                        children.Enqueue(pending);
                        children.Enqueue(child.Transform(this));
                        break;
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
        public ASTNode Transform(BlockNode item)
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
            return item;
        }

    }
}
