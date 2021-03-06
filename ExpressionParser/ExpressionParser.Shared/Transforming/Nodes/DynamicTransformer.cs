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
    internal class DynamicTransformer<TScope, TRootScope> : INodeTransformVisitor<object?>
        where TScope : DynamicScope
        where TRootScope : TScope
    {
        internal DynamicTransformer(ScopeStack<TScope, TRootScope> scopeStack, ILanguageDefinition languageDefinition)
        {
            LanguageDefinition = languageDefinition;
            ScopeStack = scopeStack;
            OperatorTransformer = new DynamicOperatorTransformer<TScope, TRootScope>(scopeStack, languageDefinition, this);
        }

        internal ScopeStack<TScope, TRootScope> ScopeStack { get; }

        internal DynamicOperatorTransformer<TScope, TRootScope> OperatorTransformer { get; }

        internal ILanguageDefinition LanguageDefinition { get; }
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
            var valueKeyword = LanguageDefinition.Keywords.OfType<ValueKeywordDefinition>().FirstOrDefault(x => x.Names.Contains(item.TextValue));
            if (valueKeyword != default) return valueKeyword.Value;

            if (ScopeStack.Current.TryGetVariable(item.TextValue, out var value)) return value;
            var scopedFunction = LanguageDefinition.ScopedFunctions.FirstOrDefault(func => func.Declaration.Name == item.TextValue);
            if (scopedFunction != null) return MethodGroup.Create(scopedFunction);

            var function = LanguageDefinition.Functions.FirstOrDefault(func => func.Declaration.Name == item.TextValue);
            if (function != null) return MethodGroup.Create(function);

            var test = LanguageDefinition.Tests.FirstOrDefault(func => func.Declaration.Name == item.TextValue);
            if (test != null) return MethodGroup.Create(test);
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
            var tupleItems = item.TupleItems.Select(listItem => listItem.Transform(this));
            return Common.ReflectionHelpers.MakeGenericTuple(tupleItems);
        }

        public object? Transform(ListNode item)
        {
            var listItems = item.ListItems.Select(listItem => listItem.Transform(this));
            return Common.ReflectionHelpers.MakeGenericList(listItems);
        }

        public object? Transform(PipelineMethodGroup item)
        {
            return item.FunctionDefinition.Invoke(LanguageDefinition, new object?[] { item.ReferredObject });
        }

        public object? Transform(ArgumentSetNode item)
        {
            throw new NotImplementedException();
        }
    }
}
