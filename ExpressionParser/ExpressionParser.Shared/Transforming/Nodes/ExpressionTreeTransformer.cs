using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ExpressionParser.Configuration;
using ExpressionParser.Operators;
using ExpressionParser.Parsing;
using ExpressionParser.Scopes;
using ExpressionParser.Transforming.Operators;
using ExpressionParser.VariableManagement;

namespace ExpressionParser.Transforming.Nodes
{
    public class ExpressionTreeTransformer : INodeTransformVisitor<Expression>
    {
        public ExpressionTreeTransformer(ILanguageDefinition languageDefinition, Scope scope)
        {
            LanguageDefinition = languageDefinition;
            OperatorTransformer = new ExpressionTreeOperatorTransformer(this, languageDefinition);
            Scopes.Push(scope);
        }
        public IOperatorTransformVisitor<ASTNode, Expression> OperatorTransformer { get; }
        public ILanguageDefinition LanguageDefinition { get; }

        private Stack<Scope> Scopes { get; } = new Stack<Scope>();
        public Scope CurrentScope => Scopes.Peek();

        public Expression Transform(BinaryASTNode item)
        {
            return item.Operator.Transform(OperatorTransformer, new ASTNode[] { item.Left, item.Right });
        }

        public Expression Transform(UnaryASTNode item)
        {
            return item.Operator.Transform(OperatorTransformer, new ASTNode[] { item.Right });
        }

        public Expression Transform(LiteralNode item)
        {
            return Expression.Constant(item.Value);
        }

        public Expression Transform(IdentifierNode item)
        {
            var valueKeyword = LanguageDefinition.Keywords.OfType<ValueKeywordDefinition>().FirstOrDefault(keyword => keyword.Text == item.TextValue);
            if(valueKeyword != null)
            {
                return Expression.Constant(valueKeyword.Value);
            }
            if (CurrentScope.TryGetVariable(item.TextValue, out var expression))
            {
                return expression;
            }
            throw new NotImplementedException();
        }

        public Expression Transform(DictionaryItemNode item)
        {
            var items = new[]
            {
                item.Key.Transform(this),
                item.Value.Transform(this)
            };
            var types = items.Select(item => item.Type).ToArray();
            var genericType = typeof(KeyValuePair<,>).MakeGenericType(types);
            var constructor = genericType.GetConstructor(types);
            return Expression.New(constructor, items);
        }

        public Expression Transform(DictionaryNode item)
        {
            var items = item.Values.Select(item => Tuple.Create(item.Key.Transform(this), item.Value.Transform(this))).ToArray();
            if(items.All(item => item.Item1.Type == items[0].Item1.Type) == false || items.All(item => item.Item1.Type == items[0].Item1.Type) == false)
            {
                throw new NotImplementedException(); // They aren't all the same type...
            }
            var genericType = typeof(Dictionary<,>).MakeGenericType(items[0].Item1.Type, items[1].Item2.Type);
            var addMethod = genericType.GetMethod(nameof(Dictionary<int,int>.Add));
            var newExpression = Expression.New(genericType);
            var initExpressions = items.Select(item => Expression.ElementInit(addMethod, item.Item1, item.Item2));
            return Expression.ListInit(newExpression, initExpressions);
        }

        public Expression Transform(TupleNode item)
        {
            var tupleItems = item.TupleItems.Select(item => item.Transform(this)).ToArray();
            if(tupleItems.Length > 7)
            {
                throw new NotImplementedException();
            }
            var types = tupleItems.Select(item => item.Type).ToArray();
            var genericType = GetTupleType(tupleItems.Length).MakeGenericType(types);
            var constructor = genericType.GetConstructor(types);
            return Expression.New(constructor, tupleItems);

            Type GetTupleType(int count)
            {
                return count switch
                {
                    1 => typeof(Tuple<>),
                    2 => typeof(Tuple<,>),
                    3 => typeof(Tuple<,,>),
                    4 => typeof(Tuple<,,,>),
                    5 => typeof(Tuple<,,,,>),
                    6 => typeof(Tuple<,,,,,>),
                    7 => typeof(Tuple<,,,,,,>),
                    _ => throw new NotImplementedException(),
                };
            }
        }

        public Expression Transform(ListNode item)
        {
            var listItems = item.ListItems.Select(item => item.Transform(this)).ToArray();
            if(listItems.All(item => item.Type == listItems[0].Type) == false)
            {
                throw new NotImplementedException(); // They aren't all the same type...
            }
            var genericType = typeof(List<>).MakeGenericType(listItems[0].Type);
            var addMethod = genericType.GetMethod(nameof(List<int>.Add));
            var newExpression = Expression.New(genericType);
            var initExpressions = listItems.Select(item => Expression.ElementInit(addMethod, item));
            return Expression.ListInit(newExpression, initExpressions);
        }
    }
}
