using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    [Flags]
    public enum NumericalType
    {
        Unknown = 0,
        SignedByte,
        UnsignedByte,
        SignedShort,
        UnsignedShort,
        SignedInt,
        UnsignedInt,
        SignedLong,
        UnsignedLong,
        SinglePrecision,
        DoublePrecision,
        DecimalNumber,
    }

}
