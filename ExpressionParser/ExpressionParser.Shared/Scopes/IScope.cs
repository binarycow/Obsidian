using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text;

namespace ExpressionParser.Scopes
{
    public interface IScope
    {
        public IScope? ParentScope { get; }
        public string? Name { get; }

        public bool IsRootScope { get; }

        public IScope? FindScope(string name);
        public IScope FindRootScope();
        public Expression ToDictionary();
        public IEnumerable<ParameterExpression> VariableWalk();
        public ParameterExpression this[string name] { get; }
        public ParameterExpression DefineVariable(string name, Type type);
        public void DefineVariable(ParameterExpression variable);
        public BinaryExpression DefineAndSetVariable(string name, Expression valueToSet);
        public ParameterExpression DefineAndSetVariable(string name, Expression valueToSet, out BinaryExpression assignmentExpression);
        public BinaryExpression DefineAndSetVariable(string name, object? valueToSet);
        public ParameterExpression DefineAndSetVariable(string name, object? valueToSet, out BinaryExpression assignmentExpression);
        public bool TryGetVariable(string name, [NotNullWhen(true)]out ParameterExpression? variable);

        public BlockExpression CloseScope(IEnumerable<Expression> body);
        public IEnumerable<ParameterExpression> Variables { get; }

        public IScope CreateChild(string name);
        public IScope CreateChild();
    }
}
