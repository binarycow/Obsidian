using System;
using System.Collections.Generic;
using System.Text;
using ExpressionParser.Scopes;
using Obsidian.AST.Nodes;
using Obsidian.AST.Nodes.MiscNodes;
using Obsidian.AST.Nodes.Statements;
using Obsidian.AST;
using Obsidian.Templates;
using ExpressionParser;
using Common;

namespace Obsidian.Transforming
{
    public class StringBuilderTransformer : ITransformVisitor
    {
        public StringBuilderTransformer(JinjaEnvironment environment, IDictionary<string, object?> variables)
        {
            Scopes = new ScopeStack<DynamicContext, DynamicRootContext>(DynamicRootContext.CreateNew("root", this, variables));
            Environment = environment;
            StringRenderTransformer = new StringRenderTransformer(environment, Scopes);
        }

        public StringBuilder StringBuilder { get; } = new StringBuilder();
        private ScopeStack<DynamicContext, DynamicRootContext> Scopes { get; }
        public JinjaEnvironment Environment { get; }
        public StringRenderTransformer StringRenderTransformer { get; }






        public void Transform(TemplateNode item)
        {
            foreach(var output in item.Transform(StringRenderTransformer))
            {
                StringBuilder.Append(output);
            }
        }

        public void Transform(ForNode item)
        {
            throw new NotImplementedException();
        }

        public void Transform(ContainerNode item)
        {
            throw new NotImplementedException();
        }

        public void Transform(RawNode item)
        {
            throw new NotImplementedException();
        }



        public void Transform(CallNode item)
        {
            throw new NotImplementedException();
        }


        public void Transform(MacroNode item)
        {
            throw new NotImplementedException();
        }
        public void Transform(ExpressionNode item)
        {
            throw new NotImplementedException();
        }

        public void Transform(SetNode item)
        {
            throw new NotImplementedException();
        }



        public void Transform(NewLineNode item)
        {
            throw new NotImplementedException();
        }

        public void Transform(OutputNode item)
        {
            throw new NotImplementedException();
        }

        public void Transform(WhiteSpaceNode item)
        {
            throw new NotImplementedException();
        }

        public void Transform(IfNode item)
        {
            throw new NotImplementedException();
        }

        public void Transform(ConditionalNode item)
        {
            throw new NotImplementedException();
        }

        public void Transform(CommentNode item)
        {
            throw new NotImplementedException();
        }


        public void Transform(FilterNode item)
        {
            throw new NotImplementedException();
        }
        public void Transform(BlockNode item)
        {
            throw new NotImplementedException();
        }

        public void Transform(ExtendsNode item)
        {
            throw new NotImplementedException();
        }


        public void Transform(EmptyNode emptyNode)
        {
            throw new NotImplementedException();
        }


        public void TransformAll(IEnumerable<ASTNode> items)
        {
            foreach (var item in items)
            {
                item.Transform(this);
            }
        }
    }
}
