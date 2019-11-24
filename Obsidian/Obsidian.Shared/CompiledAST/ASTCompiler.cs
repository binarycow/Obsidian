using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Common;
using Common.ExpressionCreators;
using ExpressionParser;
using ExpressionParser.Scopes;
using ExpressionToString;
using Obsidian.AST;
using Obsidian.AST.Nodes;
using Obsidian.AST.Nodes.MiscNodes;
using Obsidian.AST.Nodes.Statements;
using Obsidian.Transforming;
using StringBuilder = System.Text.StringBuilder;

namespace Obsidian.CompiledAST
{
    public class ASTCompiler : ITransformVisitor<Expression>
    {
        private ASTCompiler(ExpressionExtensionData<StringBuilder> stringBuilder, JinjaEnvironment environment, IDictionary<string, object?> variableTemplate)
        {
            _RootScope = RootScope.CreateRootScope(variableTemplate);
            _Scopes = new Stack<Scope>();
            _Scopes.Push(_RootScope);
            _Environment = environment;
            _StringBuilder = stringBuilder;
        }

        private JinjaEnvironment _Environment;
        private RootScope _RootScope;
        private Stack<Scope> _Scopes;
        private ExpressionExtensionData<StringBuilder> _StringBuilder;
        public Scope CurrentScope => _Scopes.Peek();

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
            if(scope.Name != name)
            {
                throw new NotImplementedException();
            }
            return block;
        }
        public BlockExpression PopScope(string name, Expression child)
        {
            return PopScope(name, child.YieldOne());
        }

        internal static ExpressionData Compile(JinjaEnvironment environment, ASTNode node, IDictionary<string, object?> variableTemplate)
        {

            var stringBuilderVariable = Expression.Variable(typeof(StringBuilder), "stringBuilder");
            if (ExpressionExtensionData.TryCreate<StringBuilder>(stringBuilderVariable, out var stringBuilderExpressionExtensionData,
                out var newExpression) == false || stringBuilderExpressionExtensionData == null)
            {
                throw new NotImplementedException(); // Couldn't create variable
            }
            var assignExpression = Expression.Assign(stringBuilderVariable, newExpression);



            //var stringBuilderVariable = Expression.Variable(typeof(StringBuilder), "stringBuilder");
            //var assignStringBuilder = Expression.Assign(stringBuilderVariable, Expression.New(typeof(StringBuilder)));



            var compiler = new ASTCompiler(stringBuilderExpressionExtensionData, environment, variableTemplate);
            var children = new Queue<Expression>();
            var compiledNodes = node.Transform(compiler);
            var toString = Expression.Call(stringBuilderVariable, typeof(StringBuilder).GetMethod("ToString", Type.EmptyTypes));
            var allContent = new[] { assignExpression }.Concat(compiledNodes).Concat(toString);
            var finalBlock = Expression.Block(stringBuilderVariable.YieldOne(), allContent);
            var debug = finalBlock.ToString("C#");
            return ExpressionData.CreateCompiled(finalBlock, compiler.CurrentScope.FindRootScope());
        }



        public Expression Transform(ForNode item)
        {
            if (ExpressionEx.ToArray(item.Expression.Transform(this), out var expression, out var elementType) == false ||
                expression == null || elementType == null)
            {
                throw new NotImplementedException();
            }


            PushScope("For Loop - Overall"); // Overall

            // Define overall variables (array and loopIndex)
            var breakLabel = Expression.Label("loopBreak");
            var arrayVariable = CurrentScope.AddLocalVariable("array", expression, out var arrayAssignment);

            PushScope("Setup and contents of While Loop");
            var loopIndex = CurrentScope.AddLocalVariable("loopIndex", Expression.Constant(0), out var initLoopIndex); // TODO: Add a way to have an unnamed variable, so I don't conflict with the user's choice for var names
            if (ExpressionParser.Reflection.Array.TryLength(arrayVariable, out var arrayLength) == false) throw new NotImplementedException();



            PushScope("For Loop - Contents"); // Contents of loop
            var itemVariable = CurrentScope.AddLocalVariable(item.VariableNames[0], Expression.ArrayIndex(arrayVariable, loopIndex), out var setItem); // "item"
            //var loopVariable = CurrentScope.AddAndCreateLocalVariable_Generic("loop", out var setLoopVariable, elementType, arrayVariable, loopIndex);

            Expression setLoopInfoVar;
            if (elementType.IsValueType)
            {
                throw new NotImplementedException();
            }
            else
            {
                var loopInfoVar = CurrentScope.AddLocalVariable("loop",
                    ExpressionEx.New_Generic(typeof(LoopInfoClass<>), new[] { elementType }, arrayVariable, loopIndex),
                    out setLoopInfoVar);
            }

            
            var usersBlock = PopScope("For Loop - Contents",
                setItem,
                setLoopInfoVar,
                //Expression.Break(breakLabel),
                item.PrimaryBlock.Transform(this)
            );

            var insideIf = Expression.Block(
                usersBlock,
                Expression.PreIncrementAssign(loopIndex)
            );
            var loopCondition = Expression.LessThan(loopIndex, arrayLength);
            var ifElse = Expression.IfThenElse(loopCondition, insideIf, Expression.Break(breakLabel));

            var loop = Expression.Loop(ifElse, breakLabel);

            var whileLoop = PopScope("Setup and contents of While Loop",
                initLoopIndex,
                loop
            );

            var enumerateClause = Expression.AndAlso(
                Expression.NotEqual(arrayVariable, Expression.Constant(null)),
                Expression.GreaterThan(arrayLength, Expression.Constant(0))
            );

            Expression overall;
            if (item.ElseBlock == null)
            {
                overall = Expression.IfThen(enumerateClause, whileLoop);
            }
            else
            {
                overall = Expression.IfThenElse(enumerateClause, whileLoop, item.ElseBlock.Transform(this));
            }

            return PopScope("For Loop - Overall", arrayAssignment, overall);
        }

        public Expression Transform(ContainerNode item)
        {
            return Expression.Block(item.Children.Select(child => child.Transform(this)));
        }

        public Expression Transform(ExpressionNode item)
        {
            var expression = _Environment.Evaluation.ToExpression(item.Expression, CurrentScope);
            if (item.Output)
            {
                expression = _StringBuilder.Append(expression);
            }
            return expression;
        }

        public Expression Transform(NewLineNode item)
        {
            return _StringBuilder.Append(Expression.Constant(item.ToString()));
        }

        public Expression Transform(OutputNode item)
        {
            return _StringBuilder.Append(Expression.Constant(item.Value));
        }

        public Expression Transform(WhiteSpaceNode item)
        {
            return _StringBuilder.Append(Expression.Constant(item.ToString()));
        }

        public Expression Transform(IfNode item)
        {
            if(item.Conditions.Length == 1)
            {
                var onlyCondition = item.Conditions[0].Expression.Transform(this);
                var body = Expression.Block(item.Conditions[0].Children.Select(child => child.Transform(this)));
                return Expression.IfThen(onlyCondition, body);
            }
            var conditions = item.Conditions;


            var condition = conditions[conditions.Length - 1].Expression.Transform(this);
            var block = Expression.Block(conditions[conditions.Length - 1].Children.Select(child => child.Transform(this)));

            var ifBlock = Expression.IfThen(condition, block);


            for(int i = conditions.Length - 2; i >= 0; --i)
            {
                condition = conditions[i].Expression.Transform(this);
                block = Expression.Block(conditions[i].Children.Select(child => child.Transform(this)));
                ifBlock = Expression.IfThenElse(condition, block, ifBlock);
            }
            return ifBlock;
        }

        public Expression Transform(ConditionalNode item)
        {
            throw new NotImplementedException();
        }

        public Expression Transform(CommentNode item)
        {
            throw new NotImplementedException();
        }

    }
}
