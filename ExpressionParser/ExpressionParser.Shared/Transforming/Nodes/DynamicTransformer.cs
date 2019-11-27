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
    public class DynamicTransformer : INodeTransformVisitor<object?>
    {
        public DynamicTransformer(ILanguageDefinition languageDefinition, IDynamicScope scope)
        {
            LanguageDefinition = languageDefinition;
            Scope = scope;
            OperatorTransformer = new DynamicOperatorTransformer(languageDefinition, scope, this);
        }

        public DynamicOperatorTransformer OperatorTransformer { get; }

        public ILanguageDefinition LanguageDefinition { get; }
        public IDynamicScope Scope { get; }
        public object? Transform(BinaryASTNode item)
        {
            return item.Operator.Transform(OperatorTransformer, new[] { item.Left, item.Right });
        }

        public object? Transform(UnaryASTNode item)
        {
            throw new NotImplementedException();
        }

        public object? Transform(LiteralNode item)
        {
            return item.Value;
        }

        public object? Transform(IdentifierNode item)
        {
            if (Scope.TryGetVariable(item.TextValue, out var value)) return value;

            var function = LanguageDefinition.Functions.FirstOrDefault(func => func.Name == item.TextValue);
            if (function != null) return MethodGroup.Create(function);
            throw new NotImplementedException();
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
