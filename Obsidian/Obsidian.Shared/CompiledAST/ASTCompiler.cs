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
using Obsidian.AST;
using Obsidian.AST.Nodes;
using Obsidian.AST.Nodes.MiscNodes;
using Obsidian.AST.Nodes.Statements;
using Obsidian.Transforming;
using StringBuilder = System.Text.StringBuilder;

namespace Obsidian.CompiledAST
{
    public class ASTCompiler : ITransformVisitor<Expression>, IForceTransformVisitor<Expression>
    {
        private class SpecialVariableInfo
        {
            public class AssignInfo<T> : AssignInfo
            {
                public AssignInfo(ExpressionExtensionData<T> extensionData, BinaryExpression assignmentExpression) : base(extensionData, assignmentExpression)
                {
                    TypedExtensionData = extensionData;
                }
                public ExpressionExtensionData<T> TypedExtensionData { get; }
            }
            public class AssignInfo
            {
                public AssignInfo(ExpressionExtensionData extensionData, BinaryExpression assignmentExpression)
                {
                    ExtensionData = extensionData;
                    AssignmentExpression = assignmentExpression;
                }
                public ExpressionExtensionData ExtensionData { get; }
                public BinaryExpression AssignmentExpression { get; }
            }

            public SpecialVariableInfo(AssignInfo<StringBuilder> stringBuilder, AssignInfo<Self> self)
            {
                _StringBuilder = stringBuilder;
                _Self = self;
            }

            private AssignInfo<StringBuilder> _StringBuilder;
            private AssignInfo<Self> _Self;

            public ExpressionExtensionData<StringBuilder> StringBuilder => _StringBuilder.TypedExtensionData;
            public ExpressionExtensionData<Self> Self => _Self.TypedExtensionData;

            public IEnumerable<AssignInfo> AllAssignInfo => new AssignInfo[]
            {
                _StringBuilder,
                _Self,
            };

            public IEnumerable<ParameterExpression> AllVariables => AllAssignInfo.Select(x => x.ExtensionData.ParameterExpression);
            public IEnumerable<BinaryExpression> AllAssignments => AllAssignInfo.Select(x => x.AssignmentExpression);
        }


        private ASTCompiler(SpecialVariableInfo specialVariables, JinjaEnvironment environment, IDictionary<string, object?> variableTemplate)
        {
            _RootScope = RootScope.CreateRootScope(variableTemplate);
            _Scopes = new Stack<Scope>();
            _Scopes.Push(_RootScope);
            _Environment = environment;
            SpecialVariables = specialVariables;
        }

        private JinjaEnvironment _Environment;
        private RootScope _RootScope;
        private Stack<Scope> _Scopes;
        private SpecialVariableInfo SpecialVariables { get; }
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
            var specialVariables = SetupVariables();
            var compiler = new ASTCompiler(specialVariables, environment, variableTemplate);
            var compiledNodes = node.Transform(compiler);
            var toString = Expression.Call(specialVariables.StringBuilder.ParameterExpression, 
                typeof(StringBuilder).GetMethod("ToString", Type.EmptyTypes)
            );
            var allContent = specialVariables.AllAssignments.Concat(compiledNodes).Concat(toString);
            var finalBlock = Expression.Block(specialVariables.AllVariables, allContent);
            //var debug = finalBlock.ToString("C#");
            return ExpressionData.CreateCompiled(finalBlock, compiler.CurrentScope.FindRootScope());
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




        public Expression Transform(ContainerNode item) => Transform(item, false);
        public Expression Transform(ContainerNode item, bool forceRender)
        {
            if (forceRender)
            {
                return Expression.Block(TransformAll(item.Children, forceRender));
            }
            return IfInDirectRenderMode(
                Expression.Block(TransformAll(item.Children, forceRender)),
                Expression.Empty()
            );
        }

        public Expression Transform(ExpressionNode item) => Transform(item, false);
        public Expression Transform(ExpressionNode item, bool forceRender)
        {
            var expression = _Environment.Evaluation.ToExpression(item.Expression, CurrentScope);
            if (item.Output)
            {
                if (forceRender)
                {
                    return SpecialVariables.StringBuilder.Append(item.ToString());
                }
                expression = IfInDirectRenderMode(
                    SpecialVariables.StringBuilder.Append(item.ToString()),
                    Expression.Empty()
                );
            }
            return expression;
        }

        public Expression Transform(NewLineNode item) => Transform(item, false);
        public Expression Transform(NewLineNode item, bool forceRender)
        {
            if (forceRender)
            {
                return SpecialVariables.StringBuilder.Append(item.ToString());
            }
            return IfInDirectRenderMode(
                SpecialVariables.StringBuilder.Append(item.ToString()),
                Expression.Empty()
            );
        }

        public Expression Transform(OutputNode item) => Transform(item, false);
        public Expression Transform(OutputNode item, bool forceRender)
        {
            if (forceRender)
            {
                return SpecialVariables.StringBuilder.Append(item.Value);
            }
            return IfInDirectRenderMode(
                SpecialVariables.StringBuilder.Append(item.Value),
                Expression.Empty()
            );
        }

        public Expression Transform(WhiteSpaceNode item) => Transform(item, false);
        public Expression Transform(WhiteSpaceNode item, bool forceRender)
        {
            if (forceRender)
            {
                return SpecialVariables.StringBuilder.Append(item.ToString());
            }
            return IfInDirectRenderMode(
                SpecialVariables.StringBuilder.Append(item.ToString()),
                Expression.Empty()
            );
        }

        public Expression Transform(IfNode item) => Transform(item, false);
        public Expression Transform(IfNode item, bool forceRender)
        {
            if(item.Conditions.Length == 1)
            {
                var onlyCondition = item.Conditions[0].Expression.Transform(this, forceRender);
                var body = Expression.Block(item.Conditions[0].Children.Select(child => child.Transform(this, forceRender)));
                return Expression.IfThen(onlyCondition, body);
            }
            var conditions = item.Conditions;


            var condition = conditions[conditions.Length - 1].Expression.Transform(this, forceRender);
            var block = Expression.Block(conditions[conditions.Length - 1].Children.Select(child => child.Transform(this, forceRender)));

            var ifBlock = Expression.IfThen(condition, block);


            for(int i = conditions.Length - 2; i >= 0; --i)
            {
                condition = conditions[i].Expression.Transform(this, forceRender);
                block = Expression.Block(conditions[i].Children.Select(child => child.Transform(this, forceRender)));
                ifBlock = Expression.IfThenElse(condition, block, ifBlock);
            }


            if (forceRender)
            {
                return ifBlock;
            }
            return IfInDirectRenderMode(
                ifBlock,
                Expression.Empty()
            );
        }

        public Expression Transform(ConditionalNode item) => Transform(item, false);
        public Expression Transform(ConditionalNode item, bool forceRender)
        {
            throw new NotImplementedException();
        }

        public Expression Transform(CommentNode item) => Transform(item, false);
        public Expression Transform(CommentNode item, bool forceRender)
        {
            throw new NotImplementedException();
        }

        public Expression Transform(BlockNode item) => Transform(item, false);
        public Expression Transform(BlockNode item, bool forceRender)
        {
            var blockExpression = this.Transform(item.BlockContents, forceRender: true);
            // TODO: Actually process this...
            if (forceRender)
            {
                return blockExpression;
            }
            return IfInDirectRenderMode(
                blockExpression,
                SpecialVariables.Self.AddBlock(item.Name, blockExpression)
            );
        }

        public Expression Transform(ExtendsNode item) => Transform(item, false);
        public Expression Transform(ExtendsNode item, bool forceRender)
        {
            Expression template;
            ;
            if (_Environment.Evaluation.IsLiteralValue(item.Template.Expression))
            {
                var resolved = _Environment.Evaluation.EvaluateAs<string>(item.Template.Expression);
                template = Expression.Constant(_Environment.GetTemplate(resolved, new Dictionary<string, object?>()));
            }
            else
            {
                template = item.Template.Transform(this, forceRender);
                if (template.Type != typeof(Template))
                {
                    throw new NotImplementedException();
                }
            }

            var property = Expression.Property(template, nameof(Template.TemplateNode));
            if (forceRender)
            {
                return SpecialVariables.Self.EnqueueIntoTemplateQueue(property);
            }
            return Expression.Block(
                SpecialVariables.Self.EnqueueIntoTemplateQueue(property),
                SpecialVariables.Self.SetRenderMode(RenderMode.ParentAtCompletion)
            );
        }


        private Expression IfInDirectRenderMode(Expression ifInDirectRenderMode, Expression ifNotInDirectRenderMode)
        {
            return Expression.IfThenElse(
                Expression.Equal(SpecialVariables.Self.GetRenderMode(), Expression.Constant(RenderMode.Direct)),
                ifInDirectRenderMode,
                ifNotInDirectRenderMode
            );
        }

        private IEnumerable<Expression> TransformAll(IEnumerable<ASTNode> nodes, bool forceRender)
        {
            return nodes.Select(node => node.Transform(this, forceRender));
        }

        public Expression Transform(ForNode item) => Transform(item, false);
        public Expression Transform(ForNode item, bool forceRender)
        {
            if (ExpressionEx.ToArray(item.Expression.Transform(this, forceRender), out var expression, out var elementType) == false ||
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
                item.PrimaryBlock.Transform(this, forceRender)
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
                overall = Expression.IfThenElse(enumerateClause, whileLoop, item.ElseBlock.Transform(this, forceRender));
            }

            var forLoop = PopScope("For Loop - Overall", arrayAssignment, overall);


            if (forceRender)
            {
                return forLoop;
            }
            return IfInDirectRenderMode(
                forLoop,
                Expression.Empty()
            );
        }

        public Expression Transform(TemplateNode item) => Transform(item, false);
        public Expression Transform(TemplateNode item, bool forceRender)
        {
            var children = TransformAll(item.Children, forceRender);

            var loopBreak = Expression.Label("breakLabel");
            var loopContents = Expression.Block(
                Expression.IfThen(
                    Expression.Equal(SpecialVariables.Self.HasQueuedTemplates(), Expression.Constant(false)),
                    Expression.Break(loopBreak)
                ),
                SpecialVariables.Self.SetRenderMode(RenderMode.Direct),
                Expression.Property(SpecialVariables.Self.DequeueTemplate(), nameof(ExpressionData.ExpressionTree))
            );

            return Expression.Block(children.Concat(Expression.Loop(loopContents, loopBreak)));
        }
    }
}
