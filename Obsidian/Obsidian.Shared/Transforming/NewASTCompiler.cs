#if DEBUG

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Common;
using Common.ExpressionCreators;
using ExpressionParser;
using ExpressionParser.Scopes;
using Obsidian.AST;
using Obsidian.AST.Nodes;
using Obsidian.AST.Nodes.MiscNodes;
using Obsidian.AST.Nodes.Statements;
using Obsidian.ExpressionCreators;
using StringBuilder = System.Text.StringBuilder;

namespace Obsidian.Transforming
{
    internal class NewASTCompiler : ITransformVisitor<Expression>
    {
        private const string SCOPE_NAME_INTERNAL = "Internal";
        private const string VARNAME_STRING_BUILDER = "stringBuilder";
        private const string VARNAME_STRING_SELF = "self";
        private const string SCOPE_NAME_TEMPLATE = "TEMPLATE: {0}";

        internal Expression SelfVar => CurrentScope[VARNAME_STRING_SELF];
        internal Expression StringBuilderVar => CurrentScope[VARNAME_STRING_BUILDER];

        internal static Expression ToExpression(string templateName, JinjaEnvironment environment, ASTNode node, out NewASTCompiler compiler, CompiledScope rootScope)
        {
            compiler = new NewASTCompiler(environment, rootScope);
            compiler.PushScope(SCOPE_NAME_INTERNAL);
            var internalVariableAssignments = new Expression[]
            {
                CreateVariable(compiler, VARNAME_STRING_BUILDER, Expression.New(typeof(StringBuilder))),
                CreateVariable(compiler, VARNAME_STRING_SELF, Expression.New(typeof(CompiledSelf)))
            };
            compiler.PushScope(string.Format(CultureInfo.InvariantCulture, SCOPE_NAME_TEMPLATE, templateName));
            var compiledNodes = node.Transform(compiler);
            var contentBlock = compiler.PopScope(string.Format(CultureInfo.InvariantCulture, SCOPE_NAME_TEMPLATE, templateName), compiledNodes);

            var toString = ExpressionEx.Object.ToStringEx(compiler.StringBuilderVar);
            var internalContent = internalVariableAssignments
                .Concat(contentBlock)
                .Concat(toString);


            var internalBlock = compiler.PopScope(SCOPE_NAME_INTERNAL, internalContent);

            return internalBlock;

            static BinaryExpression CreateVariable(NewASTCompiler localCompiler, string name, Expression createExpression)
            {
                localCompiler.CurrentScope.DefineAndSetVariable(name, createExpression, out var assignExpression);
                return assignExpression;
            }
        }


        private NewASTCompiler(JinjaEnvironment environment, CompiledScope scope)
        {
            Environment = environment;
            _Scopes.Push(scope);
        }

        internal JinjaEnvironment Environment { get; }
        private readonly Stack<CompiledScope> _Scopes = new Stack<CompiledScope>();
        internal CompiledScope CurrentScope => _Scopes.Peek();

        internal void PushScope(string name)
        {
            _Scopes.Push(CurrentScope.CreateCompiledChild(name));
        }
        internal BlockExpression PopScope(string name, Expression childOne, params Expression[] children)
        {
            return PopScope(name, childOne.YieldOne().Concat(children));
        }
        internal BlockExpression PopScope(string name, IEnumerable<Expression> children)
        {
            var scope = _Scopes.Pop();
            var block = scope.CloseScope(children.ToArray());
            if (scope.Name != name)
            {
                throw new NotImplementedException();
            }
            return block;
        }

        protected virtual IEnumerable<Expression> TransformAll(IEnumerable<ASTNode> nodes)
        {
            return nodes.Select(node => node.Transform(this));
        }



        public Expression Transform(ForNode item)
        {
            throw new NotImplementedException();
        }

        public Expression Transform(ContainerNode item)
        {
            return Expression.Block(TransformAll(item.Children));
        }

        public Expression Transform(ExpressionNode item)
        {
            var expression = Environment.Evaluation.ToExpression(item.Expression, CurrentScope);
            if (item.Output)
            {
#if DEBUG
                return IfRenderMode(ExpressionEx.Console.Write(expression), Expression.Empty());
#endif
            }
            return expression;
        }

        public Expression Transform(NewLineNode item)
        {
            return Expression.Empty();
            //return ExpressionEx.Console.Write(item.ToString());
        }

        public Expression Transform(OutputNode item)
        {

            var direct = ExpressionEx.Console.Write(item.Value);
            var renderAtCompletion = ExpressionEx.Console.Write($"Render at completion: {item.GetType().Name} {item.Value}");
            return IfRenderMode(direct, renderAtCompletion);
        }

        public Expression Transform(WhiteSpaceNode item)
        {
            return ExpressionEx.Console.Write(item.ToString());
        }

        public Expression Transform(IfNode item)
        {
            if (item.Conditions.Length == 1)
            {
                var onlyCondition = item.Conditions[0].Expression.Transform(this);
                var body = Expression.Block(item.Conditions[0].Children.Select(child => child.Transform(this)));
                return Expression.IfThen(onlyCondition, body);
            }
            var conditions = item.Conditions;


            var condition = conditions[conditions.Length - 1].Expression.Transform(this);
            var block = Expression.Block(conditions[conditions.Length - 1].Children.Select(child => child.Transform(this)));

            var ifBlock = Expression.IfThen(condition, block);


            for (int i = conditions.Length - 2; i >= 0; --i)
            {
                condition = conditions[i].Expression.Transform(this);
                block = Expression.Block(conditions[i].Children.Select(child => child.Transform(this)));
                ifBlock = Expression.IfThenElse(condition, block, ifBlock);
            }

            var renderAtCompletion = ExpressionEx.Console.WriteLine($"Render at completion: {item.GetType().Name}");
            return IfRenderMode(ifBlock, renderAtCompletion);
        }

        public Expression Transform(ConditionalNode item)
        {
            var direct = Expression.Block(
                ExpressionEx.Console.WriteLine($"START: {item.GetType().Name}").YieldOne()
                    .Concat(ExpressionEx.Console.WriteLine("Expression:"))
                    .Concat(item.Expression.Transform(this))
                    .Concat(ExpressionEx.Console.WriteLine("Conditions:"))
                    .Concat(TransformAll(item.Children))
                    .Concat(ExpressionEx.Console.WriteLine($"END: {item.GetType().Name}"))
            );
            var renderAtCompletion = ExpressionEx.Console.WriteLine($"Render at completion: {item.GetType().Name} {item.Expression.Expression}");
            return IfRenderMode(direct, renderAtCompletion);
        }

        public Expression Transform(CommentNode item)
        {
            return ExpressionEx.Console.WriteLine($"ITEM: {item.GetType().Name} : {item}");
        }

        //public Expression Transform(BlockNode item)
        //{
        //    ExpressionData MakeCompiledNode()
        //    {
        //        var internalScope = CurrentScope.FindScope(SCOPE_NAME_INTERNAL);
        //        var rootScope = CurrentScope.FindRootScope();
        //        var scopeName = string.Format(CultureInfo.InvariantCulture, SCOPE_NAME_BLOCK, item.Name);
        //        var blockScope = Scope.CreateDerivedRootScope(scopeName, internalScope, rootScope);

        //        _Scopes.Push(blockScope); // Do this manually, it's a special case.
        //        var children = Expression.Block(TransformAll(item.Children));
        //        _Scopes.Pop(); // Don't "Close out" the scope - just discard it.  We'll pass the scope and the expressions to ExpressionData.CreateCompiled - it'll handle parameters and stuff.

        //        return ExpressionData.CreateCompiled(children, blockScope);
        //    }

        //Direct: Get the newest block from Self.Print that.
        //    var scopeDictionaryValues = CurrentScope.ToDictionary();
        //    var direct = ExpressionEx.Console.WriteLine(
        //        Expression.Call(
        //            SelfEx.GetBlock(SelfVar, item.Name),
        //            nameof(Block.Render),
        //            Type.EmptyTypes,
        //            scopeDictionaryValues
        //        )
        //    );

        //    Render at completion: Add the block to Self.
        //   var addBlock = Expression.Block(
        //       SelfEx.AddBlock(SelfVar, item.Name, MakeCompiledNode())
        //   );

        //    var ifRender = IfRenderMode(direct, Expression.Empty());
        //    return Expression.Block(
        //        addBlock,
        //        ifRender
        //    );
        //}


        public Expression Transform(TemplateNode item)
        {
            var initialTemplate = TransformAll(item.Children).Concat(Expression.Constant(string.Empty));

            var breakLabel = Expression.Label("breakLoop");

            var loopBody = Expression.Block(
                Expression.IfThen(
                    Expression.Equal(SelfEx.TemplateQueueCount(SelfVar), Expression.Constant(0)),
                    Expression.Break(breakLabel)
                )
#if DEBUG
                ,ExpressionEx.Console.Write("Queue Count: "),
                ExpressionEx.Console.WriteLine(SelfEx.TemplateQueueCount(SelfVar)),
#endif
                //Expression.Constant(SelfEx.DequeueTemplate(SelfVar)),

                //ExpressionEx.Console.WriteLine(SelfEx.DequeueTemplate(SelfVar)),
#if DEBUG
                ExpressionEx.Console.Write("Queue Count: "),
                ExpressionEx.Console.WriteLine(SelfEx.TemplateQueueCount(SelfVar))
#endif
            );

            var loop = Expression.Loop(loopBody, breakLabel);

            return Expression.Block(
                initialTemplate.Concat(loop).Concat(Expression.Constant(""))
            );
        }


        public Expression Transform(BlockNode item)
        {
            PushScope($"Block: {item.Name}");
            var children = TransformAll(item.Children).ToArray();
            var block = PopScope($"Block: {item.Name}", children);

            var addBlockToSelf = SelfEx.AddBlock(SelfVar, item.Name, block);

            return Expression.Block(addBlockToSelf, IfRenderMode(block, Expression.Empty()));

        }

        public Expression Transform(ExtendsNode item)
        {
            Expression expr;
            if (Environment.Evaluation.IsLiteralValue(item.TemplateName) == false) throw new NotImplementedException();
            // Since we're only evaluating this to get the name of the template, we don't need to pass in a scope to Evaluation.
            var parsedTemplateName = Environment.Evaluation.Evaluate(item.TemplateName)?.ToString() ?? string.Empty;


            // For *TEMPLATES*, we pass in stringBuilder and self as parameters, as well as any globals.

            //var internalScope = CurrentScope.FindScope(SCOPE_NAME_INTERNAL);
            //var rootScope = CurrentScope.FindRootScope();
            //var scopeName = string.Format(CultureInfo.InvariantCulture, SCOPE_NAME_TEMPLATE, parsedTemplateName);
            //var templateScope = Scope.CreateDerivedRootScope(scopeName, internalScope, rootScope);

            //expr = Expression.Constant(Environment.GetTemplate(parsedTemplateName, templateScope));
            expr = Environment.GetTemplateExpression(parsedTemplateName, CurrentScope);





            var quoted = Expression.Quote(Expression.Lambda(expr));
            return Expression.Block(
                ExpressionEx.Console.Write("Template Queue:   "),
                ExpressionEx.Console.WriteLine(SelfEx.TemplateQueueCount(SelfVar)),
                ExpressionEx.Console.WriteLine("Adding to queue"),
                //SelfEx.EnqueueIntoTemplateQueue(SelfVar, expr),
                SelfEx.EnqueueIntoTemplateQueue(SelfVar, quoted),
                ExpressionEx.Console.Write("Template Queue:   "),
                ExpressionEx.Console.WriteLine(SelfEx.TemplateQueueCount(SelfVar)),
                SelfEx.SetRenderMode(SelfVar, RenderMode.ParentAtCompletion),
                ExpressionEx.Console.Write("Setting render mode...   "),
                ExpressionEx.Console.WriteLine(SelfEx.RenderMode(SelfVar))
            );

        }
        private Expression IfRenderMode(Expression direct, Expression parentAtCompletion)
        {
            return Expression.IfThenElse(
                Expression.Equal(
                    SelfEx.RenderMode(SelfVar),
                    Expression.Constant(RenderMode.Direct)
                ),
                direct,
                parentAtCompletion
            );
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


#endif