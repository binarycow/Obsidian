using Common;
using ExpressionParser.Configuration;
using ExpressionParser.References;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ExpressionParser.Operators
{
#pragma warning disable CA1812 // Avoid uninstantiated internal classes
    internal class OperatorExecution
#pragma warning restore CA1812 // Avoid uninstantiated internal classes
    {

        internal OperatorExecution()
        {

        }

        internal static bool Equal(object? left, object? right)
        {
            if (left == null && right == null) return true;
            if (left == null || right == null) return false;
            var leftType = left.GetType();
            var rightType = right.GetType();

            if (leftType == rightType) return left.Equals(right);
            if (Numerical.TryCreate(left, out var leftNumerical) && Numerical.TryCreate(right, out var rightNumerical)) return leftNumerical == rightNumerical;

            throw new NotImplementedException();
        }

        internal static bool Is(ILanguageDefinition languageDefinition, object? left, object? right)
        {
            switch(right)
            {
                case TestMethodGroup test:
                    return test.TestDefinition.Invoke(languageDefinition, new object?[] { left });
                default: 
                    throw new NotImplementedException();
            }
        }

        internal static object GreaterThan(object? left, object? right)
        {
            throw new NotImplementedException();
        }
    }
}
