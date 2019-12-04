using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Extensions.Common
{
    public static class NumericalTypeExtensions
    {

        public static Type GetTypeObject(this NumericalType numericalType)
        {
            return numericalType switch
            {
                NumericalType.SignedByte => typeof(sbyte),
                NumericalType.UnsignedByte => typeof(byte),
                NumericalType.SignedShort => typeof(short),
                NumericalType.UnsignedShort => typeof(ushort),
                NumericalType.SignedInt => typeof(int),
                NumericalType.UnsignedInt => typeof(uint),
                NumericalType.SignedLong => typeof(long),
                NumericalType.UnsignedLong => typeof(ulong),
                NumericalType.SinglePrecision => typeof(float),
                NumericalType.DoublePrecision => typeof(double),
                NumericalType.DecimalNumber => typeof(decimal),
                _ => throw new ArgumentOutOfRangeException(nameof(numericalType))
            };
        }
    }
}
