using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Common;
using Common.ExpressionCreators;
using ExpressionParser.Scopes;
using ExpressionToString;
using Obsidian.AST;
using Obsidian.AST.Nodes;
using Obsidian.AST.Nodes.MiscNodes;
using Obsidian.AST.Nodes.Statements;
using StringBuilder = System.Text.StringBuilder;

namespace Obsidian.Transforming
{
    public class NewASTCompiler : ITransformVisitor<Expression>
    {
        internal static Expression ToExpression(JinjaEnvironment environment, ASTNode node, IDictionary<string, object?> variableTemplate,
            out NewASTCompiler compiler)
        {
            var specialVariables = SetupVariables();
            compiler = new NewASTCompiler(specialVariables, environment, variableTemplate);
            var compiledNodes = node.Transform(compiler);
            var toString = Expression.Call(specialVariables.StringBuilder.ParameterExpression,
                typeof(StringBuilder).GetMethod("ToString", Type.EmptyTypes)
            );
            var allContent = specialVariables.AllAssignments.Concat(compiledNodes).Concat(toString);
            var finalBlock = Expression.Block(specialVariables.AllVariables.Concat(compiler._RootScope.RootParameterExpression), allContent);
            return finalBlock;
        }


        private static SpecialVariableInfo SetupVariables()
        {
            return new SpecialVariableInfo(
                CreateParameterless<StringBuilder>("stringBuilder"),
                CreateParameterless<Self>("self")
            );

            SpecialVariableInfo.AssignInfo<T> CreateParameterless<T>(string name)
            {
                var variable = Expression.Variable(typeof(T), name);
                if (ExpressionExtensionData.TryCreate<T>(variable, out var extensionData,
                    out var newExpression) == false || extensionData == null)
                {
                    throw new NotImplementedException(); // Couldn't create variable
                }
                var assign = Expression.Assign(variable, newExpression);
                return new SpecialVariableInfo.AssignInfo<T>(extensionData, assign);
            }
        }

        private NewASTCompiler(SpecialVariableInfo specialVariables, JinjaEnvironment environment, IDictionary<string, object?> variableTemplate)
        {
            Environment = environment;
            _RootScope = RootScope.CreateRootScope(variableTemplate);
            _Scopes.Push(_RootScope);
            SpecialVariables = specialVariables;
        }

        public JinjaEnvironment Environment { get; }
        private RootScope _RootScope;
        private Stack<Scope> _Scopes = new Stack<Scope>();
        public Scope CurrentScope => _Scopes.Peek();
        private SpecialVariableInfo SpecialVariables { get; }

        public void PushScope(string name)
        {
            _Scopes.Push(CurrentScope.CreateChild(name));
        }
        public BlockExpression PopScope(string name, Expression childOne, params Expression[] children)
        {
            return PopScope(name, childOne.YieldOne().Concat(children));
        }
        public BlockExpression PopScope(string name, IEnumerable<Expression> children)
        {
            var scope = _Scopes.Pop();
            var block = scope.CloseScope(children);
            if (scope.Name != name)
            {
                throw new NotImplementedException();
            }
            return block;
        }
        public BlockExpression PopScope(string name, Expression child)
        {
            return PopScope(name, child.YieldOne());
        }

        protected virtual IEnumerable<Expression> TransformAll(IEnumerable<ASTNode> nodes)
        {
            return nodes.Select(node => node.Transform(this));
        }


        public Expression Transform(TemplateNode item)
        {
            var initialTemplate = TransformAll(item.Children).Concat(Expression.Constant(string.Empty));

            var breakLabel = Expression.Label("breakLoop");
            var loopBody = Expression.Block(
                Expression.IfThen(
                    Expression.Equal(SpecialVariables.Self.TemplateQueueCount(), Expression.Constant(0)),
                    Expression.Break(breakLabel)
                ),
                ExpressionEx.Console.Write("Queue Count: "),
                ExpressionEx.Console.WriteLine(SpecialVariables.Self.TemplateQueueCount()),
                ExpressionEx.Console.WriteLine(Expression.Call(SpecialVariables.Self.DequeueTemplate(), nameof(Template.Render), Type.EmptyTypes)),
                ExpressionEx.Console.Write("Queue Count: "),
                ExpressionEx.Console.WriteLine(SpecialVariables.Self.TemplateQueueCount())
            );

            var loop = Expression.Loop(loopBody, breakLabel);

            return Expression.Block(
                initialTemplate.Concat(loop).Concat(Expression.Constant(""))
            );
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
                return IfRenderMode(ExpressionEx.Console.Write(expression), Expression.Empty());
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
            return Expression.Empty();
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

        public Expression Transform(BlockNode item)
        {
            var transformed = Expression.Block(TransformAll(item.Children));
            // Direct: Get the newest block from Self.  Print that.




            // Render at completion: Add the block to Self.

            var renderAtCompletion = Expression.Block(

            );

            var direct = Expression.Block(TransformAll(item.Children));

            return IfRenderMode(direct, renderAtCompletion);
        }

        public Expression Transform(ExtendsNode item)
        {
            Expression expr;
            if(Environment.Evaluation.IsLiteralValue(item.TemplateName))
            {
                var parsedTemplateName = Environment.Evaluation.Evaluate(item.TemplateName)?.ToString() ?? string.Empty;
                expr = Expression.Constant(Environment.GetTemplate(parsedTemplateName, new Dictionary<string, object?>()));
            }
            else
            {
                expr = item.Template.Transform(this);
            }


            return Expression.Block(
                ExpressionEx.Console.Write("Template Queue:   "),
                ExpressionEx.Console.WriteLine(SpecialVariables.Self.TemplateQueueCount()),
                ExpressionEx.Console.WriteLine("Adding to queue"),
                SpecialVariables.Self.EnqueueIntoTemplateQueue(expr),
                ExpressionEx.Console.Write("Template Queue:   "),
                ExpressionEx.Console.WriteLine(SpecialVariables.Self.TemplateQueueCount()),
                SpecialVariables.Self.SetRenderMode(RenderMode.ParentAtCompletion),
                ExpressionEx.Console.Write("Setting render mode...   "),
                ExpressionEx.Console.WriteLine(SpecialVariables.Self.GetRenderMode())
            );

        }
        private Expression IfRenderMode(Expression direct, Expression parentAtCompletion)
        {
            return Expression.IfThenElse(
                Expression.Equal(
                    SpecialVariables.Self.GetRenderMode(),
                    Expression.Constant(RenderMode.Direct)
                ),
                direct,
                parentAtCompletion
            );
        }
    }
}
