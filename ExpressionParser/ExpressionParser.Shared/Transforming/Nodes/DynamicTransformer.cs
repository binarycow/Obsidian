using System;
using System.Collections.Generic;
using System.Text;
using ExpressionParser.Configuration;
using ExpressionParser.Parsing;
using ExpressionParser.Scopes;
using ExpressionParser.Transforming.Operators;
using System.Linq;
using ExpressionParser.References;

namespace ExpressionParser.Transforming.Nodes
{
    public class DynamicTransformer<TScope, TRootScope> : INodeTransformVisitor<object?>
        where TScope : DynamicScope
        where TRootScope : TScope
    {
        public DynamicTransformer(ScopeStack<TScope, TRootScope> scopeStack, ILanguageDefinition languageDefinition)
        {
            LanguageDefinition = languageDefinition;
            ScopeStack = scopeStack;
            OperatorTransformer = new DynamicOperatorTransformer<TScope, TRootScope>(scopeStack, languageDefinition, this);
        }

        public ScopeStack<TScope, TRootScope> ScopeStack { get; }

        public DynamicOperatorTransformer<TScope, TRootScope> OperatorTransformer { get; }

        public ILanguageDefinition LanguageDefinition { get; }
        public object? Transform(BinaryASTNode item)
        {
            return item.Operator.Transform(OperatorTransformer, new[] { item.Left, item.Right });
        }

        public object? Transform(UnaryASTNode item)
        {
            return item.Operator.Transform(OperatorTransformer, new[] { item.Right });
        }

        public object? Transform(LiteralNode item)
        {
            return item.Value;
        }

        public object? Transform(IdentifierNode item)
        {
            var valueKeyword = LanguageDefinition.Keywords.OfType<ValueKeywordDefinition>().FirstOrDefault(x => x.Text == item.TextValue);
            if (valueKeyword != default) return valueKeyword.Value;

            if (ScopeStack.Current.TryGetVariable(item.TextValue, out var value)) return value;
            var function = LanguageDefinition.Functions.FirstOrDefault(func => func.Name == item.TextValue);
            if (function != null) return MethodGroup.Create(function);
            return null;
        }

        public object? Transform(DictionaryItemNode item)
        {
            throw new NotImplementedException();
        }

        public object? Transform(DictionaryNode item)
        {
            throw new NotImplementedException();
        }

        public object? Transform(TupleNode item)
        {
            throw new NotImplementedException();
        }

        public object? Transform(ListNode item)
        {
            throw new NotImplementedException();
        }
    }
}
