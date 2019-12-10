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
    internal class StringBuilderTransformer : ITransformVisitor
    {
        internal StringBuilderTransformer(JinjaEnvironment environment, IDictionary<string, object?> variables)
        {
            Scopes = new ScopeStack<DynamicContext, DynamicRootContext>(DynamicRootContext.CreateNew("root", this, variables));
            Environment = environment;
            StringRenderTransformer = new StringRenderTransformer(environment, Scopes);
        }

        internal StringBuilder StringBuilder { get; } = new StringBuilder();
        private ScopeStack<DynamicContext, DynamicRootContext> Scopes { get; }
        internal JinjaEnvironment Environment { get; }
        internal StringRenderTransformer StringRenderTransformer { get; }



        private void TransformASTNode(ASTNode astNode)
        {
            foreach (var output in astNode.Transform(StringRenderTransformer))
            {
                StringBuilder.Append(output);
            }
        }


        public void Transform(TemplateNode item)
        {
            TransformASTNode(item);
        }

        public void Transform(ForNode item)
        {
            TransformASTNode(item);
        }

        public void Transform(ContainerNode item)
        {
            TransformASTNode(item);
        }

        public void Transform(RawNode item)
        {
            TransformASTNode(item);
        }



        public void Transform(CallNode item)
        {
            TransformASTNode(item);
        }


        public void Transform(MacroNode item)
        {
            TransformASTNode(item);
        }
        public void Transform(ExpressionNode item)
        {
            TransformASTNode(item);
        }

        public void Transform(SetNode item)
        {
            TransformASTNode(item);
        }



        public void Transform(NewLineNode item)
        {
            TransformASTNode(item);
        }

        public void Transform(OutputNode item)
        {
            TransformASTNode(item);
        }

        public void Transform(WhiteSpaceNode item)
        {
            TransformASTNode(item);
        }

        public void Transform(IfNode item)
        {
            TransformASTNode(item);
        }

        public void Transform(ConditionalNode item)
        {
            TransformASTNode(item);
        }

        public void Transform(CommentNode item)
        {
            TransformASTNode(item);
        }


        public void Transform(FilterNode item)
        {
            TransformASTNode(item);
        }
        public void Transform(BlockNode item)
        {
            TransformASTNode(item);
        }

        public void Transform(ExtendsNode item)
        {
            TransformASTNode(item);
        }


        public void Transform(EmptyNode item)
        {
            TransformASTNode(item);
        }

        public void Transform(IncludeNode item)
        {
            throw new NotImplementedException();
        }
    }
}
