using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Collections;
using ExpressionParser;
using ExpressionParser.Scopes;
using Obsidian.AST.Nodes;
using Obsidian.AST.Nodes.MiscNodes;
using Obsidian.AST.Nodes.Statements;
using Obsidian.Rendering.RenderObjects;
using Obsidian.TemporaryStuff;
using Obsidian.WhiteSpaceControl;

namespace Obsidian.Transforming
{
    public class ReflectionVisitor : ITransformVisitor<IEnumerable<RenderObj>>
    {
        public ExpressionEval Evaluation { get; }
        public JinjaEnvironment Environment { get; }
        public ReflectionVisitor(JinjaEnvironment environment, IDictionary<string, object?> variables)
        {
            Environment = environment;
            Evaluation = Environment.Evaluation;
            _Scopes.Push(RuntimeScope.CreateRoot(variables));
        }


        private Stack<RuntimeScope> _Scopes = new Stack<RuntimeScope>();
        public RuntimeScope CurrentScope => _Scopes.Peek();
        public void PushScope() => _Scopes.Push(CurrentScope.CreateChild());
        public void PopScope() => _Scopes.Pop();

        public IEnumerable<RenderObj> Transform(ContainerNode item)
        {
            yield return new WhiteSpaceControlRenderObj(item.StartWhiteSpace, WhiteSpaceControlPosition.BeforeThis, $"Before Container | {item.GetType().Name}");
            foreach (var obj in item.Children.SelectMany(child => child.Transform(this)))
            {
                yield return obj;
            }
            yield return new WhiteSpaceControlRenderObj(item.EndWhiteSpace, WhiteSpaceControlPosition.AfterThis, $"After Container | {item.GetType().Name}");
        }

        public IEnumerable<RenderObj> Transform(ExpressionNode item)
        {
            var result = Evaluation.Evaluate(item.Expression, CurrentScope.Variables);
            yield return new WhiteSpaceControlRenderObj(item.StartWhiteSpace, WhiteSpaceControlPosition.BeforeThis, $"Before Expression {item.ToString(debug: true)} | {item.GetType().Name}");
            yield return new LiteralRenderObj(result);
            yield return new WhiteSpaceControlRenderObj(item.EndWhiteSpace, WhiteSpaceControlPosition.AfterThis, $"After Expression {item.ToString(debug: true)} | {item.GetType().Name}");
        }

        public IEnumerable<RenderObj> Transform(NewLineNode item)
        {
            yield return new NewLineRenderObj(item.ToString(), item.ControlMode);
        }

        public IEnumerable<RenderObj> Transform(OutputNode item)
        {
            yield return new LiteralRenderObj(item.ToString());
        }

        public IEnumerable<RenderObj> Transform(WhiteSpaceNode item)
        {
            yield return new WhiteSpaceRenderObj(item.ToString(), item.WhiteSpaceControlMode);
        }

        public IEnumerable<RenderObj> Transform(ForNode item)
        {
            // Setup the enumerator
            
            var expression = Evaluation.Evaluate(item.Expression.Expression, CurrentScope.Variables); // TODO: Variables
            if (expression == null)
            {
                if (Environment.Settings.TreatNullCollectionsAsEmpty == false)
                {
                    throw new NullReferenceException();
                }
                expression = Enumerable.Empty<object>();
            }
            var enumerator = EnumeratorFactory.GetEnumerator(expression);
            if (enumerator == null)
            {
                throw new NotImplementedException();
            }
            var didIterate = false;

            // Begin Primary Block
            yield return new WhiteSpaceControlRenderObj(item.StartWhiteSpace, WhiteSpaceControlPosition.BeforeThis, "Before For Loop");
            while (enumerator.MoveNext())
            {
                PushScope();
                // TODO: Support multiple variable names
                CurrentScope.AddLocalVariable(item.VariableNames[0], enumerator.Current);
                didIterate = true;
                yield return new WhiteSpaceControlRenderObj(item.PrimaryBlock.StartWhiteSpace, WhiteSpaceControlPosition.AfterThis, $"For Item {enumerator.Current}");
                foreach (var transformedChildItem in item.PrimaryBlock.Children.SelectMany(child => child.Transform(this)))
                {
                    yield return transformedChildItem;
                }
                yield return new WhiteSpaceControlRenderObj(item.PrimaryBlock.EndWhiteSpace, WhiteSpaceControlPosition.BeforeThis, $"For Item {enumerator.Current}");
                PopScope();
            }

            // Begin Else Block
            if (didIterate == false && item.ElseBlock != null)
            {
                yield return new WhiteSpaceControlRenderObj(item.PrimaryBlock.StartWhiteSpace, WhiteSpaceControlPosition.AfterThis, $"For Item [ELSE]");
                foreach (var transformedChildItem in item.ElseBlock.Children.SelectMany(child => child.Transform(this)))
                {
                    yield return transformedChildItem;
                }
                yield return new WhiteSpaceControlRenderObj(item.PrimaryBlock.EndWhiteSpace, WhiteSpaceControlPosition.BeforeThis, $"For Item [ELSE]");
            }
        }

        public IEnumerable<RenderObj> Transform(IfNode item)
        {
            yield return new WhiteSpaceControlRenderObj(item.StartWhiteSpace, WhiteSpaceControlPosition.BeforeThis, "Before If Node");
            foreach (var condition in item.Conditions)
            {
                if (Evaluation.EvaluateAs<bool>(condition.Expression.Expression, CurrentScope.Variables))
                {
                    yield return new WhiteSpaceControlRenderObj(condition.StartWhiteSpace, WhiteSpaceControlPosition.AfterThis, $"If Clause {condition.Expression}");
                    foreach (var obj in condition.Children.SelectMany(child => child.Transform(this)))
                    {
                        yield return obj;
                    }
                    yield return new WhiteSpaceControlRenderObj(condition.StartWhiteSpace, WhiteSpaceControlPosition.AfterThis, $"If Clause {condition.Expression}");
                    break;
                }
            }
            yield return new WhiteSpaceControlRenderObj(item.EndWhiteSpace, WhiteSpaceControlPosition.AfterThis, "After If Node");
        }

        public IEnumerable<RenderObj> Transform(ConditionalNode item)
        {
            yield return new WhiteSpaceControlRenderObj(item.StartWhiteSpace, WhiteSpaceControlPosition.BeforeThis, $"Before ConditionalNode | {item.GetType().Name}");
            foreach (var obj in item.Children.SelectMany(child => child.Transform(this)))
            {
                yield return obj;
            }
            yield return new WhiteSpaceControlRenderObj(item.EndWhiteSpace, WhiteSpaceControlPosition.AfterThis, $"After ConditionalNode | {item.GetType().Name}");
        }

        public IEnumerable<RenderObj> Transform(CommentNode item)
        {
            yield break;
        }

        public IEnumerable<RenderObj> Transform(BlockNode item)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RenderObj> Transform(ExtendsNode item)
        {
            throw new NotImplementedException();
        }
    }

}
