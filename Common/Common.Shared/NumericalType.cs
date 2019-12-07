using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
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

    [Flags]
    public enum ImplicitNumericalTypeConversions
    {
        None                        = 0b0000_0000_0000,
        SimpleSignedByte            = 0b0000_0000_0001,
        SimpleUnsignedByte          = 0b0000_0000_0010,
        SimpleSignedShort           = 0b0000_0000_0100,
        SimpleUnsignedShort         = 0b0000_0000_1000,
        SimpleSignedInt             = 0b0000_0001_0000,
        SimpleUnsignedInt           = 0b0000_0010_0000,
        SimpleSignedLong            = 0b0000_0100_0000,
        SimpleUnsignedLong          = 0b0000_1000_0000,
        SimpleSinglePrecision       = 0b0001_0000_0000,
        SimpleDoublePrecision       = 0b0010_0000_0000,
        SimpleDecimalNumber         = 0b0100_0000_0000,
        CombinedSignedByte = SimpleSignedByte | SimpleSignedShort | SimpleSignedInt 
            | SimpleSignedLong | SimpleSinglePrecision | SimpleDoublePrecision 
            | SimpleDecimalNumber,
        CombinedUnsignedByte = SimpleUnsignedByte | SimpleSignedShort | SimpleUnsignedShort
            | SimpleSignedInt | SimpleUnsignedInt | SimpleSignedLong | SimpleUnsignedLong
            | SimpleSinglePrecision | SimpleDoublePrecision | SimpleDecimalNumber,


        CombinedSignedShort = SimpleSignedShort | SimpleSignedInt | SimpleSignedLong 
            | SimpleSinglePrecision | SimpleDoublePrecision | SimpleDecimalNumber,
        CombinedUnsignedShort = SimpleUnsignedShort | SimpleSignedInt | SimpleUnsignedInt
            | SimpleSignedLong | SimpleUnsignedLong | SimpleSinglePrecision
            | SimpleDoublePrecision | SimpleDecimalNumber,


        CombinedSignedInt = SimpleSignedInt | SimpleSignedLong | SimpleSinglePrecision | SimpleDoublePrecision | SimpleDecimalNumber,

        CombinedUnsignedInt = SimpleUnsignedInt | SimpleSignedLong | SimpleUnsignedLong
            | SimpleSinglePrecision | SimpleDoublePrecision | SimpleDecimalNumber,


        CombinedSignedLong = SimpleSignedLong | SimpleSinglePrecision 
            | SimpleDoublePrecision | SimpleDecimalNumber,

        CombinedUnsignedLong = SimpleUnsignedLong | SimpleSinglePrecision
            | SimpleDoublePrecision | SimpleDecimalNumber,

        CombinedSinglePrecision = SimpleSinglePrecision | SimpleDoublePrecision,

        CombinedDoublePrecision = SimpleDoublePrecision,

        CombinedDecimalNumber = SimpleDecimalNumber,
    }

}
