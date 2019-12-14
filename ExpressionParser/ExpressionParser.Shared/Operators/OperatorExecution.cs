using Common;
using ExpressionParser.Configuration;
using ExpressionParser.References;
using System;
using System.Collections.Concurrent;
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
        private static readonly Lazy<ConcurrentDictionary<(Type Type, OperatorType OperatorType), MethodInfo?>> _Operators = new Lazy<ConcurrentDictionary<(Type Type, OperatorType OperatorType), MethodInfo?>>();
        private static ConcurrentDictionary<(Type Type, OperatorType OperatorType), MethodInfo?> Operators => _Operators.Value;

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

        internal static object? EvaluateOperator(OperatorType operatorType, object? left, object? right)
        {
            if (TryExecuteBuiltInOperators(operatorType, left, right, out var result)) return result;

            if (left == null) throw new NotImplementedException(); // We properly handle null in "Equal" above....


            var tuple = (Type: left.GetType(), OperatorType: operatorType);
            if(Operators.TryGetValue(tuple, out var method) == false)
            {
                var operatorName = GetOperatorName(operatorType);
                method = tuple.Type.GetMethod(operatorName);
                Operators.TryAdd(tuple, method);
            }

            if (method == null) throw new NotImplementedException();

            return method.Invoke(left, new object?[] { right });
        }

        private static string GetOperatorName(OperatorType operatorType)
        {
            return operatorType switch
            {
                OperatorType.GreaterThan => "op_GreaterThan",
                _ => throw new NotImplementedException(),
            };

            /*
                op_Implicit
                op_Explicit
                op_Addition
                op_Subtraction
                op_Multiply
                op_Division
                op_Modulus
                op_ExclusiveOr
                op_BitwiseAnd
                op_BitwiseOr
                op_LogicalAnd
                op_LogicalOr
                op_Assign
                op_LeftShift
                op_RightShift
                op_SignedRightShift
                op_UnsignedRightShift
                op_Equality
                op_GreaterThan
                op_LessThan
                op_Inequality
                op_GreaterThanOrEqual
                op_LessThanOrEqual
                op_MultiplicationAssignment
                op_SubtractionAssignment
                op_ExclusiveOrAssignment
                op_LeftShiftAssignment
                op_ModulusAssignment
                op_AdditionAssignment
                op_BitwiseAndAssignment
                op_BitwiseOrAssignment
                op_Comma
                op_DivisionAssignment
                op_Decrement
                op_Increment
                op_UnaryNegation
                op_UnaryPlus
                op_OnesComplement
             */
        }

        internal static bool IsNot(ILanguageDefinition languageDefinition, object? left, object? right)
        {
            return !Is(languageDefinition, left, right);
        }

        private static bool TryExecuteBuiltInOperators(OperatorType operatorType, object? left, object? right, out object? result)
        {
            result = null;
            return left switch
            {
                int intLeft => TryExecuteBuiltInOperators(operatorType, intLeft, right, out result),
                _ => false,
            };
        }

        private static bool TryExecuteBuiltInOperators(OperatorType operatorType, int left, object? right, out object? result)
        {
            result = null;
            return right switch
            {
                int intRight => TryExecuteBuiltInOperators(operatorType, left, intRight, out result),
                _ => false,
            };
        }
        private static bool TryExecuteBuiltInOperators(OperatorType operatorType, int left, int right, out object? result)
        {
            result = operatorType switch
            {
                OperatorType.Subtract => left - right,
                OperatorType.GreaterThan => left > right,
                _ => throw new NotImplementedException(),
            };
            return result != null;
        }

    }
}
