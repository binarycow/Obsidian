using Obsidian.AST;
using Obsidian.AST.Nodes;
using Obsidian.AST.Nodes.MiscNodes;
using Obsidian.AST.Nodes.Statements;
using Obsidian.Transforming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obsidian.Transforming
{
    internal class NodeFinderVisitor : ITransformVisitor<IEnumerable<ASTNode>>
    {
        public NodeFinderVisitor(Func<ASTNode, bool> func)
        {
            _Predicate = func;
        }

        private Func<ASTNode, bool> _Predicate;

        internal static IEnumerable<T> FindNodes<T>(ASTNode rootNode) where T : ASTNode
        {
            return new NodeFinderVisitor(node => node is T).FindNodes(rootNode).OfType<T>();
        }

        public IEnumerable<ASTNode> FindNodes(ASTNode rootNode)
        {
            foreach(var node in rootNode.Transform(this))
            {
                if(_Predicate(node))
                {
                    yield return node;
                }
            }
        }
        private IEnumerable<ASTNode> TransformAll(IEnumerable<ASTNode?> items)
        {
            foreach(var item in items.SelectMany(item => item?.Transform(this)).Where(item => item != null))
            {
                yield return item;
            }
        }
        private IEnumerable<ASTNode> TransformAll(ASTNode item)
        {
            foreach(var child in item.Transform(this))
            {
                yield return child;
            }
        }

        public IEnumerable<ASTNode> Transform(TemplateNode item)
        {
            yield return item;
            foreach(var child in TransformAll(item.Children))
            {
                yield return child;
            }
        }

        public IEnumerable<ASTNode> Transform(EmptyNode item)
        {
            yield return item;
        }

        public IEnumerable<ASTNode> Transform(ForNode item)
        {
            yield return item;
            foreach (var child in TransformAll(new ASTNode?[] { item.PrimaryBlock, item.ElseBlock, item.Expression, item.Filter }))
            {
                yield return child;
            }
        }

        public IEnumerable<ASTNode> Transform(ContainerNode item)
        {
            yield return item;
            foreach (var child in TransformAll(item.Children))
            {
                yield return child;
            }
        }

        public IEnumerable<ASTNode> Transform(ExpressionNode item)
        {
            yield return item;
            if(item.ElseClause != null)
            {
                foreach (var child in TransformAll(item.ElseClause))
                {
                    yield return child;
                }
            }
        }

        public IEnumerable<ASTNode> Transform(NewLineNode item)
        {
            yield return item;
        }

        public IEnumerable<ASTNode> Transform(OutputNode item)
        {
            yield return item;
        }

        public IEnumerable<ASTNode> Transform(WhiteSpaceNode item)
        {
            yield return item;
        }

        public IEnumerable<ASTNode> Transform(IfNode item)
        {
            yield return item;
            foreach (var child in TransformAll(item.Conditions))
            {
                yield return child;
            }
        }

        public IEnumerable<ASTNode> Transform(ConditionalNode item)
        {
            yield return item;
            foreach (var child in TransformAll(item.Expression))
            {
                yield return child;
            }
            foreach (var child in TransformAll(item.Children))
            {
                yield return child;
            }
        }

        public IEnumerable<ASTNode> Transform(CommentNode item)
        {
            yield return item;
        }

        public IEnumerable<ASTNode> Transform(BlockNode item)
        {
            yield return item;
            foreach (var child in TransformAll(item.BlockContents))
            {
                yield return child;
            }
        }

        public IEnumerable<ASTNode> Transform(ExtendsNode item)
        {
            yield return item;
            foreach (var child in TransformAll(item.Template))
            {
                yield return child;
            }
        }

        public IEnumerable<ASTNode> Transform(RawNode item)
        {
            yield return item;
        }

        public IEnumerable<ASTNode> Transform(MacroNode item)
        {
            yield return item;
            foreach (var child in TransformAll(item.Contents))
            {
                yield return child;
            }
        }

        public IEnumerable<ASTNode> Transform(CallNode item)
        {
            yield return item;
            foreach (var child in TransformAll(item.Contents))
            {
                yield return child;
            }
        }

        public IEnumerable<ASTNode> Transform(FilterNode item)
        {
            yield return item;
            foreach (var child in TransformAll(item.FilterContents))
            {
                yield return child;
            }
        }

        public IEnumerable<ASTNode> Transform(SetNode item)
        {
            yield return item;
            foreach (var child in TransformAll(item.Children))
            {
                yield return child;
            }
        }

        public IEnumerable<ASTNode> Transform(IncludeNode item)
        {
            yield return item;
        }

        public IEnumerable<ASTNode> Transform(ImportNode item)
        {
            yield return item;
        }

        public IEnumerable<ASTNode> Transform(FromNode item)
        {
            throw new NotImplementedException();
        }
    }
}
