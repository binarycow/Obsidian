using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser.Configuration
{
    public enum OperatorType
    {
        Unknown = 0,
        Add,
        Subtract,
        Multiply,
        Divide,
        UnaryAdd,
        Range,
        Modulo,
        LeftShift,
        RightShift,
        LessThan,
        GreaterThan,
        LessThanOrEqual,
        GreaterThanOrEqual,
        Is,
        As,
        Equal,
        NotEqual,
        BitwiseAnd,
        ExclusiveOr,
        BitwiseOr,
        LogicalAnd,
        LogicalOr,
        NullCoalesce,
    }
}
