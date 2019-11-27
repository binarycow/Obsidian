using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser.Scopes
{
    public interface IDynamicScope : IScope<IDynamicScope>
    {
        public void DefineAndSetVariable(string name, object? valueToSet);
        bool TryGetVariable(string textValue, out object? value);
    }
}
