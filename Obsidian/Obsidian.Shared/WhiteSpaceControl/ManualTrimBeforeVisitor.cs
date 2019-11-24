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
    public class ManualTrimBeforeVisitor : ITransformVisitor<ASTNode>
    {
        public static Lazy<ManualTrimBeforeVisitor> _Instance = new Lazy<ManualTrimBeforeVisitor>();
        public static ManualTrimBeforeVisitor Instance => _Instance.Value;
        public ASTNode Transform(ForNode item)
        {
            if (item.PrimaryBlock.Transform(this).TryConvert<ContainerNode>(out var primaryBlock) == false)
            {
                throw new NotImplementedException();
            }
            var elseBlock = item.ElseBlock?.Transform(this) as ContainerNode;
            return new ForNode(primaryBlock, elseBlock, item.VariableNames, item.Expression, item.StartWhiteSpace, item.EndWhiteSpace);
        }

        public ASTNode Transform(ContainerNode item)
        {
            var children = new Queue<ASTNode>();
            if (item.StartWhiteSpace != WhiteSpaceControlMode.Trim)
            {
                foreach (var child in item.Children)
                {
                    children.Enqueue(child.Transform(this));
                }
            }
            else
            {
                var trim = true;
                foreach (var child in item.Children)
                {
                    switch (child)
                    {
                        case WhiteSpaceNode whiteSpaceNode:
                        case NewLineNode newLineNode:
                            if(trim == false)
                            {
                                children.Enqueue(child.Transform(this));
                            }
                            break;
                        default:
                            trim = false;
                            children.Enqueue(child.Transform(this));
                            break;
                    }
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

        public ASTNode Transform(ConditionalNode item)
        {
            return item;
        }

        public ASTNode Transform(CommentNode item)
        {
            return item;
        }
    }
}
