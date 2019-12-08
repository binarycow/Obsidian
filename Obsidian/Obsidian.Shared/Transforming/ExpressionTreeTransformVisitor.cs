using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Obsidian.AST.Nodes;
using Obsidian.AST.Nodes.MiscNodes;
using Obsidian.AST.Nodes.Statements;

namespace Obsidian.Transforming
{
    internal class ExpressionTreeTransformVisitor : ITransformVisitor<Expression>
    {
        internal ExpressionTreeTransformVisitor(JinjaEnvironment environment, IDictionary<string, object?> variables)
        {
            Environment = environment;
            Variables = variables;
        }

        internal JinjaEnvironment Environment { get; set; }
        internal IDictionary<string, object?> Variables { get; }

        public Expression Transform(ForNode item)
        {
            throw new NotImplementedException();
        }

        public Expression Transform(ContainerNode item)
        {
            throw new NotImplementedException();
        }

        public Expression Transform(ExpressionNode item)
        {
            throw new NotImplementedException();
        }

        public Expression Transform(NewLineNode item)
        {
            throw new NotImplementedException();
        }

        public Expression Transform(OutputNode item)
        {
            throw new NotImplementedException();
        }

        public Expression Transform(WhiteSpaceNode item)
        {
            throw new NotImplementedException();
        }

        public Expression Transform(IfNode item)
        {
            throw new NotImplementedException();
        }

        public Expression Transform(ConditionalNode item)
        {
            throw new NotImplementedException();
        }

        public Expression Transform(CommentNode item)
        {
            throw new NotImplementedException();
        }

        public Expression Transform(BlockNode item)
        {
            throw new NotImplementedException();
        }
        public Expression Transform(ExtendsNode item)
        {
            throw new NotImplementedException();
        }

        public Expression Transform(TemplateNode item)
        {
            throw new NotImplementedException();
        }

        public Expression Transform(EmptyNode emptyNode)
        {
            throw new NotImplementedException();
        }

        public Expression Transform(RawNode item)
        {
            throw new NotImplementedException();
        }
        public Expression Transform(MacroNode item)
        {
            throw new NotImplementedException();
        }

        public Expression Transform(CallNode item)
        {
            throw new NotImplementedException();
        }

        public Expression Transform(FilterNode item)
        {
            throw new NotImplementedException();
        }

        public Expression Transform(SetNode item)
        {
            throw new NotImplementedException();
        }
    }
}
