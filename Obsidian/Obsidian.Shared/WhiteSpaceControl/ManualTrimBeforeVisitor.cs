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
    public class ManualTrimBeforeVisitor : ITransformVisitor<IEnumerable<ASTNode>>
    {
        public static Lazy<ManualTrimBeforeVisitor> _Instance = new Lazy<ManualTrimBeforeVisitor>();
        public static ManualTrimBeforeVisitor Instance => _Instance.Value;

        private Queue<ASTNode> _PendingWhiteSpace = new Queue<ASTNode>();

        public IEnumerable<ASTNode> Transform(TemplateNode item)
        {
            yield return new TemplateNode(TransformAll(item.Children));
        }

        public IEnumerable<ASTNode> Transform(EmptyNode emptyNode)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ASTNode> Transform(ForNode item)
        {
            var whiteSpace = WhiteSpaceIfNotTrimmed(item).ToArray();
            var primaryBlock = item.PrimaryBlock.Transform(this).OfType<ContainerNode>().First();
            var elseBlock = item.ElseBlock?.Transform(this).OfType<ContainerNode>().First();
            var forNode = new ForNode(primaryBlock, elseBlock, item.VariableNames, item.Expression, item.StartWhiteSpace, item.EndWhiteSpace);
            return whiteSpace.Concat(forNode);
        }

        public IEnumerable<ASTNode> Transform(ContainerNode item)
        {
            var whiteSpace = WhiteSpaceIfNotTrimmed(item).ToArray();
            var newNode = new ContainerNode(TransformAll(item.Children), item.StartWhiteSpace, item.EndWhiteSpace);
            return whiteSpace.Concat(newNode);
        }

        public IEnumerable<ASTNode> Transform(ExpressionNode item)
        {
            yield return item;
        }

        public IEnumerable<ASTNode> Transform(NewLineNode item)
        {
            _PendingWhiteSpace.Enqueue(item);
            yield break;
        }

        public IEnumerable<ASTNode> Transform(OutputNode item)
        {
            yield return item;
        }

        public IEnumerable<ASTNode> Transform(WhiteSpaceNode item)
        {
            _PendingWhiteSpace.Enqueue(item);
            yield break;
        }

        public IEnumerable<ASTNode> Transform(IfNode item)
        {
            var whiteSpace = WhiteSpaceIfNotTrimmed(item).ToArray();

            var newNode = new IfNode(
                item.Conditions.Select(child => child.Transform(this).OfType<ConditionalNode>().First()),
                    item.StartWhiteSpace, item.EndWhiteSpace);
            return whiteSpace.Concat(newNode).ToArray();
        }

        public IEnumerable<ASTNode> Transform(ConditionalNode item)
        {
            var whiteSpace = WhiteSpaceIfNotTrimmed(item).ToArray();
            var newNode = new ConditionalNode(item.Expression, TransformAll(item.Children),
                    item.StartWhiteSpace, item.EndWhiteSpace);
            return whiteSpace.Concat(newNode).ToArray();
        }

        public IEnumerable<ASTNode> Transform(CommentNode item)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ASTNode> Transform(BlockNode item)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ASTNode> Transform(ExtendsNode item)
        {
            throw new NotImplementedException();
        }


        private IEnumerable<ASTNode> WhiteSpaceIfNotTrimmed<T>(T item) where T : IWhiteSpaceControlling
        {
            if (item.StartWhiteSpace != WhiteSpaceControlMode.Trim)
            {
                foreach (var ws in _PendingWhiteSpace) yield return ws;
            }
            _PendingWhiteSpace.Clear();
        }


        private IEnumerable<ASTNode> TransformAll(IEnumerable<ASTNode> items)
        {
            foreach(var item in items)
            {
                foreach(var child in item.Transform(this))
                {
                    yield return child;
                }
            }
        }
    }
}
