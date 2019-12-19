using System;
using System.Collections.Generic;
using System.Text;
using ExpressionParser.Configuration;
using ExpressionParser.Operators;
using ExpressionParser.Parsing;
using ExpressionParser.Scopes;
using ExpressionParser.Transforming.Nodes;
using System.Linq;
using Common;
using ExpressionParser.References;
using System.Dynamic;

namespace ExpressionParser.Transforming.Operators
{
    internal class DynamicOperatorTransformer<TScope, TRootScope> : IOperatorTransformVisitor<ASTNode, object?>
        where TScope : DynamicScope
        where TRootScope : TScope
    {
        internal DynamicOperatorTransformer(ScopeStack<TScope, TRootScope> scopeStack, ILanguageDefinition languageDefinition, DynamicTransformer<TScope, TRootScope> nodeTransformer)
        {
            LanguageDefinition = languageDefinition;
            NodeTransformer = nodeTransformer;
            ScopeStack = scopeStack;
        }

        internal ScopeStack<TScope, TRootScope> ScopeStack { get; }


        internal DynamicTransformer<TScope, TRootScope> NodeTransformer { get; }

        internal ILanguageDefinition LanguageDefinition { get; }


        public object? Transform(StandardOperator item, ASTNode[] args)
        {
            switch(item.OperatorType)
            {
                case OperatorType.Subtract:
                case OperatorType.GreaterThan:
                case OperatorType.IsNot:
                case OperatorType.Is:
                case OperatorType.Equal:
                case OperatorType.NotEqual:
                    return TransformBinary(item, args[0], args[1]);
                case OperatorType.LogicalNot:
                case OperatorType.Negate:
                    return TransformUnary(item, args[0]);
                case OperatorType.Assign:
                    return item.AssignmentOperatorBehavior switch
                    {
                        AssignmentOperatorBehavior.Assign => throw new NotImplementedException(),
                        AssignmentOperatorBehavior.NamedParameter => TransformNamedArgument(args[0], args[1]),
                        _ => throw new NotImplementedException(),
                    };
                default:
                    throw new NotImplementedException();
            }
        }

        private object? TransformBinary(StandardOperator item, ASTNode leftNode, ASTNode rightNode)
        {
            var left = leftNode.Transform(NodeTransformer);
            var right = rightNode.Transform(NodeTransformer);
            return item.OperatorType switch
            {
                OperatorType.Subtract => OperatorExecution.EvaluateOperator(item.OperatorType, left, right),
                OperatorType.GreaterThan => OperatorExecution.EvaluateOperator(item.OperatorType, left, right),
                OperatorType.Is => OperatorExecution.Is(LanguageDefinition, left, right),
                OperatorType.IsNot => OperatorExecution.IsNot(LanguageDefinition, left, right),
                OperatorType.Equal => OperatorExecution.Equal(left, right),
                _ => throw new NotImplementedException(),
            };
        }

        private (string name, object? value) TransformNamedArgument(ASTNode left, ASTNode right)
        {
            if (!(left is IdentifierNode identifierNode)) throw new NotImplementedException();
            var value = right.Transform(NodeTransformer);
            return (name: identifierNode.TextValue, value);
        }

        private object? TransformUnary(StandardOperator item, ASTNode rightNode)
        {
            var right = rightNode.Transform(NodeTransformer);

            switch (item.OperatorType)
            {
                case OperatorType.LogicalNot:
                    if (right == null) throw new NotImplementedException();
                    if(TypeCoercion.CanCast(right.GetType(), typeof(bool)))
                    {
                        return !((bool)right);
                    }
                    throw new NotImplementedException();
                case OperatorType.Negate:
                    return Negate(right);
                default:
                    throw new NotImplementedException();
            }
        }

        internal static object? Negate(object? right)
        {
            switch(right)
            {
                case long s64:
                    return -s64;
                case ulong u64:
                    if (u64 == 9_223_372_036_854_775_808)
                        return long.MinValue;
                    if (u64 > long.MaxValue)
                        throw new NotImplementedException();
                    return -((long)u64);
                case int s32:
                    return -s32;
                case uint u32:
                    return -u32;
                case short s16:
                    return -s16;
                case ushort u16:
                    return -u16;
                case sbyte s8:
                    return -s8;
                case byte u8:
                    return -u8;
                case float fl:
                    return -fl;
                case double dou:
                    return -dou;
                case decimal dec:
                    return -dec;
                case Numerical num:
                    return -num;
                default:
                    throw new NotImplementedException();
            }
        }

        public object? Transform(SpecialOperator item, ASTNode[] args)
        {
            var left = args[0].Transform(NodeTransformer);
            if (left == null) throw new ArgumentNullException();
            return item.OperatorType switch
            {
                SpecialOperatorType.MethodCall => Method(left, args[1]),
                SpecialOperatorType.PropertyAccess => Property(left, args[1]),
                SpecialOperatorType.Index => Index(left, args[1]),
                SpecialOperatorType.Pipeline => Pipeline(left, args[1]),
                _ => throw new NotImplementedException(),
            };
            object? Method(object? left, ASTNode right)
            {
                switch (right)
                {
                    case ArgumentSetNode argSet:
                        var args = argSet.Arguments.Select(arg => arg.Transform(NodeTransformer)).ToArray();
                        return DynamicResolver.CallMethod(LanguageDefinition, ScopeStack, left, args);
                    default:
                        throw new NotImplementedException();
                }
            }
            object? Pipeline(object? left, ASTNode right)
            {
                switch (right)
                {
                    case IdentifierNode identifierNode:
                        var test = identifierNode.TextValue;
                        var functions = LanguageDefinition.Functions.Where(func => 
                            func.Declaration.Name == identifierNode.TextValue || func.Declaration.Aliases.Contains(identifierNode.TextValue)
                        ).ToArray();
                        if (functions.Length != 1) throw new NotImplementedException();
                        return MethodGroup.Create(functions[0], left);
                    default:
                        throw new NotImplementedException();
                }
            }

            object? Index(object? left, ASTNode rightNode)
            {
                switch(rightNode)
                {
                    case ArgumentSetNode argSet:
                        var args = argSet.Arguments.Select(arg => arg.Transform(NodeTransformer)).ToArray();
                        if (DynamicResolver.TryIndex(left, args, out var result)) return result;
                        throw new NotImplementedException();
                    default:
                        throw new NotImplementedException();
                }
            }

            object? Property(object? left, ASTNode rightNode)
            {
                switch(rightNode)
                {
                    case IdentifierNode ident:
                        var allowedTypes = DynamicResolver.MemberTypes.Field | DynamicResolver.MemberTypes.Property;
                        if (LanguageDefinition.AllowStringIndexersAsProperties) allowedTypes |= DynamicResolver.MemberTypes.StringIndexer;
                        if (DynamicResolver.TryGetMember(left, ident.TextValue, allowedTypes, out var result)) return result;

                        if(left is DynamicObject dynamic)
                        {
                            if(DynamicEval.TryGetDynamicMember(dynamic, ident.TextValue, out result))
                            {
                                return result;
                            }
                        }

                        if (LanguageDefinition.ReturnNullOnNonExistantProperties) return null;
                        throw new NotImplementedException();
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}
