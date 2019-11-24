using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ExpressionParser.Configuration;
using ExpressionParser.Operators;
using ExpressionParser.Parsing;
using ExpressionParser.Transforming.Nodes;

namespace ExpressionParser.Transforming.Operators
{
    public class ExpressionTreeOperatorTransformer : IOperatorTransformVisitor<ASTNode, Expression>
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
            return item.OperatorType switch
            {
                OperatorType.Add => Expression.Add(args[0].Transform(NodeTransformVisitor), args[1].Transform(NodeTransformVisitor)),
                _ => throw new NotImplementedException(),
            };
        }

        public Expression Transform(SpecialOperator item, ASTNode[] args)
        {
            switch(item.OperatorType)
            {
                case SpecialOperatorType.MemberAccess:
                    var left = args[0].Transform(NodeTransformVisitor);

                    switch(item.SubType)
                    {
                        case SpecialOperatorSubType.Property:
                            if (Resolver.TryGetPropertyOrFieldInfo(left, args[1].TextValue, out var memberInfo))
                            {
                                return Expression.MakeMemberAccess(left, memberInfo);
                            }
                            if(LanguageDefinition.AllowStringIndexersAsProperties && Resolver.TryGetIndexer(left, out var propertyInfo, typeof(string)))
                            {
                                return Expression.MakeIndex(left, propertyInfo, Expression.Constant(args[1].TextValue).YieldOne());
                            }
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
