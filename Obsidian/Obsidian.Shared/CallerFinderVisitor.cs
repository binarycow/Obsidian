using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExpressionParser.Parsing;
using ExpressionParser.References;
using ExpressionParser.Transforming.Nodes;
using Obsidian.AST.Nodes;
using Obsidian.AST.Nodes.MiscNodes;
using Obsidian.AST.Nodes.Statements;
using Obsidian.Transforming;

namespace Obsidian
{
    internal class CallerFinderVisitor : INodeTransformVisitor<IEnumerable<bool>>, ITransformVisitor<IEnumerable<bool>>
    {
        private CallerFinderVisitor()
        {

        }
        private readonly static Lazy<CallerFinderVisitor> _Instance = new Lazy<CallerFinderVisitor>(() => new CallerFinderVisitor());
        public static CallerFinderVisitor Instance => _Instance.Value;


        private IEnumerable<bool> TransformAll(IEnumerable<ASTNode> nodes)
        {
            return nodes.SelectMany(node => node.Transform(this));
        }
        private IEnumerable<bool> TransformAll(IEnumerable<Obsidian.AST.ASTNode> nodes)
        {
            return nodes.SelectMany(node => node.Transform(this));
        }

        public IEnumerable<bool> Transform(BinaryASTNode item)
        {
            return TransformAll(item.Left.YieldOne().Concat(item.Right));
        }

        public IEnumerable<bool> Transform(UnaryASTNode item)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<bool> Transform(LiteralNode item)
        {
            yield return false;
        }

        public IEnumerable<bool> Transform(IdentifierNode item)
        {
            if (item.TextValue == "caller")
            {
                yield return true;
                yield break;
            }
            yield return false;
        }

        public IEnumerable<bool> Transform(DictionaryItemNode item)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<bool> Transform(DictionaryNode item)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<bool> Transform(TupleNode item)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<bool> Transform(ListNode item)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<bool> Transform(PipelineMethodGroup item)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<bool> Transform(TemplateNode item)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<bool> Transform(EmptyNode emptyNode)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<bool> Transform(ForNode item)
        {
            return item.Expression.ExpressionParserNode.Transform(this)
                .Concat(item.PrimaryBlock.Transform(this))
                .Concat(item.ElseBlock?.Transform(this) ?? Enumerable.Empty<bool>());
        }

        public IEnumerable<bool> Transform(ContainerNode item) => TransformAll(item.Children);

        public IEnumerable<bool> Transform(ExpressionNode item)
        {
            return item.ExpressionParserNode.Transform(this);
        }

        public IEnumerable<bool> Transform(NewLineNode item)
        {
            yield return false;
        }

        public IEnumerable<bool> Transform(OutputNode item)
        {
            yield return false;
        }

        public IEnumerable<bool> Transform(WhiteSpaceNode item)
        {
            yield return false;
        }

        public IEnumerable<bool> Transform(IfNode item)
        {
            return item.Conditions.SelectMany(condition => condition.Transform(this));
        }

        public IEnumerable<bool> Transform(ConditionalNode item)
        {
            return item.Expression.Transform(this).Concat(item.Children.SelectMany(child => child.Transform(this)));
        }

        public IEnumerable<bool> Transform(CommentNode item)
        {
            yield return false;
        }

        public IEnumerable<bool> Transform(BlockNode item)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<bool> Transform(ExtendsNode item)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<bool> Transform(RawNode item)
        {
            yield return false;
        }

        public IEnumerable<bool> Transform(MacroNode item)
        {
            return item.Contents.Transform(this);
        }

        public IEnumerable<bool> Transform(CallNode item)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<bool> Transform(FilterNode item)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<bool> Transform(SetNode item)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<bool> Transform(IncludeNode item)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<bool> Transform(ArgumentSetNode item)
        {
            yield return false;
        }
    }
}
