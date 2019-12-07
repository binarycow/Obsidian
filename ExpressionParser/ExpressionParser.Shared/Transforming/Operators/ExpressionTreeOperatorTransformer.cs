using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Common;
using ExpressionParser.Configuration;
using ExpressionParser.Operators;
using ExpressionParser.Parsing;
using ExpressionParser.References;
using ExpressionParser.Transforming.Nodes;

namespace ExpressionParser.Transforming.Operators
{
    internal class ExpressionTreeOperatorTransformer : IOperatorTransformVisitor<ASTNode, Expression>
    {
        public ExpressionTreeOperatorTransformer(INodeTransformVisitor<Expression> nodeTransformVisitor, ILanguageDefinition languageDefinition)
        {
            NodeTransformVisitor = nodeTransformVisitor;
            LanguageDefinition = languageDefinition;
        }

        public INodeTransformVisitor<Expression> NodeTransformVisitor { get; }
        public ILanguageDefinition LanguageDefinition { get; }

        public Expression Transform(StandardOperator item, ASTNode[] args)
        {
            var left = args[0].Transform(NodeTransformVisitor);
            return item.OperatorType switch
            {
                OperatorType.Add => Expression.Add(left, args[1].Transform(NodeTransformVisitor)),
                OperatorType.LogicalNot => Expression.Block(
#if DEBUG
                        ExpressionEx.Console.Write("NOT : LEFT : "),
                        ExpressionEx.Console.WriteLine(left),
#endif
                        Expression.IfThenElse(
                            Expression.Call(typeof(object), nameof(object.Equals), Type.EmptyTypes, Expression.Convert(left, typeof(object)), Expression.Constant(null)),
                            Expression.Constant("NULL!"),
                            Expression.Not(left)
                        )
                    ),
                _ => throw new NotImplementedException(),
            };
        }

        public Expression Transform(SpecialOperator item, ASTNode[] args)
        {
            var left = args[0].Transform(NodeTransformVisitor);
            switch (item.OperatorType)
            {
                case SpecialOperatorType.PropertyAccess:
                    if (ExpressionResolver.TryGetPropertyOrFieldInfo(left, args[1].TextValue, out var memberInfo))
                    {
                        return Expression.MakeMemberAccess(left, memberInfo);
                    }
                    if (LanguageDefinition.AllowStringIndexersAsProperties && ExpressionResolver.TryGetIndexer(left, out var propertyInfo, typeof(string)))
                    {
                        return Expression.MakeIndex(left, propertyInfo, Expression.Constant(args[1].TextValue).YieldOne());
                    }
                    throw new NotImplementedException();
                case SpecialOperatorType.MethodCall:
                    return Transform_MethodCall(left, args[1]);
                case SpecialOperatorType.Index:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }
        }

        private Expression Transform_MethodCall(Expression left, ASTNode right)
        {
            if (!(left is ConstantExpression constLeft)) throw new NotImplementedException();
            if (!(right is ArgumentSetNode argSet)) throw new NotImplementedException();

            var args = argSet.Arguments.Select(arg => arg.Transform(NodeTransformVisitor)).ToArray();
            return constLeft.Value switch
            {
                ExpressionMethodGroup expr => Transform_MethodCall_Expr(expr, args),
                FunctionMethodGroup func => throw new NotImplementedException(),
                //if (func.FunctionDefinition.OverloadDefinitions.Length != 1) throw new NotImplementedException();
                //return Transform_MethodCall_Func(func, args);
                _ => throw new NotImplementedException(),
            };
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        private Expression Transform_MethodCall_Func(FunctionMethodGroup left, Expression[] args)
        {
            throw new NotImplementedException();
            //return (left.FunctionDefinition.OverloadDefinitions[0]) switch
            //{
            //    SingleTypeOverloadDefinition single => Transform_MethodCall_Func_Single(single, args),
            //    MultiTypeOverloadDefinition multi => throw new NotImplementedException(),
            //    _ => throw new NotImplementedException(),
            //};
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        private Expression Transform_MethodCall_Func_Single(SingleTypeOverloadDefinition left, Expression[] args)
        {
            if (args.Length < left.MinimumArguments) throw new NotImplementedException();
            if (args.Length > left.MaximumArguments) throw new NotImplementedException();

            var typedArgsArray = new Expression[args.Length];
            for(var i = 0; i < typedArgsArray.Length; ++i)
            {
                if(TypeCoercion.CanCast(args[i].Type, left.ArgumentType) == false)
                {
                    throw new NotImplementedException();
                }
                typedArgsArray[i] = Expression.Convert(args[i], typeof(object));
            }

            var typedArgs = Expression.NewArrayInit(typeof(object), typedArgsArray);
            if(left.Function != default)
            {
                return Expression.Convert(
                    Expression.Invoke(
                        Expression.Constant(left.Function)
                        , typedArgs)
                    , left.ReturnType
                );
            }
            else if(left.Action != null)
            {
                return Expression.Block(
                    Expression.Invoke(
                        Expression.Constant(left.Action)
                        , typedArgs), 
                    Expression.Empty()
                );
            }
            throw new NotImplementedException();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        private Expression Transform_MethodCall_Expr(ExpressionMethodGroup left, Expression[] args)
        {
            throw new NotImplementedException();
        }
    }
}
