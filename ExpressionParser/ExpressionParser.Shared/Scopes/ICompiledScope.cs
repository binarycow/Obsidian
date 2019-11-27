using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text;

namespace ExpressionParser.Scopes
{
    public interface ICompiledScope : IScope<ICompiledScope>
    {
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


    }
}
