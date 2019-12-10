using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using ExpressionParser;
using Obsidian.AST;
using Obsidian.AST.Nodes;
using Obsidian.AST.Nodes.MiscNodes;
using Obsidian.AST.Nodes.Statements;
using Obsidian.Templates;

namespace Obsidian.Transforming
{
    internal class StringRenderTransformer : ITransformVisitor<IEnumerable<string>>
    {
        internal StringRenderTransformer(JinjaEnvironment environment, ScopeStack<DynamicContext, DynamicRootContext> scopes)
        {
            Scopes = scopes;
            Environment = environment;
        }

        internal JinjaEnvironment Environment { get; }
        private ScopeStack<DynamicContext, DynamicRootContext> Scopes { get; }

        private ExpressionNode? _NextTemplate = null;
        private bool _EncounteredOutputStyleBlock;
        internal bool ShouldRender => _NextTemplate == null;


        public IEnumerable<string> Transform(TemplateNode item)
        {
            var toRender = item;
            while (true)
            {
                foreach (var output in TransformAll(toRender.Children))
                {
                    yield return output;
                }


                if (_NextTemplate == null) break;

                var nextTemplateObj = Environment.Evaluation.EvaluateDynamic(_NextTemplate.Expression, Scopes);
                _NextTemplate = null;
                switch (nextTemplateObj)
                {
                    case DynamicTemplate d:
                        toRender = d.TemplateNode;
                        break;
                    case string templateName:
                        var temp = Environment.GetDynamicTemplate(templateName);
                        if (!(temp is DynamicTemplate dt)) throw new NotImplementedException();
                        toRender = dt.TemplateNode;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public IEnumerable<string> Transform(EmptyNode emptyNode)
        {
            yield break;
        }

        public IEnumerable<string> Transform(ForNode item)
        {
            _EncounteredOutputStyleBlock = true;
            if (!(ShouldRender && _EncounteredOutputStyleBlock)) yield break;
            if (item.VariableNames.Length != 1) throw new NotImplementedException();
            var evalObj = Environment.Evaluation.EvaluateDynamic(item.Expression.Expression, Scopes);
            var arr = CollectionEx.ToArray(evalObj) ?? Array.Empty<object>();

            if (arr.Length == 0 && item.ElseBlock != null)
            {
                foreach (var output in item.ElseBlock.Transform(this)) yield return output;
                yield break;
            }

            for (var index = 0; index < arr.Length; ++index)
            {
                var arrItem = arr[index];
                var loopInfo = new LoopInfoClass<object>(arr, index);
                Scopes.Push($"ForNode: {item.Expression} Item: {arrItem}");
                Scopes.Current.DefineAndSetVariable(item.VariableNames[0], arrItem);
                Scopes.Current.DefineAndSetVariable("loop", loopInfo);

                foreach (var output in item.PrimaryBlock.Transform(this)) yield return output;
                Scopes.Pop($"ForNode: {item.Expression} Item: {arrItem}");
            }
        }

        public IEnumerable<string> Transform(ContainerNode item)
        {
            _EncounteredOutputStyleBlock = true;
            if (!(ShouldRender && _EncounteredOutputStyleBlock)) yield break;


            foreach (var output in TransformAll(item.Children))
            {
                yield return output;
            }
        }

        public IEnumerable<string> Transform(ExpressionNode item)
        {
            _EncounteredOutputStyleBlock = true;
            if (!(ShouldRender && _EncounteredOutputStyleBlock)) yield break;

            var result = Environment.Evaluation.EvaluateDynamic(item.Expression, Scopes);

            switch(result)
            {
                case ExpressionParser.Void _:
                    yield break;
                case ASTNode astNode:
                    foreach(var output in astNode.Transform(this))
                    {
                        yield return output;
                    }
                    yield break;
                default:
                    yield return JinjaCustomStringProvider.Instance.ToString(result);
                    yield break;
            }
        }

        public IEnumerable<string> Transform(NewLineNode item)
        {
            if (!(ShouldRender && _EncounteredOutputStyleBlock)) yield break;
            _EncounteredOutputStyleBlock = true;

            if (item.WhiteSpaceMode != WhiteSpaceControl.WhiteSpaceMode.Trim)
            {
                yield return JinjaCustomStringProvider.Instance.ToString(item.ToString());
            }
        }

        public IEnumerable<string> Transform(OutputNode item)
        {
            _EncounteredOutputStyleBlock = true;
            if (!(ShouldRender && _EncounteredOutputStyleBlock)) yield break;
            yield return JinjaCustomStringProvider.Instance.ToString(item.Value);
        }

        public IEnumerable<string> Transform(WhiteSpaceNode item)
        {
            _EncounteredOutputStyleBlock = true;
            if (!(ShouldRender && _EncounteredOutputStyleBlock)) yield break;

            if (item.WhiteSpaceMode != WhiteSpaceControl.WhiteSpaceMode.Trim)
            {
                yield return JinjaCustomStringProvider.Instance.ToString(item.ToString());
            }
        }

        public IEnumerable<string> Transform(IfNode item)
        {
            if (ShouldRender == false) yield break;
            foreach (var condition in item.Conditions)
            {
                var result = Environment.Evaluation.EvaluateDynamic(condition.Expression.Expression, Scopes);
                if (result == null) throw new NotImplementedException();
                if (TypeCoercion.CanCast(result.GetType(), typeof(bool)) == false) throw new NotImplementedException();
                var boolResult = (bool)result;

                if (boolResult)
                {
                    _EncounteredOutputStyleBlock = true;
                    
                    foreach(var output in condition.Transform(this))
                    {
                        yield return output;
                    }
                    yield break;
                }
            }

        }

        public IEnumerable<string> Transform(ConditionalNode item)
        {
            _EncounteredOutputStyleBlock = true;
            if (ShouldRender == false) yield break;


            foreach (var output in TransformAll(item.Children))
            {
                yield return output;
            }
        }

        public IEnumerable<string> Transform(CommentNode item)
        {
            yield break;
        }

        public IEnumerable<string> Transform(BlockNode item)
        {
            Scopes.Root.AddBlock(item.Name, item.BlockContents);
            if (ShouldRender)
            {
                Scopes.Root.CurrentBlockName = item.Name;
                Scopes.Root.CurrentBlockIndex = 0;
                var containerNode = Scopes.Root.GetBlock(item.Name) ?? item.BlockContents;
                foreach(var output in containerNode.Transform(this))
                {
                    yield return output;
                }
                Scopes.Root.CurrentBlockName = null;
            }
        }

        public IEnumerable<string> Transform(ExtendsNode item)
        {
            if (ShouldRender == false) throw new NotImplementedException();
            _NextTemplate = item.Template;
            yield break;
        }

        public IEnumerable<string> Transform(RawNode item)
        {
            _EncounteredOutputStyleBlock = true;
            if (!(ShouldRender && _EncounteredOutputStyleBlock)) yield break;
            foreach (var node in item.Children)
            {
                yield return JinjaCustomStringProvider.Instance.ToString(node.ToString());

            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0039:Use local function", Justification = "<Pending>")]
        public IEnumerable<string> Transform(MacroNode item)
        {
            _EncounteredOutputStyleBlock = true;
            if (!(ShouldRender && _EncounteredOutputStyleBlock)) yield break;
            if (Environment.Evaluation.TryParseFunctionDeclaration(item.MacroText, out var functionDeclaration) == false || functionDeclaration == null)
            {
                throw new NotImplementedException();
            }

            var usesCaller = item.Contents.Transform(CallerFinderVisitor.Instance).Any(x => x);


            Func<UserDefinedArgumentData, object?> func = arguments =>
            {
                using var checkout = StringBuilderPool.Instance.Checkout();
                var stringBuilder = checkout.CheckedOutObject;

                Scopes.Push($"Macro: {functionDeclaration.Name}");
                foreach (var arg in arguments.AllArguments)
                {
                    Scopes.Current.DefineAndSetVariable(arg.Name, arg.Value);
                }
                Scopes.Current.DefineAndSetVariable("varargs", new ArgumentNameCollection(Enumerable.Empty<string>()));
                Scopes.Current.DefineAndSetVariable("kwargs", new Dictionary<string, object?>());
                
                foreach(var output in item.Contents.Transform(this))
                {
                    stringBuilder.Append(output);
                }

                Scopes.Pop($"Macro: {functionDeclaration.Name}");
                return stringBuilder.ToString();
            };
            UserDefinedFunction.UserDefinedFunctionDelegate del = args => func(args);


            Scopes.Current.DefineAndSetVariable(functionDeclaration.Name, new JinjaUserDefinedFunction(functionDeclaration, del, usesCaller));

            yield break;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0039:Use local function", Justification = "<Pending>")]
        public IEnumerable<string> Transform(CallNode item)
        {
            _EncounteredOutputStyleBlock = true;
            if (!(ShouldRender && _EncounteredOutputStyleBlock)) yield break;

            Func<UserDefinedArgumentData, object?> func = arguments =>
            {
                using var checkout = StringBuilderPool.Instance.Checkout();
                var stringBuilder = checkout.CheckedOutObject;

                foreach(var output in item.Contents.Transform(this))
                {
                    stringBuilder.Append(output);
                }
                return stringBuilder.ToString();
            };



            if (Environment.Evaluation.TryParseFunctionDeclaration(item.CallerDefinition.Expression, out var functionDeclaration) == false || functionDeclaration == null)
            {
                throw new NotImplementedException();
            }

            Scopes.Push($"Call: {item.CallerDefinition.Expression}");
            UserDefinedFunction.UserDefinedFunctionDelegate del = args => func(args);
            Scopes.Current.DefineAndSetVariable("caller", new JinjaUserDefinedFunction(functionDeclaration, del));
            var evalObj = Environment.Evaluation.EvaluateDynamic(item.MacroCall.Expression, Scopes);
            if (!(evalObj is ExpressionParser.Void))
            {
                yield return JinjaCustomStringProvider.Instance.ToString(evalObj);
            }
            Scopes.Pop($"Call: {item.CallerDefinition.Expression}");

            if (!string.IsNullOrEmpty(item.CallerDefinition.Expression))
            {
            }
        }

        public IEnumerable<string> Transform(FilterNode item)
        {
            _EncounteredOutputStyleBlock = true;
            if (ShouldRender == false) yield break;

            var filterExpression = $"{item.Filter}(__item)";

            foreach(var output in item.FilterContents.Transform(this))
            {
                Scopes.Push("Filter");
                Scopes.Current.DefineAndSetVariable("__item", output);

                var evaluated = Environment.Evaluation.EvaluateDynamic(filterExpression, Scopes);


                Scopes.Pop("Filter");

                yield return evaluated?.ToString() ?? string.Empty;
            }
        }

        public IEnumerable<string> Transform(SetNode item)
        {
            _EncounteredOutputStyleBlock = true;
            if (!(ShouldRender && _EncounteredOutputStyleBlock)) yield break;

            if (item.VariableNames.Length != 1) throw new NotImplementedException();

            if (item.AssignmentExpression != null)
            {
                var objToAssign = Environment.Evaluation.EvaluateDynamic(item.AssignmentExpression, Scopes);
                Scopes.Current.DefineAndSetVariable(item.VariableNames[0], objToAssign);
                yield break;
            }
            if (item.AssignmentBlock != null)
            {
                Scopes.Current.DefineAndSetVariable(item.VariableNames[0], item.AssignmentBlock);
                yield break;
            }
            throw new NotImplementedException();
        }


        internal IEnumerable<string> TransformAll(IEnumerable<ASTNode> items)
        {
            foreach (var item in items)
            {
                foreach(var output in item.Transform(this))
                {
                    yield return output;
                }
            }
        }
    }
}
