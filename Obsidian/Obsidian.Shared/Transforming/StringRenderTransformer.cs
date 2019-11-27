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
    public class StringRenderTransformer : ITransformVisitor<string>
    {
        public StringRenderTransformer(JinjaEnvironment environment, IDictionary<string, object?> variables)
        {
            Scopes = new ScopeStack<DynamicContext, DynamicRootContext>(DynamicRootContext.CreateNew("root", this, variables));
            Environment = environment;
        }

        public JinjaEnvironment Environment { get; }
        private readonly StringBuilder _StringBuilder = new StringBuilder();

        private ScopeStack<DynamicContext, DynamicRootContext> Scopes { get; }

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

                var nextTemplateObj = Environment.Evaluation.EvaluateDynamic(_NextTemplate.Expression, Scopes);
                _NextTemplate = null;
                switch(nextTemplateObj)
                {
                    case DynamicTemplate d:
                        toRender = d.TemplateNode;
                        break;
                    case string templateName:
                        var temp = Environment.GetTemplate(templateName, Scopes.Current);
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
            if (item.VariableNames.Length != 1) throw new NotImplementedException();
            var evalObj = Environment.Evaluation.EvaluateDynamic(item.Expression.Expression, Scopes);
            var arr = CollectionEx.ToArray(evalObj);
            if (arr == null) throw new NotImplementedException();

            if(arr.Length == 0 && item.ElseBlock != null)
            {
                return item.ElseBlock.Transform(this);
            }

            for(var index = 0; index < arr.Length; ++index)
            {
                var arrItem = arr[index];
                Scopes.Push($"ForNode: {item.Expression} Item: {arrItem}");
                Scopes.Current.DefineAndSetVariable(item.VariableNames[0], arrItem);
                item.PrimaryBlock.Transform(this);
                Scopes.Pop($"ForNode: {item.Expression} Item: {arrItem}");
            }
            return string.Empty;
        }

        public string Transform(ContainerNode item)
        {
            return TransformAll(item.Children);
        }

        public string Transform(ExpressionNode item)
        {
            _StringBuilder.Append(Environment.Evaluation.EvaluateDynamic(item.Expression, Scopes));
            return string.Empty;
        }

        public string Transform(NewLineNode item)
        {
            _StringBuilder.Append(item.ToString());
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
            foreach(var condition in item.Conditions)
            {
                var result = Environment.Evaluation.EvaluateDynamic(condition.Expression.Expression, Scopes);
                if (result == null) throw new NotImplementedException();
                if (TypeCoercion.CanCast(result.GetType(), typeof(bool)) == false) throw new NotImplementedException();
                var boolResult = (bool)result;

                if(boolResult)
                {
                    condition.Transform(this);
                    return string.Empty;
                }
            }
            return string.Empty;
        }

        public string Transform(ConditionalNode item)
        {
            return TransformAll(item.Children);
        }

        public string Transform(CommentNode item)
        {
            return string.Empty;
        }

        public string Transform(BlockNode item)
        {
            Scopes.Root.AddBlock(item.Name, item.BlockContents);
            if (ShouldRender)
            {
                Scopes.Root.CurrentBlockName = item.Name;
                Scopes.Root.CurrentBlockIndex = 0;
                var containerNode = Scopes.Root.GetBlock(item.Name) ?? item.BlockContents;
                containerNode.Transform(this);
                Scopes.Root.CurrentBlockName = null;
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
