using System;
using System.Collections.Generic;
using System.Text;
using ExpressionParser.Configuration;
using ExpressionParser.Operators;
using ExpressionParser.Parsing;
using ExpressionParser.Scopes;
using ExpressionParser.Transforming.Nodes;
using System.Linq;

namespace ExpressionParser.Transforming.Operators
{
    public class DynamicOperatorTransformer : IOperatorTransformVisitor<ASTNode, object?>
    {
        public DynamicOperatorTransformer(ILanguageDefinition languageDefinition, IDynamicScope scope, DynamicTransformer nodeTransformer)
        {
            LanguageDefinition = languageDefinition;
            Scope = scope;
            NodeTransformer = nodeTransformer;
        }

        public DynamicTransformer NodeTransformer { get; }

        public ILanguageDefinition LanguageDefinition { get; }
        public IDynamicScope Scope { get; }


        public object? Transform(StandardOperator item, ASTNode[] args)
        {
            throw new NotImplementedException();
        }

        public object? Transform(SpecialOperator item, ASTNode[] args)
        {
            var left = args[0].Transform(NodeTransformer);
            switch(item.OperatorType)
            {
                case SpecialOperatorType.MethodCall:
                    return Method(left, args[1]);
                default:
                    throw new NotImplementedException();
            }

            object? Method(object? left, ASTNode right)
            {
                switch(right)
                {
                    case ArgumentSetNode argSet:
                        var args = argSet.Arguments.Select(arg => arg.Transform(NodeTransformer)).ToArray();
                        return Resolver.CallMethod(left, args);
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}
