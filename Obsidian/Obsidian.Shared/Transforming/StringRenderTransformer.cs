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
    public class StringRenderTransformer : ITransformVisitor
    {
        public StringRenderTransformer(JinjaEnvironment environment, IDictionary<string, object?> variables)
        {
            Scopes = new ScopeStack<DynamicContext, DynamicRootContext>(DynamicRootContext.CreateNew("root", this, variables));
            Environment = environment;
        }

        public JinjaEnvironment Environment { get; }
        public StringBuilder StringBuilder { get; } = new StringBuilder();

        private ScopeStack<DynamicContext, DynamicRootContext> Scopes { get; }

        private ExpressionNode? _NextTemplate = null;
        private bool _EncounteredOutputStyleBlock { get; set; }
        public bool ShouldRender => _NextTemplate == null;
        private DynamicSelf _Self = new DynamicSelf();


        public void Transform(TemplateNode item)
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
        }

        public void Transform(ForNode item)
        {
            _EncounteredOutputStyleBlock = true;
            if (!(ShouldRender && _EncounteredOutputStyleBlock)) return;
            if (item.VariableNames.Length != 1) throw new NotImplementedException();
            var evalObj = Environment.Evaluation.EvaluateDynamic(item.Expression.Expression, Scopes);
            var arr = CollectionEx.ToArray(evalObj);
            arr = arr ?? Array.Empty<object>();

            if(arr.Length == 0 && item.ElseBlock != null)
            {
                item.ElseBlock.Transform(this);
                return;
            }

            for(var index = 0; index < arr.Length; ++index)
            {
                var arrItem = arr[index];
                var loopInfo = new LoopInfoClass<object>(arr, index);
                Scopes.Push($"ForNode: {item.Expression} Item: {arrItem}");
                Scopes.Current.DefineAndSetVariable(item.VariableNames[0], arrItem);
                Scopes.Current.DefineAndSetVariable("loop", loopInfo);

                item.PrimaryBlock.Transform(this);
                Scopes.Pop($"ForNode: {item.Expression} Item: {arrItem}");
            }
        }

        public void Transform(ContainerNode item)
        {
            _EncounteredOutputStyleBlock = true;
            if (!(ShouldRender && _EncounteredOutputStyleBlock)) return;
            TransformAll(item.Children);
        }

        public void Transform(ExpressionNode item)
        {
            _EncounteredOutputStyleBlock = true;
            if (!(ShouldRender && _EncounteredOutputStyleBlock)) return;

            var result = Environment.Evaluation.EvaluateDynamic(item.Expression, Scopes);
            if (result is ExpressionParser.Void) return;

            StringBuilder.Append(result);
            return;
        }

        public void Transform(NewLineNode item)
        {
            if (!(ShouldRender && _EncounteredOutputStyleBlock)) return;
            _EncounteredOutputStyleBlock = true;

            if(item.WhiteSpaceMode != WhiteSpaceControl.WhiteSpaceMode.Trim)
            {
                StringBuilder.Append(item.ToString());
            }

            return;
        }

        public void Transform(OutputNode item)
        {
            _EncounteredOutputStyleBlock = true;
            if (!(ShouldRender && _EncounteredOutputStyleBlock)) return;
            StringBuilder.Append(item.Value);
            _EncounteredOutputStyleBlock = true; // TODO: Is this right?
            return;
        }

        public void Transform(WhiteSpaceNode item)
        {
            _EncounteredOutputStyleBlock = true;
            if (!(ShouldRender && _EncounteredOutputStyleBlock)) return;

            if (item.WhiteSpaceMode != WhiteSpaceControl.WhiteSpaceMode.Trim)
            {
                StringBuilder.Append(item.ToString());
            }
            return;
        }

        public void Transform(IfNode item)
        {
            if (ShouldRender == false) return;
            foreach (var condition in item.Conditions)
            {
                var result = Environment.Evaluation.EvaluateDynamic(condition.Expression.Expression, Scopes);
                if (result == null) throw new NotImplementedException();
                if (TypeCoercion.CanCast(result.GetType(), typeof(bool)) == false) throw new NotImplementedException();
                var boolResult = (bool)result;

                if(boolResult)
                {
                    _EncounteredOutputStyleBlock = true;
                    condition.Transform(this);
                    return;
                }
            }
        }

        public void Transform(ConditionalNode item)
        {
            _EncounteredOutputStyleBlock = true;
            if (ShouldRender == false) return;
            TransformAll(item.Children);
        }

        public void Transform(CommentNode item)
        {
            return;
        }

        public void Transform(BlockNode item)
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
        }

        public void Transform(ExtendsNode item)
        {
            if (ShouldRender == false) throw new NotImplementedException();
            _NextTemplate = item.Template;
        }

        public void TransformAll(IEnumerable<ASTNode> items)
        {
            foreach (var item in items)
                item.Transform(this);
        }

        public void Transform(EmptyNode emptyNode)
        {
            return;
        }

        public void Transform(RawNode item)
        {
            foreach(var node in item.Children)
            {
                StringBuilder.Append(node.ToString());
            }
        }
        public void Transform(MacroNode item)
        {
            if (Environment.Evaluation.TryParseFunctionDeclaration(item.MacroText, out var functionDeclaration) == false || functionDeclaration == null)
            {
                throw new NotImplementedException();
            }

            Func<UserDefinedArgumentData, object?> func = arguments =>
            {
                Scopes.Push($"Macro: {functionDeclaration.Name}");
                foreach (var arg in arguments.PositionalArguments)
                {
                    Scopes.Current.DefineAndSetVariable(arg.name, arg.value);
                }
                foreach (var arg in arguments.KeywordArguments)
                {
                    Scopes.Current.DefineAndSetVariable(arg.name, arg.value);
                }
                item.Contents.Transform(this);



                Scopes.Pop($"Macro: {functionDeclaration.Name}");
                return ExpressionParser.Void.Instance;
            };
            Scopes.Current.DefineAndSetVariable(functionDeclaration.Name, new UserDefinedFunction(functionDeclaration, func));
        }
    }
}
