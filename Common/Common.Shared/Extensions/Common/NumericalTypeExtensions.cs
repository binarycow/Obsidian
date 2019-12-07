using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    internal static class NumericalTypeExtensions
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

        public static ImplicitNumericalTypeConversions GetCombinedConversions(this NumericalType numericalType)
        {
            return numericalType switch
            {
                NumericalType.SignedByte => ImplicitNumericalTypeConversions.CombinedSignedByte,
                NumericalType.UnsignedByte => ImplicitNumericalTypeConversions.CombinedUnsignedByte,
                NumericalType.SignedShort => ImplicitNumericalTypeConversions.CombinedSignedShort,
                NumericalType.UnsignedShort => ImplicitNumericalTypeConversions.CombinedUnsignedShort,
                NumericalType.SignedInt => ImplicitNumericalTypeConversions.CombinedSignedInt,
                NumericalType.UnsignedInt => ImplicitNumericalTypeConversions.CombinedUnsignedInt,
                NumericalType.SignedLong => ImplicitNumericalTypeConversions.CombinedSignedLong,
                NumericalType.UnsignedLong => ImplicitNumericalTypeConversions.CombinedUnsignedLong,
                NumericalType.SinglePrecision => ImplicitNumericalTypeConversions.CombinedSinglePrecision,
                NumericalType.DoublePrecision => ImplicitNumericalTypeConversions.CombinedDoublePrecision,
                NumericalType.DecimalNumber => ImplicitNumericalTypeConversions.CombinedDecimalNumber,
                _ => throw new ArgumentOutOfRangeException(nameof(numericalType))
            };
        }

        public static ImplicitNumericalTypeConversions GetSimpleConversions(this NumericalType numericalType)
        {
            return numericalType switch
            {
                NumericalType.SignedByte => ImplicitNumericalTypeConversions.SimpleSignedByte,
                NumericalType.UnsignedByte => ImplicitNumericalTypeConversions.SimpleUnsignedByte,
                NumericalType.SignedShort => ImplicitNumericalTypeConversions.SimpleSignedShort,
                NumericalType.UnsignedShort => ImplicitNumericalTypeConversions.SimpleUnsignedShort,
                NumericalType.SignedInt => ImplicitNumericalTypeConversions.SimpleSignedInt,
                NumericalType.UnsignedInt => ImplicitNumericalTypeConversions.SimpleUnsignedInt,
                NumericalType.SignedLong => ImplicitNumericalTypeConversions.SimpleSignedLong,
                NumericalType.UnsignedLong => ImplicitNumericalTypeConversions.SimpleUnsignedLong,
                NumericalType.SinglePrecision => ImplicitNumericalTypeConversions.SimpleSinglePrecision,
                NumericalType.DoublePrecision => ImplicitNumericalTypeConversions.SimpleDoublePrecision,
                NumericalType.DecimalNumber => ImplicitNumericalTypeConversions.SimpleDecimalNumber,
                _ => throw new ArgumentOutOfRangeException(nameof(numericalType))
            };
        }


        public static bool FastHasFlag(this ImplicitNumericalTypeConversions numericalType, ImplicitNumericalTypeConversions flag)
        {
            return (numericalType & flag) == flag;
        }
    }
}
