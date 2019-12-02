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

namespace ExpressionParser.Transforming.Operators
{
    public class DynamicOperatorTransformer<TScope, TRootScope> : IOperatorTransformVisitor<ASTNode, object?>
        where TScope : DynamicScope
        where TRootScope : TScope
    {
        public DynamicOperatorTransformer(ScopeStack<TScope, TRootScope> scopeStack, ILanguageDefinition languageDefinition, DynamicTransformer<TScope, TRootScope> nodeTransformer)
        {
            LanguageDefinition = languageDefinition;
            NodeTransformer = nodeTransformer;
            ScopeStack = scopeStack;
        }

        public ScopeStack<TScope, TRootScope> ScopeStack { get; }


        public DynamicTransformer<TScope, TRootScope> NodeTransformer { get; }

        public ILanguageDefinition LanguageDefinition { get; }


        public object? Transform(StandardOperator item, ASTNode[] args)
        {
            switch(item.OperatorType)
            {
                case OperatorType.LogicalNot:
                    return TransformUnary(item, args[0]);
                case OperatorType.Assign:
                    switch(item.AssignmentOperatorBehavior)
                    {
                        case AssignmentOperatorBehavior.Assign:
                            throw new NotImplementedException();
                        case AssignmentOperatorBehavior.NamedParameter:
                            return TransformNamedArgument(args[0], args[1]);
                        default:
                            throw new NotImplementedException();
                    }
                default:
                    throw new NotImplementedException();
            }
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
                default:
                    throw new NotImplementedException();
            }
        }

        public object? Transform(SpecialOperator item, ASTNode[] args)
        {
            var left = args[0].Transform(NodeTransformer);
            switch(item.OperatorType)
            {
                case SpecialOperatorType.MethodCall:
                    return Method(left, args[1]);
                case SpecialOperatorType.PropertyAccess:
                    return Property(left, args[1]);
                case SpecialOperatorType.Index:
                    return Index(left, args[1]);
                case SpecialOperatorType.Pipeline:
                    return Pipeline(left, args[1]);
                default:
                    throw new NotImplementedException();
            }

            object? Method(object? left, ASTNode right)
            {
                switch (right)
                {
                    case ArgumentSetNode argSet:
                        var args = argSet.Arguments.Select(arg => arg.Transform(NodeTransformer)).ToArray();
                        return DynamicResolver.CallMethod(ScopeStack, left, args);
                    default:
                        throw new NotImplementedException();
                }
            }
            object? Pipeline(object? left, ASTNode right)
            {
                switch (right)
                {
                    case IdentifierNode identifierNode:
                        var functions = LanguageDefinition.PipelineFunctions().Where(func => func.function.Name == identifierNode.TextValue).ToArray();
                        if (functions.Length != 1) throw new NotImplementedException();
                        return functions[0].overload?.Function?.Invoke(new object?[] { left });
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
                        throw new NotImplementedException();
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}
