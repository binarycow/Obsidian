//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Reflection;
//using System.Text;
//using Common;
//using Common.ExpressionCreators;
//using ExpressionParser;
//using ExpressionParser.Scopes;
//using Obsidian.AST;
//using Obsidian.AST.Nodes;
//using Obsidian.AST.Nodes.MiscNodes;
//using Obsidian.AST.Nodes.Statements;
//using Obsidian.Transforming;
//using StringBuilder = System.Text.StringBuilder;
//using ExpressionToString;
//using static System.Linq.Expressions.Expression;
//using static Common.ExpressionEx;

//namespace Obsidian
//{
//    internal class ASTCompiler : ITransformVisitor<Expression>, IForceTransformVisitor<Expression>
//    {
//        private class SpecialVariableInfo
//        {
//            internal class AssignInfo<T> : AssignInfo
//            {
//                internal AssignInfo(ExpressionExtensionData<T> extensionData, BinaryExpression assignmentExpression) : base(extensionData, assignmentExpression)
//                {
//                    TypedExtensionData = extensionData;
//                }
//                internal ExpressionExtensionData<T> TypedExtensionData { get; }
//            }
//            internal class AssignInfo
//            {
//                internal AssignInfo(ExpressionExtensionData extensionData, BinaryExpression assignmentExpression)
//                {
//                    ExtensionData = extensionData;
//                    AssignmentExpression = assignmentExpression;
//                }
//                internal ExpressionExtensionData ExtensionData { get; }
//                internal BinaryExpression AssignmentExpression { get; }
//            }

//            internal SpecialVariableInfo(AssignInfo<StringBuilder> stringBuilder, AssignInfo<Self> self)
//            {
//                _StringBuilder = stringBuilder;
//                _Self = self;
//            }

//            private AssignInfo<StringBuilder> _StringBuilder;
//            private AssignInfo<Self> _Self;

//            internal ExpressionExtensionData<StringBuilder> StringBuilder => _StringBuilder.TypedExtensionData;
//            internal ExpressionExtensionData<Self> Self => _Self.TypedExtensionData;

//            internal IEnumerable<AssignInfo> AllAssignInfo => new AssignInfo[]
//            {
//                _StringBuilder,
//                _Self,
//            };

//            internal IEnumerable<ParameterExpression> AllVariables => AllAssignInfo.Select(x => x.ExtensionData.ParameterExpression);
//            internal IEnumerable<BinaryExpression> AllAssignments => AllAssignInfo.Select(x => x.AssignmentExpression);
//        }


//        private ASTCompiler(SpecialVariableInfo specialVariables, JinjaEnvironment environment, IDictionary<string, object?> variableTemplate)
//        {
//            _RootScope = Scope.CreateRootScope(variableTemplate);
//            _Scopes = new Stack<Scope>();
//            _Scopes.Push(_RootScope);
//            _Environment = environment;
//            SpecialVariables = specialVariables;
//        }

//        private JinjaEnvironment _Environment;
//        private Scope _RootScope;
//        private Stack<Scope> _Scopes;
//        private SpecialVariableInfo SpecialVariables { get; }
//        internal Scope CurrentScope => _Scopes.Peek();

//        internal void PushScope(string name)
//        {
//            _Scopes.Push(CurrentScope.CreateChild(name));
//        }
//        internal BlockExpression PopScope(string name, Expression childOne, params Expression[] children)
//        {
//            return PopScope(name, childOne.YieldOne().Concat(children));
//        }
//        internal BlockExpression PopScope(string name, IEnumerable<Expression> children)
//        {
//            var scope = _Scopes.Pop();
//            var block = scope.CloseScope(children);
//            if(scope.Name != name)
//            {
//                throw new NotImplementedException();
//            }
//            return block;
//        }
//        internal BlockExpression PopScope(string name, Expression child)
//        {
//            return PopScope(name, child.YieldOne());
//        }

//        internal ExpressionData Compile(Expression expression)
//        {
//            return ExpressionData.CreateCompiled(expression, CurrentScope.FindRootScope());
//        }
//        internal static Expression ToExpression(JinjaEnvironment environment, ASTNode node, IDictionary<string, object?> variableTemplate, 
//            out ASTCompiler compiler)
//        {
//            var specialVariables = SetupVariables();
//            compiler = new ASTCompiler(specialVariables, environment, variableTemplate);
//            var compiledNodes = node.Transform(compiler);
//            var toString = Expression.Call(specialVariables.StringBuilder.ParameterExpression,
//                typeof(StringBuilder).GetMethod("ToString", Type.EmptyTypes)
//            );
//            var allContent = specialVariables.AllAssignments.Concat(compiledNodes).Concat(toString);
//            var finalBlock = Expression.Block(specialVariables.AllVariables, allContent);
//            return finalBlock;
//        }

//        private static SpecialVariableInfo SetupVariables()
//        {
//            return new SpecialVariableInfo(
//                CreateParameterless<StringBuilder>("stringBuilder"),
//                CreateParameterless<Self>("self")
//            );

//            SpecialVariableInfo.AssignInfo<T> CreateParameterless<T>(string name)
//            {
//                var variable = Expression.Variable(typeof(T), name);
//                if (ExpressionExtensionData.TryCreate<T>(variable, out var extensionData,
//                    out var newExpression) == false || extensionData == null)
//                {
//                    throw new NotImplementedException(); // Couldn't create variable
//                }
//                var assign = Expression.Assign(variable, newExpression);
//                return new SpecialVariableInfo.AssignInfo<T>(extensionData, assign);
//            }
//        }




//        internal Expression Transform(ContainerNode item) => Transform(item, false);
//        internal Expression Transform(ContainerNode item, bool forceRender)
//        {
//            if (forceRender)
//            {
//                return Expression.Block(TransformAll(item.Children, forceRender));
//            }
//            return IfInDirectRenderMode(
//                Expression.Block(TransformAll(item.Children, forceRender)),
//                Expression.Empty()
//            );
//        }

//        internal Expression Transform(ExpressionNode item) => Transform(item, false);
//        internal Expression Transform(ExpressionNode item, bool forceRender)
//        {
//            var expression = _Environment.Evaluation.ToExpression(item.Expression, CurrentScope);
//            if (item.Output)
//            {
//                if (forceRender)
//                {
//                    return SpecialVariables.StringBuilder.Append(item.ToString());
//                }
//                expression = IfInDirectRenderMode(
//                    SpecialVariables.StringBuilder.Append(item.ToString()),
//                    Expression.Empty()
//                );
//            }
//            return expression;
//        }

//        internal Expression Transform(NewLineNode item) => Transform(item, false);
//        internal Expression Transform(NewLineNode item, bool forceRender)
//        {
//            if (forceRender)
//            {
//                return SpecialVariables.StringBuilder.Append(item.ToString());
//            }
//            return IfInDirectRenderMode(
//                SpecialVariables.StringBuilder.Append(item.ToString()),
//                Expression.Empty()
//            );
//        }

//        internal Expression Transform(OutputNode item) => Transform(item, false);
//        internal Expression Transform(OutputNode item, bool forceRender)
//        {
//            if (forceRender)
//            {
//                return SpecialVariables.StringBuilder.Append(item.Value);
//            }
//            return IfInDirectRenderMode(
//                SpecialVariables.StringBuilder.Append(item.Value),
//                Expression.Empty()
//            );
//        }

//        internal Expression Transform(WhiteSpaceNode item) => Transform(item, false);
//        internal Expression Transform(WhiteSpaceNode item, bool forceRender)
//        {
//            if (forceRender)
//            {
//                return SpecialVariables.StringBuilder.Append(item.ToString());
//            }
//            return IfInDirectRenderMode(
//                SpecialVariables.StringBuilder.Append(item.ToString()),
//                Expression.Empty()
//            );
//        }

//        internal Expression Transform(IfNode item) => Transform(item, false);
//        internal Expression Transform(IfNode item, bool forceRender)
//        {
//            if(item.Conditions.Length == 1)
//            {
//                var onlyCondition = item.Conditions[0].Expression.Transform(this, forceRender);
//                var body = Expression.Block(item.Conditions[0].Children.Select(child => child.Transform(this, forceRender)));
//                return Expression.IfThen(onlyCondition, body);
//            }
//            var conditions = item.Conditions;


//            var condition = conditions[conditions.Length - 1].Expression.Transform(this, forceRender);
//            var block = Expression.Block(conditions[conditions.Length - 1].Children.Select(child => child.Transform(this, forceRender)));

//            var ifBlock = Expression.IfThen(condition, block);


//            for(int i = conditions.Length - 2; i >= 0; --i)
//            {
//                condition = conditions[i].Expression.Transform(this, forceRender);
//                block = Expression.Block(conditions[i].Children.Select(child => child.Transform(this, forceRender)));
//                ifBlock = Expression.IfThenElse(condition, block, ifBlock);
//            }


//            if (forceRender)
//            {
//                return ifBlock;
//            }
//            return IfInDirectRenderMode(
//                ifBlock,
//                Expression.Empty()
//            );
//        }

//        internal Expression Transform(ConditionalNode item) => Transform(item, false);
//        internal Expression Transform(ConditionalNode item, bool forceRender)
//        {
//            throw new NotImplementedException();
//        }

//        internal Expression Transform(CommentNode item) => Transform(item, false);
//        internal Expression Transform(CommentNode item, bool forceRender)
//        {
//            throw new NotImplementedException();
//        }

//        internal Expression Transform(BlockNode item) => Transform(item, false);
//        internal Expression Transform(BlockNode item, bool forceRender)
//        {
//            throw new NotImplementedException();
//        }

//        internal Expression Transform(ExtendsNode item) => Transform(item, false);
//        internal Expression Transform(ExtendsNode item, bool forceRender)
//        {
//            throw new NotImplementedException();
//        }


//        private Expression IfInDirectRenderMode(Expression ifInDirectRenderMode, Expression ifNotInDirectRenderMode)
//        {
//            return Expression.IfThenElse(
//                Expression.Equal(SpecialVariables.Self.GetRenderMode(), Expression.Constant(RenderMode.Direct)),
//                ifInDirectRenderMode,
//                ifNotInDirectRenderMode
//            );
//        }

//        private IEnumerable<Expression> TransformAll(IEnumerable<ASTNode> nodes, bool forceRender)
//        {
//            return nodes.Select(node => node.Transform(this, forceRender));
//        }

//        internal Expression Transform(ForNode item) => Transform(item, false);
//        internal Expression Transform(ForNode item, bool forceRender)
//        {
//            if (ExpressionEx.ToArray(item.Expression.Transform(this, forceRender), out var expression, out var elementType) == false ||
//                expression == null || elementType == null)
//            {
//                throw new NotImplementedException();
//            }


//            PushScope("For Loop - Overall"); // Overall

//            // Define overall variables (array and loopIndex)
//            var breakLabel = Expression.Label("loopBreak");
//            var arrayVariable = CurrentScope.AddLocalVariable("array", expression, out var arrayAssignment);

//            PushScope("Setup and contents of While Loop");
//            var loopIndex = CurrentScope.AddLocalVariable("loopIndex", Expression.Constant(0), out var initLoopIndex); // TODO: Add a way to have an unnamed variable, so I don't conflict with the user's choice for var names
//            if (ExpressionParser.Reflection.Array.TryLength(arrayVariable, out var arrayLength) == false) throw new NotImplementedException();



//            PushScope("For Loop - Contents"); // Contents of loop
//            var itemVariable = CurrentScope.AddLocalVariable(item.VariableNames[0], Expression.ArrayIndex(arrayVariable, loopIndex), out var setItem); // "item"
//            //var loopVariable = CurrentScope.AddAndCreateLocalVariable_Generic("loop", out var setLoopVariable, elementType, arrayVariable, loopIndex);

//            Expression setLoopInfoVar;
//            if (elementType.IsValueType)
//            {
//                throw new NotImplementedException();
//            }
//            else
//            {
//                var loopInfoVar = CurrentScope.AddLocalVariable("loop",
//                    ExpressionEx.New_Generic(typeof(LoopInfoClass<>), new[] { elementType }, arrayVariable, loopIndex),
//                    out setLoopInfoVar);
//            }


//            var usersBlock = PopScope("For Loop - Contents",
//                setItem,
//                setLoopInfoVar,
//                //Expression.Break(breakLabel),
//                item.PrimaryBlock.Transform(this, forceRender)
//            );

//            var insideIf = Expression.Block(
//                usersBlock,
//                Expression.PreIncrementAssign(loopIndex)
//            );
//            var loopCondition = Expression.LessThan(loopIndex, arrayLength);
//            var ifElse = Expression.IfThenElse(loopCondition, insideIf, Expression.Break(breakLabel));

//            var loop = Expression.Loop(ifElse, breakLabel);

//            var whileLoop = PopScope("Setup and contents of While Loop",
//                initLoopIndex,
//                loop
//            );

//            var enumerateClause = Expression.AndAlso(
//                Expression.NotEqual(arrayVariable, Expression.Constant(null)),
//                Expression.GreaterThan(arrayLength, Expression.Constant(0))
//            );

//            Expression overall;
//            if (item.ElseBlock == null)
//            {
//                overall = Expression.IfThen(enumerateClause, whileLoop);
//            }
//            else
//            {
//                overall = Expression.IfThenElse(enumerateClause, whileLoop, item.ElseBlock.Transform(this, forceRender));
//            }

//            var forLoop = PopScope("For Loop - Overall", arrayAssignment, overall);


//            if (forceRender)
//            {
//                return forLoop;
//            }
//            return IfInDirectRenderMode(
//                forLoop,
//                Expression.Empty()
//            );
//        }

//        internal Expression Transform(TemplateNode item) => Transform(item, false);
//        internal Expression Transform(TemplateNode item, bool forceRender)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
