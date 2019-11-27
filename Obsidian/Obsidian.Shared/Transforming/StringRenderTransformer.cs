using System;
using System.Collections.Generic;
using System.Text;
using ExpressionParser.Scopes;
using Obsidian.AST.Nodes;
using Obsidian.AST.Nodes.MiscNodes;
using Obsidian.AST.Nodes.Statements;
using Obsidian.AST;
using Obsidian.Templates;

namespace Obsidian.Transforming
{
    public class StringRenderTransformer : ITransformVisitor<string>
    {
        public StringRenderTransformer(JinjaEnvironment environment, IDictionary<string, object?> variables)
        {
            _Scopes.Push(DynamicScope.CreateRootScope("root", variables));
            Environment = environment;
        }

        public JinjaEnvironment Environment { get; }
        private readonly StringBuilder _StringBuilder = new StringBuilder();

        private readonly Stack<IDynamicScope> _Scopes = new Stack<IDynamicScope>();
        public IDynamicScope CurrentScope => _Scopes.Peek();

        private ExpressionNode? _NextTemplate = null;
        public bool ShouldRender => _NextTemplate == null;
        private DynamicSelf _Self = new DynamicSelf();


        public string Transform(TemplateNode item)
        {
            var toRender = item;
            while(true)
            {
                TransformAll(toRender.Children);
                if (_NextTemplate == null) break;

                var nextTemplateObj = Environment.Evaluation.EvaluateDynamic(_NextTemplate.Expression, CurrentScope);
                _NextTemplate = null;
                switch(nextTemplateObj)
                {
                    case DynamicTemplate d:
                        toRender = d.TemplateNode;
                        break;
                    case string templateName:
                        var temp = Environment.GetTemplate(templateName, CurrentScope);
                        if (!(temp is DynamicTemplate dt)) throw new NotImplementedException();
                        toRender = dt.TemplateNode;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            return _StringBuilder.ToString();
        }

        public string Transform(ForNode item)
        {
            throw new NotImplementedException();
        }

        public string Transform(ContainerNode item)
        {
            return TransformAll(item.Children);
        }

        public string Transform(ExpressionNode item)
        {
            _StringBuilder.Append(Environment.Evaluation.EvaluateDynamic(item.Expression, CurrentScope));
            return string.Empty;
        }

        public string Transform(NewLineNode item)
        {
            //_StringBuilder.Append(item.ToString());
            return string.Empty;
        }

        public string Transform(OutputNode item)
        {
            _StringBuilder.Append(item.Value);
            return string.Empty;
        }

        public string Transform(WhiteSpaceNode item)
        {
            _StringBuilder.Append(item.ToString());
            return string.Empty;
        }

        public string Transform(IfNode item)
        {
            throw new NotImplementedException();
        }

        public string Transform(ConditionalNode item)
        {
            throw new NotImplementedException();
        }

        public string Transform(CommentNode item)
        {
            throw new NotImplementedException();
        }

        public string Transform(BlockNode item)
        {
            _Self.AddBlock(item);
            if (ShouldRender)
            {
                var blockNode = _Self.GetBlock(item.Name) ?? item;
                TransformAll(blockNode.Children);
            }
            return string.Empty;
        }

        public string Transform(ExtendsNode item)
        {
            _NextTemplate = item.Template;
            return string.Empty;
        }

        public string TransformAll(IEnumerable<ASTNode> items)
        {
            foreach (var item in items)
                item.Transform(this);
            return string.Empty;
        }
    }
}
