using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Common;
using Common.ExpressionCreators;
using ExpressionParser.Scopes;
using Obsidian.AST;
using Obsidian.AST.Nodes;
using Obsidian.AST.Nodes.MiscNodes;
using Obsidian.AST.Nodes.Statements;

namespace Obsidian.Transforming
{
    public class NewASTCompiler : ITransformVisitor<Expression>
    {
        internal static Expression ToExpression(JinjaEnvironment environment, ASTNode node, IDictionary<string, object?> variableTemplate,
               out NewASTCompiler compiler)
        {
            compiler = new NewASTCompiler(environment, variableTemplate);
            return node.Transform(compiler);
        }

        private NewASTCompiler(JinjaEnvironment environment, IDictionary<string, object?> variableTemplate)
        {
            Environment = environment;
            _RootScope = RootScope.CreateRootScope(variableTemplate);
            _Scopes.Push(_RootScope);
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
            return Expression.Block(
                TransformAll(item.Children)
                    .Concat(Expression.Constant(string.Empty))
            );
        }

        public Expression Transform(ForNode item)
        {
            throw new NotImplementedException();
        }

        public Expression Transform(ContainerNode item)
        {
            return Expression.Block(
                ExpressionEx.Console.WriteLine($"START: {item.GetType().Name}").YieldOne()
                    .Concat(TransformAll(item.Children))
                    .Concat(ExpressionEx.Console.WriteLine($"END: {item.GetType().Name}"))
            );
        }

        public Expression Transform(ExpressionNode item)
        {
            var expression = Environment.Evaluation.ToExpression(item.Expression, CurrentScope);
            if (item.Output)
            {
                return SpecialVariables.StringBuilder.Append(item.ToString());
            }
            return expression;
        }

        public Expression Transform(NewLineNode item)
        {
            return ExpressionEx.Console.WriteLine($"ITEM: {item.GetType().Name} : {item}");
        }

        public Expression Transform(OutputNode item)
        {
            return ExpressionEx.Console.WriteLine($"ITEM: {item.GetType().Name} : {item.Value}");
        }

        public Expression Transform(WhiteSpaceNode item)
        {
            return ExpressionEx.Console.WriteLine($"ITEM: {item.GetType().Name} : {item}");
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
            return ifBlock;
        }

        public Expression Transform(ConditionalNode item)
        {
            return Expression.Block(
                ExpressionEx.Console.WriteLine($"START: {item.GetType().Name}").YieldOne()
                    .Concat(ExpressionEx.Console.WriteLine("Expression:"))
                    .Concat(item.Expression.Transform(this))
                    .Concat(ExpressionEx.Console.WriteLine("Conditions:"))
                    .Concat(TransformAll(item.Children))
                    .Concat(ExpressionEx.Console.WriteLine($"END: {item.GetType().Name}"))
            );
        }

        public Expression Transform(CommentNode item)
        {
            return ExpressionEx.Console.WriteLine($"ITEM: {item.GetType().Name} : {item}");
        }

        public Expression Transform(BlockNode item)
        {
            return Expression.Block(
                ExpressionEx.Console.WriteLine($"START: {item.GetType().Name}").YieldOne()
                    .Concat(ExpressionEx.Console.Write("Name:"))
                    .Concat(ExpressionEx.Console.WriteLine(item.Name))
                    .Concat(TransformAll(item.Children))
                    .Concat(ExpressionEx.Console.WriteLine($"END: {item.GetType().Name}"))
            );
        }

        public Expression Transform(ExtendsNode item)
        {
            return ExpressionEx.Console.WriteLine($"ITEM: {item.GetType().Name} : {item.TemplateName}");
        }
    }
}
