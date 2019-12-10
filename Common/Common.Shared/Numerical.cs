using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Common
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Numerical : 
        IEquatable<sbyte>, IEquatable<byte>, IEquatable<short>, IEquatable<ushort>, IEquatable<int>, IEquatable<uint>, IEquatable<long>, IEquatable<ulong>, IEquatable<float>, IEquatable<double>, IEquatable<decimal>, IEquatable<Numerical>,
        IComparable<sbyte>, IComparable<byte>, IComparable<short>, IComparable<ushort>, IComparable<int>, IComparable<uint>, IComparable<long>, IComparable<ulong>, IComparable<float>, IComparable<double>, IComparable<decimal>, IComparable<Numerical>
    {
        private static readonly Lazy<ConcurrentDictionary<NumericalType, MethodInfo?>> _TrueOperators = new Lazy<ConcurrentDictionary<NumericalType, MethodInfo?>>();
        private static ConcurrentDictionary<NumericalType, MethodInfo?> TrueOperators => _TrueOperators.Value;


        public static Numerical Unknown => new Numerical(NumericalType.Unknown);

        #region Constructors
        

        private Numerical(NumericalType type)
        {
            _Type = type;
            _Sbyte = default;
            _Byte = default;
            _Short = default;
            _Ushort = default;
            _Int = default;
            _Uint = default;
            _Long = default;
            _Ulong = default;
            _Float = default;
            _Double = default;
            _Decimal = default;
        }
        public static Numerical Copy(Numerical numerical)
        {
            var newObj = new Numerical(numerical.Type)
            {
                _Decimal = numerical._Decimal
            };
            return newObj;
        }

        #endregion Constructors

        #region Fields


        [FieldOffset(0)]
        private sbyte _Sbyte;
        [FieldOffset(0)]
        private byte _Byte;
        [FieldOffset(0)]
        private short _Short;
        [FieldOffset(0)]
        private ushort _Ushort;
        [FieldOffset(0)]
        private int _Int;
        [FieldOffset(0)]
        private uint _Uint;
        [FieldOffset(0)]
        private long _Long;
        [FieldOffset(0)]
        private ulong _Ulong;
        [FieldOffset(0)]
        private float _Float;
        [FieldOffset(0)]
        private double _Double;
        [FieldOffset(0)]
        private decimal _Decimal;

        [FieldOffset(16)]
        private readonly NumericalType _Type;
        public NumericalType Type => _Type;

        #endregion Fields

        public static bool TryCreate(object? item, [NotNullWhen(true)]out Numerical? numerical)
        {
            numerical = Unknown;
            if (item == null) return false;

            switch (item)
            {
                case Numerical num:
                    numerical = Copy(num);
                    break;
                case sbyte sb:
                    numerical = FromSByte(sb);
                    break;
                case byte b:
                    numerical = FromByte(b);
                    break;
                case short s:
                    numerical = FromInt16(s);
                    break;
                case ushort us:
                    numerical = FromUInt16(us);
                    break;
                case int i:
                    numerical = FromInt32(i);
                    break;
                case uint ui:
                    numerical = FromUInt32(ui);
                    break;
                case long l:
                    numerical = FromInt64(l);
                    break;
                case ulong ul:
                    numerical = FromUInt64(ul);
                    break;
                case float f:
                    numerical = FromSingle(f);
                    break;
                case double d:
                    numerical = FromDouble(d);
                    break;
                case decimal dec:
                    numerical = FromDecimal(dec);
                    break;
                case string str:
                    _ = TryParse(str, out numerical);
                    break;
                default:
                    throw new ArgumentException($"{item.GetType().Name} with value {item} cannot be converted to {nameof(Numerical)}", nameof(item));
            }
            return numerical != null;
        }

        public static bool TryCreate(object? item, NumberStyles style, IFormatProvider provider, [NotNullWhen(true)]out Numerical? numerical)
        {
            numerical = Unknown;
            if (item == null) return false;

            switch (item)
            {
                case Numerical num:
                    numerical = Copy(num);
                    break;
                case sbyte sb:
                    numerical = FromSByte(sb);
                    break;
                case byte b:
                    numerical = FromByte(b);
                    break;
                case short s:
                    numerical = FromInt16(s);
                    break;
                case ushort us:
                    numerical = FromUInt16(us);
                    break;
                case int i:
                    numerical = FromInt32(i);
                    break;
                case uint ui:
                    numerical = FromUInt32(ui);
                    break;
                case long l:
                    numerical = FromInt64(l);
                    break;
                case ulong ul:
                    numerical = FromUInt64(ul);
                    break;
                case float f:
                    numerical = FromSingle(f);
                    break;
                case double d:
                    numerical = FromDouble(d);
                    break;
                case decimal dec:
                    numerical = FromDecimal(dec);
                    break;
                case string str:
                    _ = TryParse(str, style, provider, out numerical);
                    break;
                default:
                    throw new ArgumentException($"{item.GetType().Name} with value {item} cannot be converted to {nameof(Numerical)}", nameof(item));
            }
            return numerical != Unknown;
        }

        public static bool TryParse(string str, NumberStyles style, IFormatProvider provider, [NotNullWhen(true)]out Numerical? numerical)
        {
            str = str ?? throw new ArgumentNullException(nameof(str));
            numerical = default;
            if (str.Contains("-"))
            {
                if (sbyte.TryParse(str, style, provider, out var sb))
                {
                    numerical = FromSByte(sb);
                    return true;
                }
                if (short.TryParse(str, style, provider, out var s))
                {
                    numerical = FromInt16(s);
                    return true;
                }
                if (int.TryParse(str, style, provider, out var i))
                {
                    numerical = FromInt32(i);
                    return true;
                }
                if (long.TryParse(str, style, provider, out var l))
                {
                    numerical = FromInt64(l);
                    return true;
                }
                if (float.TryParse(str, style, provider, out var fl))
                {
                    numerical = FromSingle(fl);
                    return true;
                }
                if (double.TryParse(str, style, provider, out var dou))
                {
                    numerical = FromDouble(dou);
                    return true;
                }
                if (decimal.TryParse(str, style, provider, out var dec))
                {
                    numerical = FromDecimal(dec);
                    return true;
                }
            }
            else
            {
                if (byte.TryParse(str, style, provider, out var b))
                {
                    numerical = FromByte(b);
                    return true;
                }
                if (ushort.TryParse(str, style, provider, out var us))
                {
                    numerical = FromUInt16(us);
                    return true;
                }
                if (uint.TryParse(str, style, provider, out var ui))
                {
                    numerical = FromUInt32(ui);
                    return true;
                }
                if (ulong.TryParse(str, style, provider, out var ul))
                {
                    numerical = FromUInt64(ul);
                    return true;
                }
                if (float.TryParse(str, style, provider, out var fl))
                {
                    numerical = FromSingle(fl);
                    return true;
                }
                if (double.TryParse(str, style, provider, out var dou))
                {
                    numerical = FromDouble(dou);
                    return true;
                }
                if (decimal.TryParse(str, style, provider, out var dec))
                {
                    numerical = FromDecimal(dec);
                    return true;
                }
            }
            return false;

        }
        public static bool TryParse(string str, [NotNullWhen(true)]out Numerical? numerical)
        {
            str = str ?? throw new ArgumentNullException(nameof(str));
            numerical = default;
            if (str.Contains("-"))
            {
                if (sbyte.TryParse(str, out var sb))
                {
                    numerical = FromSByte(sb);
                    return true;
                }
                if (short.TryParse(str, out var s))
                {
                    numerical = FromInt16(s);
                    return true;
                }
                if (int.TryParse(str, out var i))
                {
                    numerical = FromInt32(i);
                    return true;
                }
                if (long.TryParse(str, out var l))
                {
                    numerical = FromInt64(l);
                    return true;
                }
                if (float.TryParse(str, out var fl))
                {
                    numerical = FromSingle(fl);
                    return true;
                }
                if (double.TryParse(str, out var dou))
                {
                    numerical = FromDouble(dou);
                    return true;
                }
                if (decimal.TryParse(str, out var dec))
                {
                    numerical = FromDecimal(dec);
                    return true;
                }
            }
            else
            {
                if (byte.TryParse(str, out var b))
                {
                    numerical = FromByte(b);
                    return true;
                }
                if (ushort.TryParse(str, out var us))
                {
                    numerical = FromUInt16(us);
                    return true;
                }
                if (uint.TryParse(str, out var ui))
                {
                    numerical = FromUInt32(ui);
                    return true;
                }
                if (ulong.TryParse(str, out var ul))
                {
                    numerical = FromUInt64(ul);
                    return true;
                }
                if (float.TryParse(str, out var fl))
                {
                    numerical = FromSingle(fl);
                    return true;
                }
                if (double.TryParse(str, out var dou))
                {
                    numerical = FromDouble(dou);
                    return true;
                }
                if (decimal.TryParse(str, out var dec))
                {
                    numerical = FromDecimal(dec);
                    return true;
                }
            }
            return false;
        }

        #region Unary Operators
        public static Numerical operator +(Numerical obj1) => Plus(obj1);
        public static Numerical operator -(Numerical obj1) => Negate(obj1);
        public static Numerical operator ++(Numerical obj1) => Increment(obj1);
        public static Numerical operator --(Numerical obj1) => Decrement(obj1);
        public static bool operator true(Numerical obj1) => obj1.IsTrue;
        public static bool operator false(Numerical obj1) => !obj1.IsTrue;
        public static Numerical operator ~(Numerical obj1) => OnesComplement(obj1);


        public static Numerical Plus(Numerical item)
        {
            return item.Type switch
            {
                NumericalType.SignedByte => +item._Sbyte,
                NumericalType.UnsignedByte => +item._Byte,
                NumericalType.SignedShort => +item._Short,
                NumericalType.UnsignedShort => +item._Ushort,
                NumericalType.SignedInt => +item._Int,
                NumericalType.UnsignedInt => +item._Uint,
                NumericalType.SignedLong => +item._Long,
                NumericalType.UnsignedLong => +item._Ulong,
                NumericalType.SinglePrecision => +item._Float,
                NumericalType.DoublePrecision => +item._Double,
                NumericalType.DecimalNumber => +item._Decimal,
                _ => throw new InvalidOperationException(),
            };
        }
        public static Numerical Negate(Numerical item)
        {
            return item.Type switch
            {
                NumericalType.SignedByte => -item._Sbyte,
                NumericalType.UnsignedByte => -item._Byte,
                NumericalType.SignedShort => -item._Short,
                NumericalType.UnsignedShort => -item._Ushort,
                NumericalType.SignedInt => -item._Int,
                NumericalType.UnsignedInt => -item._Uint,
                NumericalType.SignedLong => -item._Long,
                NumericalType.UnsignedLong => TryNegateULong(),
                NumericalType.SinglePrecision => -item._Float,
                NumericalType.DoublePrecision => -item._Double,
                NumericalType.DecimalNumber => -item._Decimal,
                _ => throw new InvalidOperationException(),
            };

            Numerical TryNegateULong()
            {
                if (item._Ulong == 9_223_372_036_854_775_808)
                    return -9_223_372_036_854_775_808;
                if (item._Ulong < long.MaxValue)
                    return -(long)item._Ulong;
                throw new OverflowException();
            }
        }
        public static Numerical Increment(Numerical item)
        {
            return item.Type switch
            {
                NumericalType.SignedByte => ++item._Sbyte,
                NumericalType.UnsignedByte => ++item._Byte,
                NumericalType.SignedShort => ++item._Short,
                NumericalType.UnsignedShort => ++item._Ushort,
                NumericalType.SignedInt => ++item._Int,
                NumericalType.UnsignedInt => ++item._Uint,
                NumericalType.SignedLong => ++item._Long,
                NumericalType.UnsignedLong => ++item._Ulong,
                NumericalType.SinglePrecision => ++item._Float,
                NumericalType.DoublePrecision => ++item._Double,
                NumericalType.DecimalNumber => ++item._Decimal,
                _ => throw new InvalidOperationException(),
            };
        }
        public static Numerical Decrement(Numerical item)
        {
            return item.Type switch
            {
                NumericalType.SignedByte => --item._Sbyte,
                NumericalType.UnsignedByte => --item._Byte,
                NumericalType.SignedShort => --item._Short,
                NumericalType.UnsignedShort => --item._Ushort,
                NumericalType.SignedInt => --item._Int,
                NumericalType.UnsignedInt => --item._Uint,
                NumericalType.SignedLong => --item._Long,
                NumericalType.UnsignedLong => --item._Ulong,
                NumericalType.SinglePrecision => --item._Float,
                NumericalType.DoublePrecision => --item._Double,
                NumericalType.DecimalNumber => --item._Decimal,
                _ => throw new InvalidOperationException(),
            };
        }
        public bool IsTrue
        {
            get
            {
                if (TrueOperators.TryGetValue(Type, out var operatorInfo))
                {
                    if (operatorInfo == null) throw new InvalidOperationException();
                    return (bool)operatorInfo.Invoke(null, new object[] { this });
                }
                operatorInfo = Type.GetTypeObject().GetMethod("op_True");
                TrueOperators.TryAdd(Type, operatorInfo);
                if (operatorInfo == null) throw new InvalidOperationException();
                return (bool)operatorInfo.Invoke(null, new object[] { this });
            }
        }
        public static Numerical OnesComplement(Numerical item)
        {
            return item.Type switch
            {
                NumericalType.SignedByte => ~item._Sbyte,
                NumericalType.UnsignedByte => ~item._Byte,
                NumericalType.SignedShort => ~item._Short,
                NumericalType.UnsignedShort => ~item._Ushort,
                NumericalType.SignedInt => ~item._Int,
                NumericalType.UnsignedInt => ~item._Uint,
                NumericalType.SignedLong => ~item._Long,
                NumericalType.UnsignedLong => ~item._Ulong,
                NumericalType.SinglePrecision => throw new InvalidOperationException(),
                NumericalType.DoublePrecision => throw new InvalidOperationException(),
                NumericalType.DecimalNumber => throw new InvalidOperationException(),
                _ => throw new InvalidOperationException(),
            };
        }
        #endregion Unary Operators


        #region Arithmatic Operators

        public static Numerical operator +(Numerical obj1, Numerical obj2) => Numerical.Add(obj1, obj2);
        public static Numerical operator -(Numerical obj1, Numerical obj2) => Numerical.Subtract(obj1, obj2);
        public static Numerical operator *(Numerical obj1, Numerical obj2) => Numerical.Multiply(obj1, obj2);
        public static Numerical operator /(Numerical obj1, Numerical obj2) => Numerical.Divide(obj1, obj2);
        public static Numerical operator %(Numerical obj1, Numerical obj2) => Numerical.Mod(obj1, obj2);

        public static Numerical Add(Numerical left, Numerical right)
        {
            if (left.Type == right.Type)
            {
                return left.Type switch
                {
                    NumericalType.SignedByte => left._Sbyte + right._Sbyte,
                    NumericalType.UnsignedByte => left._Byte + right._Byte,
                    NumericalType.SignedShort => left._Short + right._Short,
                    NumericalType.UnsignedShort => left._Ushort + right._Ushort,
                    NumericalType.SignedInt => left._Int + right._Int,
                    NumericalType.UnsignedInt => left._Uint + right._Uint,
                    NumericalType.SignedLong => left._Long + right._Long,
                    NumericalType.UnsignedLong => left._Ulong + right._Ulong,
                    NumericalType.SinglePrecision => left._Float + right._Float,
                    NumericalType.DoublePrecision => left._Double + right._Double,
                    NumericalType.DecimalNumber => left._Decimal + right._Decimal,
                    _ => throw new InvalidOperationException(),
                };
            }
            if (left.Type.GetCombinedConversions().FastHasFlag(right.Type.GetSimpleConversions()))
            {
                // convert left to right, return right's type.
                return right.Type switch
                {
                    NumericalType.SignedByte => (sbyte)left + right._Sbyte,
                    NumericalType.UnsignedByte => (byte)left + right._Byte,
                    NumericalType.SignedShort => (short)left + right._Short,
                    NumericalType.UnsignedShort => (ushort)left + right._Ushort,
                    NumericalType.SignedInt => (int)left + right._Int,
                    NumericalType.UnsignedInt => (uint)left + right._Uint,
                    NumericalType.SignedLong => (long)left + right._Long,
                    NumericalType.UnsignedLong => (ulong)left + right._Ulong,
                    NumericalType.SinglePrecision => (float)left + right._Float,
                    NumericalType.DoublePrecision => (double)left + right._Double,
                    NumericalType.DecimalNumber => (decimal)left + right._Decimal,
                    _ => throw new InvalidOperationException(),
                };
            }
            if (right.Type.GetCombinedConversions().FastHasFlag(left.Type.GetSimpleConversions()))
            {
                // convert right to left, return left's type.
                return left.Type switch
                {
                    NumericalType.SignedByte => (sbyte)right + left._Sbyte,
                    NumericalType.UnsignedByte => (byte)right + left._Byte,
                    NumericalType.SignedShort => (short)right + left._Short,
                    NumericalType.UnsignedShort => (ushort)right + left._Ushort,
                    NumericalType.SignedInt => (int)right + left._Int,
                    NumericalType.UnsignedInt => (uint)right + left._Uint,
                    NumericalType.SignedLong => (long)right + left._Long,
                    NumericalType.UnsignedLong => (ulong)right + left._Ulong,
                    NumericalType.SinglePrecision => (float)right + left._Float,
                    NumericalType.DoublePrecision => (double)right + left._Double,
                    NumericalType.DecimalNumber => (decimal)right + left._Decimal,
                    _ => throw new InvalidOperationException(),
                };
            }
            throw new InvalidOperationException();
        }

        public static Numerical Subtract(Numerical left, Numerical right)
        {
            if (left.Type == right.Type)
            {
                return left.Type switch
                {
                    NumericalType.SignedByte => left._Sbyte - right._Sbyte,
                    NumericalType.UnsignedByte => left._Byte - right._Byte,
                    NumericalType.SignedShort => left._Short - right._Short,
                    NumericalType.UnsignedShort => left._Ushort - right._Ushort,
                    NumericalType.SignedInt => left._Int - right._Int,
                    NumericalType.UnsignedInt => left._Uint - right._Uint,
                    NumericalType.SignedLong => left._Long - right._Long,
                    NumericalType.UnsignedLong => left._Ulong - right._Ulong,
                    NumericalType.SinglePrecision => left._Float - right._Float,
                    NumericalType.DoublePrecision => left._Double - right._Double,
                    NumericalType.DecimalNumber => left._Decimal - right._Decimal,
                    _ => throw new InvalidOperationException(),
                };
            }
            if (left.Type.GetCombinedConversions().FastHasFlag(right.Type.GetSimpleConversions()))
            {
                // convert left to right, return right's type.
                return right.Type switch
                {
                    NumericalType.SignedByte => (sbyte)left - right._Sbyte,
                    NumericalType.UnsignedByte => (byte)left - right._Byte,
                    NumericalType.SignedShort => (short)left - right._Short,
                    NumericalType.UnsignedShort => (ushort)left - right._Ushort,
                    NumericalType.SignedInt => (int)left - right._Int,
                    NumericalType.UnsignedInt => (uint)left - right._Uint,
                    NumericalType.SignedLong => (long)left - right._Long,
                    NumericalType.UnsignedLong => (ulong)left - right._Ulong,
                    NumericalType.SinglePrecision => (float)left - right._Float,
                    NumericalType.DoublePrecision => (double)left - right._Double,
                    NumericalType.DecimalNumber => (decimal)left - right._Decimal,
                    _ => throw new InvalidOperationException(),
                };
            }
            if (right.Type.GetCombinedConversions().FastHasFlag(left.Type.GetSimpleConversions()))
            {
                // convert right to left, return left's type.
                return left.Type switch
                {
                    NumericalType.SignedByte => (sbyte)right - left._Sbyte,
                    NumericalType.UnsignedByte => (byte)right - left._Byte,
                    NumericalType.SignedShort => (short)right - left._Short,
                    NumericalType.UnsignedShort => (ushort)right - left._Ushort,
                    NumericalType.SignedInt => (int)right - left._Int,
                    NumericalType.UnsignedInt => (uint)right - left._Uint,
                    NumericalType.SignedLong => (long)right - left._Long,
                    NumericalType.UnsignedLong => (ulong)right - left._Ulong,
                    NumericalType.SinglePrecision => (float)right - left._Float,
                    NumericalType.DoublePrecision => (double)right - left._Double,
                    NumericalType.DecimalNumber => (decimal)right - left._Decimal,
                    _ => throw new InvalidOperationException(),
                };
            }
            throw new InvalidOperationException();
        }

        public static Numerical Multiply(Numerical left, Numerical right)
        {
            if (left.Type == right.Type)
            {
                return left.Type switch
                {
                    NumericalType.SignedByte => left._Sbyte * right._Sbyte,
                    NumericalType.UnsignedByte => left._Byte * right._Byte,
                    NumericalType.SignedShort => left._Short * right._Short,
                    NumericalType.UnsignedShort => left._Ushort * right._Ushort,
                    NumericalType.SignedInt => left._Int * right._Int,
                    NumericalType.UnsignedInt => left._Uint * right._Uint,
                    NumericalType.SignedLong => left._Long * right._Long,
                    NumericalType.UnsignedLong => left._Ulong * right._Ulong,
                    NumericalType.SinglePrecision => left._Float * right._Float,
                    NumericalType.DoublePrecision => left._Double * right._Double,
                    NumericalType.DecimalNumber => left._Decimal * right._Decimal,
                    _ => throw new InvalidOperationException(),
                };
            }
            if (left.Type.GetCombinedConversions().FastHasFlag(right.Type.GetSimpleConversions()))
            {
                // convert left to right, return right's type.
                return right.Type switch
                {
                    NumericalType.SignedByte => (sbyte)left * right._Sbyte,
                    NumericalType.UnsignedByte => (byte)left * right._Byte,
                    NumericalType.SignedShort => (short)left * right._Short,
                    NumericalType.UnsignedShort => (ushort)left * right._Ushort,
                    NumericalType.SignedInt => (int)left * right._Int,
                    NumericalType.UnsignedInt => (uint)left * right._Uint,
                    NumericalType.SignedLong => (long)left * right._Long,
                    NumericalType.UnsignedLong => (ulong)left * right._Ulong,
                    NumericalType.SinglePrecision => (float)left * right._Float,
                    NumericalType.DoublePrecision => (double)left * right._Double,
                    NumericalType.DecimalNumber => (decimal)left * right._Decimal,
                    _ => throw new InvalidOperationException(),
                };
            }
            if (right.Type.GetCombinedConversions().FastHasFlag(left.Type.GetSimpleConversions()))
            {
                // convert right to left, return left's type.
                return left.Type switch
                {
                    NumericalType.SignedByte => (sbyte)right * left._Sbyte,
                    NumericalType.UnsignedByte => (byte)right * left._Byte,
                    NumericalType.SignedShort => (short)right * left._Short,
                    NumericalType.UnsignedShort => (ushort)right * left._Ushort,
                    NumericalType.SignedInt => (int)right * left._Int,
                    NumericalType.UnsignedInt => (uint)right * left._Uint,
                    NumericalType.SignedLong => (long)right * left._Long,
                    NumericalType.UnsignedLong => (ulong)right * left._Ulong,
                    NumericalType.SinglePrecision => (float)right * left._Float,
                    NumericalType.DoublePrecision => (double)right * left._Double,
                    NumericalType.DecimalNumber => (decimal)right * left._Decimal,
                    _ => throw new InvalidOperationException(),
                };
            }
            throw new InvalidOperationException();
        }

        public static Numerical Divide(Numerical left, Numerical right)
        {
            if (left.Type == right.Type)
            {
                return left.Type switch
                {
                    NumericalType.SignedByte => left._Sbyte / right._Sbyte,
                    NumericalType.UnsignedByte => left._Byte / right._Byte,
                    NumericalType.SignedShort => left._Short / right._Short,
                    NumericalType.UnsignedShort => left._Ushort / right._Ushort,
                    NumericalType.SignedInt => left._Int / right._Int,
                    NumericalType.UnsignedInt => left._Uint / right._Uint,
                    NumericalType.SignedLong => left._Long / right._Long,
                    NumericalType.UnsignedLong => left._Ulong / right._Ulong,
                    NumericalType.SinglePrecision => left._Float / right._Float,
                    NumericalType.DoublePrecision => left._Double / right._Double,
                    NumericalType.DecimalNumber => left._Decimal / right._Decimal,
                    _ => throw new InvalidOperationException(),
                };
            }
            if (left.Type.GetCombinedConversions().FastHasFlag(right.Type.GetSimpleConversions()))
            {
                // convert left to right, return right's type.
                return right.Type switch
                {
                    NumericalType.SignedByte => (sbyte)left / right._Sbyte,
                    NumericalType.UnsignedByte => (byte)left / right._Byte,
                    NumericalType.SignedShort => (short)left / right._Short,
                    NumericalType.UnsignedShort => (ushort)left / right._Ushort,
                    NumericalType.SignedInt => (int)left / right._Int,
                    NumericalType.UnsignedInt => (uint)left / right._Uint,
                    NumericalType.SignedLong => (long)left / right._Long,
                    NumericalType.UnsignedLong => (ulong)left / right._Ulong,
                    NumericalType.SinglePrecision => (float)left / right._Float,
                    NumericalType.DoublePrecision => (double)left / right._Double,
                    NumericalType.DecimalNumber => (decimal)left / right._Decimal,
                    _ => throw new InvalidOperationException(),
                };
            }
            if (right.Type.GetCombinedConversions().FastHasFlag(left.Type.GetSimpleConversions()))
            {
                // convert right to left, return left's type.
                return left.Type switch
                {
                    NumericalType.SignedByte => (sbyte)right / left._Sbyte,
                    NumericalType.UnsignedByte => (byte)right / left._Byte,
                    NumericalType.SignedShort => (short)right / left._Short,
                    NumericalType.UnsignedShort => (ushort)right / left._Ushort,
                    NumericalType.SignedInt => (int)right / left._Int,
                    NumericalType.UnsignedInt => (uint)right / left._Uint,
                    NumericalType.SignedLong => (long)right / left._Long,
                    NumericalType.UnsignedLong => (ulong)right / left._Ulong,
                    NumericalType.SinglePrecision => (float)right / left._Float,
                    NumericalType.DoublePrecision => (double)right / left._Double,
                    NumericalType.DecimalNumber => (decimal)right / left._Decimal,
                    _ => throw new InvalidOperationException(),
                };
            }
            throw new InvalidOperationException();
        }

        public static Numerical Mod(Numerical left, Numerical right)
        {
            if (left.Type == right.Type)
            {
                return left.Type switch
                {
                    NumericalType.SignedByte => left._Sbyte % right._Sbyte,
                    NumericalType.UnsignedByte => left._Byte % right._Byte,
                    NumericalType.SignedShort => left._Short % right._Short,
                    NumericalType.UnsignedShort => left._Ushort % right._Ushort,
                    NumericalType.SignedInt => left._Int % right._Int,
                    NumericalType.UnsignedInt => left._Uint % right._Uint,
                    NumericalType.SignedLong => left._Long % right._Long,
                    NumericalType.UnsignedLong => left._Ulong % right._Ulong,
                    NumericalType.SinglePrecision => left._Float % right._Float,
                    NumericalType.DoublePrecision => left._Double % right._Double,
                    NumericalType.DecimalNumber => left._Decimal % right._Decimal,
                    _ => throw new InvalidOperationException(),
                };
            }
            if (left.Type.GetCombinedConversions().FastHasFlag(right.Type.GetSimpleConversions()))
            {
                // convert left to right, return right's type.
                return right.Type switch
                {
                    NumericalType.SignedByte => (sbyte)left % right._Sbyte,
                    NumericalType.UnsignedByte => (byte)left % right._Byte,
                    NumericalType.SignedShort => (short)left % right._Short,
                    NumericalType.UnsignedShort => (ushort)left % right._Ushort,
                    NumericalType.SignedInt => (int)left % right._Int,
                    NumericalType.UnsignedInt => (uint)left % right._Uint,
                    NumericalType.SignedLong => (long)left % right._Long,
                    NumericalType.UnsignedLong => (ulong)left % right._Ulong,
                    NumericalType.SinglePrecision => (float)left % right._Float,
                    NumericalType.DoublePrecision => (double)left % right._Double,
                    NumericalType.DecimalNumber => (decimal)left % right._Decimal,
                    _ => throw new InvalidOperationException(),
                };
            }
            if (right.Type.GetCombinedConversions().FastHasFlag(left.Type.GetSimpleConversions()))
            {
                // convert right to left, return left's type.
                return left.Type switch
                {
                    NumericalType.SignedByte => (sbyte)right % left._Sbyte,
                    NumericalType.UnsignedByte => (byte)right % left._Byte,
                    NumericalType.SignedShort => (short)right % left._Short,
                    NumericalType.UnsignedShort => (ushort)right % left._Ushort,
                    NumericalType.SignedInt => (int)right % left._Int,
                    NumericalType.UnsignedInt => (uint)right % left._Uint,
                    NumericalType.SignedLong => (long)right % left._Long,
                    NumericalType.UnsignedLong => (ulong)right % left._Ulong,
                    NumericalType.SinglePrecision => (float)right % left._Float,
                    NumericalType.DoublePrecision => (double)right % left._Double,
                    NumericalType.DecimalNumber => (decimal)right % left._Decimal,
                    _ => throw new InvalidOperationException(),
                };
            }
            throw new InvalidOperationException();
        }
        #endregion Arithmatic Operators

        #region Bitwise Operators

        public static Numerical operator &(Numerical obj1, Numerical obj2) => Numerical.BitwiseAnd(obj1, obj2);
        public static Numerical operator |(Numerical obj1, Numerical obj2) => Numerical.BitwiseOr(obj1, obj2);
        public static Numerical operator ^(Numerical obj1, Numerical obj2) => Numerical.Xor(obj1, obj2);


        public static Numerical BitwiseAnd(Numerical left, Numerical right)
        {
            if (left.Type == right.Type)
            {
                return left.Type switch
                {
                    NumericalType.SignedByte => left._Sbyte & right._Sbyte,
                    NumericalType.UnsignedByte => left._Byte & right._Byte,
                    NumericalType.SignedShort => left._Short & right._Short,
                    NumericalType.UnsignedShort => left._Ushort & right._Ushort,
                    NumericalType.SignedInt => left._Int & right._Int,
                    NumericalType.UnsignedInt => left._Uint & right._Uint,
                    NumericalType.SignedLong => left._Long & right._Long,
                    NumericalType.UnsignedLong => left._Ulong & right._Ulong,
                    NumericalType.SinglePrecision => throw new InvalidOperationException(),
                    NumericalType.DoublePrecision => throw new InvalidOperationException(),
                    NumericalType.DecimalNumber => throw new InvalidOperationException(),
                    _ => throw new InvalidOperationException(),
                };
            }
            if (left.Type.GetCombinedConversions().FastHasFlag(right.Type.GetSimpleConversions()))
            {
                // convert left to right, return right's type.
                return right.Type switch
                {
                    NumericalType.SignedByte => (sbyte)left & right._Sbyte,
                    NumericalType.UnsignedByte => (byte)left & right._Byte,
                    NumericalType.SignedShort => (short)left & right._Short,
                    NumericalType.UnsignedShort => (ushort)left & right._Ushort,
                    NumericalType.SignedInt => (int)left & right._Int,
                    NumericalType.UnsignedInt => (uint)left & right._Uint,
                    NumericalType.SignedLong => (long)left & right._Long,
                    NumericalType.UnsignedLong => (ulong)left & right._Ulong,
                    NumericalType.SinglePrecision => throw new InvalidOperationException(),
                    NumericalType.DoublePrecision => throw new InvalidOperationException(),
                    NumericalType.DecimalNumber => throw new InvalidOperationException(),
                    _ => throw new InvalidOperationException(),
                };
            }
            if (right.Type.GetCombinedConversions().FastHasFlag(left.Type.GetSimpleConversions()))
            {
                // convert right to left, return left's type.
                return left.Type switch
                {
                    NumericalType.SignedByte => (sbyte)right & left._Sbyte,
                    NumericalType.UnsignedByte => (byte)right & left._Byte,
                    NumericalType.SignedShort => (short)right & left._Short,
                    NumericalType.UnsignedShort => (ushort)right & left._Ushort,
                    NumericalType.SignedInt => (int)right & left._Int,
                    NumericalType.UnsignedInt => (uint)right & left._Uint,
                    NumericalType.SignedLong => (long)right & left._Long,
                    NumericalType.UnsignedLong => (ulong)right & left._Ulong,
                    NumericalType.SinglePrecision => throw new InvalidOperationException(),
                    NumericalType.DoublePrecision => throw new InvalidOperationException(),
                    NumericalType.DecimalNumber => throw new InvalidOperationException(),
                    _ => throw new InvalidOperationException(),
                };
            }
            throw new InvalidOperationException();
        }

        public static Numerical BitwiseOr(Numerical left, Numerical right)
        {
            if (left.Type == right.Type)
            {
                return left.Type switch
                {
                    NumericalType.SignedByte => left._Sbyte | right._Sbyte,
                    NumericalType.UnsignedByte => left._Byte | right._Byte,
                    NumericalType.SignedShort => left._Short | right._Short,
                    NumericalType.UnsignedShort => left._Ushort | right._Ushort,
                    NumericalType.SignedInt => left._Int | right._Int,
                    NumericalType.UnsignedInt => left._Uint | right._Uint,
                    NumericalType.SignedLong => left._Long | right._Long,
                    NumericalType.UnsignedLong => left._Ulong | right._Ulong,
                    NumericalType.SinglePrecision => throw new InvalidOperationException(),
                    NumericalType.DoublePrecision => throw new InvalidOperationException(),
                    NumericalType.DecimalNumber => throw new InvalidOperationException(),
                    _ => throw new InvalidOperationException(),
                };
            }
            if (left.Type.GetCombinedConversions().FastHasFlag(right.Type.GetSimpleConversions()))
            {
                // convert left to right, return right's type.
                return right.Type switch
                {
                    NumericalType.SignedByte => (sbyte)left | right._Sbyte,
                    NumericalType.UnsignedByte => (byte)left | right._Byte,
                    NumericalType.SignedShort => (short)left | right._Short,
                    NumericalType.UnsignedShort => (ushort)left | right._Ushort,
                    NumericalType.SignedInt => (int)left | right._Int,
                    NumericalType.UnsignedInt => (uint)left | right._Uint,
                    NumericalType.SignedLong => (long)left | right._Long,
                    NumericalType.UnsignedLong => (ulong)left | right._Ulong,
                    NumericalType.SinglePrecision => throw new InvalidOperationException(),
                    NumericalType.DoublePrecision => throw new InvalidOperationException(),
                    NumericalType.DecimalNumber => throw new InvalidOperationException(),
                    _ => throw new InvalidOperationException(),
                };
            }
            if (right.Type.GetCombinedConversions().FastHasFlag(left.Type.GetSimpleConversions()))
            {
                // convert right to left, return left's type.
                return left.Type switch
                {
                    NumericalType.SignedByte => (sbyte)right | left._Sbyte,
                    NumericalType.UnsignedByte => (byte)right | left._Byte,
                    NumericalType.SignedShort => (short)right | left._Short,
                    NumericalType.UnsignedShort => (ushort)right | left._Ushort,
                    NumericalType.SignedInt => (int)right | left._Int,
                    NumericalType.UnsignedInt => (uint)right | left._Uint,
                    NumericalType.SignedLong => (long)right | left._Long,
                    NumericalType.UnsignedLong => (ulong)right | left._Ulong,
                    NumericalType.SinglePrecision => throw new InvalidOperationException(),
                    NumericalType.DoublePrecision => throw new InvalidOperationException(),
                    NumericalType.DecimalNumber => throw new InvalidOperationException(),
                    _ => throw new InvalidOperationException(),
                };
            }
            throw new InvalidOperationException();
        }

        public static Numerical Xor(Numerical left, Numerical right)
        {
            if (left.Type == right.Type)
            {
                return left.Type switch
                {
                    NumericalType.SignedByte => left._Sbyte ^ right._Sbyte,
                    NumericalType.UnsignedByte => left._Byte ^ right._Byte,
                    NumericalType.SignedShort => left._Short ^ right._Short,
                    NumericalType.UnsignedShort => left._Ushort ^ right._Ushort,
                    NumericalType.SignedInt => left._Int ^ right._Int,
                    NumericalType.UnsignedInt => left._Uint ^ right._Uint,
                    NumericalType.SignedLong => left._Long ^ right._Long,
                    NumericalType.UnsignedLong => left._Ulong ^ right._Ulong,
                    NumericalType.SinglePrecision => throw new InvalidOperationException(),
                    NumericalType.DoublePrecision => throw new InvalidOperationException(),
                    NumericalType.DecimalNumber => throw new InvalidOperationException(),
                    _ => throw new InvalidOperationException(),
                };
            }
            if (left.Type.GetCombinedConversions().FastHasFlag(right.Type.GetSimpleConversions()))
            {
                // convert left to right, return right's type.
                return right.Type switch
                {
                    NumericalType.SignedByte => (sbyte)left ^ right._Sbyte,
                    NumericalType.UnsignedByte => (byte)left ^ right._Byte,
                    NumericalType.SignedShort => (short)left ^ right._Short,
                    NumericalType.UnsignedShort => (ushort)left ^ right._Ushort,
                    NumericalType.SignedInt => (int)left ^ right._Int,
                    NumericalType.UnsignedInt => (uint)left ^ right._Uint,
                    NumericalType.SignedLong => (long)left ^ right._Long,
                    NumericalType.UnsignedLong => (ulong)left ^ right._Ulong,
                    NumericalType.SinglePrecision => throw new InvalidOperationException(),
                    NumericalType.DoublePrecision => throw new InvalidOperationException(),
                    NumericalType.DecimalNumber => throw new InvalidOperationException(),
                    _ => throw new InvalidOperationException(),
                };
            }
            if (right.Type.GetCombinedConversions().FastHasFlag(left.Type.GetSimpleConversions()))
            {
                // convert right to left, return left's type.
                return left.Type switch
                {
                    NumericalType.SignedByte => (sbyte)right ^ left._Sbyte,
                    NumericalType.UnsignedByte => (byte)right ^ left._Byte,
                    NumericalType.SignedShort => (short)right ^ left._Short,
                    NumericalType.UnsignedShort => (ushort)right ^ left._Ushort,
                    NumericalType.SignedInt => (int)right ^ left._Int,
                    NumericalType.UnsignedInt => (uint)right ^ left._Uint,
                    NumericalType.SignedLong => (long)right ^ left._Long,
                    NumericalType.UnsignedLong => (ulong)right ^ left._Ulong,
                    NumericalType.SinglePrecision => throw new InvalidOperationException(),
                    NumericalType.DoublePrecision => throw new InvalidOperationException(),
                    NumericalType.DecimalNumber => throw new InvalidOperationException(),
                    _ => throw new InvalidOperationException(),
                };
            }
            throw new InvalidOperationException();
        }

        #endregion Bitwise Operators

        #region Shift Operators
        public static Numerical operator <<(Numerical obj1, int obj2) => Numerical.LeftShift(obj1, obj2);
        public static Numerical operator >>(Numerical obj1, int obj2) => Numerical.RightShift(obj1, obj2);

        public static Numerical LeftShift(Numerical left, int right)
        {
            return left.Type switch
            {
                NumericalType.SignedByte => left._Sbyte << right,
                NumericalType.UnsignedByte => left._Byte << right,
                NumericalType.SignedShort => left._Short << right,
                NumericalType.UnsignedShort => left._Ushort << right,
                NumericalType.SignedInt => left._Int << right,
                NumericalType.UnsignedInt => left._Uint << right,
                NumericalType.SignedLong => left._Long << right,
                NumericalType.UnsignedLong => left._Ulong << right,
                NumericalType.SinglePrecision => throw new InvalidOperationException(),
                NumericalType.DoublePrecision => throw new InvalidOperationException(),
                NumericalType.DecimalNumber => throw new InvalidOperationException(),
                _ => throw new InvalidOperationException(),
            };
        }

        public static Numerical RightShift(Numerical left, int right)
        {

            return left.Type switch
            {
                NumericalType.SignedByte => left._Sbyte >> right,
                NumericalType.UnsignedByte => left._Byte >> right,
                NumericalType.SignedShort => left._Short >> right,
                NumericalType.UnsignedShort => left._Ushort >> right,
                NumericalType.SignedInt => left._Int >> right,
                NumericalType.UnsignedInt => left._Uint >> right,
                NumericalType.SignedLong => left._Long >> right,
                NumericalType.UnsignedLong => left._Ulong >> right,
                NumericalType.SinglePrecision => throw new InvalidOperationException(),
                NumericalType.DoublePrecision => throw new InvalidOperationException(),
                NumericalType.DecimalNumber => throw new InvalidOperationException(),
                _ => throw new InvalidOperationException(),
            };
        }


        #endregion Shift Operators

        #region Comparison Operators

        public static bool operator ==(Numerical obj1, Numerical obj2) => obj1.Equals(obj2);
        public static bool operator !=(Numerical obj1, Numerical obj2) => !obj1.Equals(obj2);
        public static bool operator <=(Numerical left, Numerical right)
        {

            if (left.Type == right.Type)
            {
                return left.Type switch
                {
                    NumericalType.SignedByte => left._Sbyte <= right._Sbyte,
                    NumericalType.UnsignedByte => left._Byte <= right._Byte,
                    NumericalType.SignedShort => left._Short <= right._Short,
                    NumericalType.UnsignedShort => left._Ushort <= right._Ushort,
                    NumericalType.SignedInt => left._Int <= right._Int,
                    NumericalType.UnsignedInt => left._Uint <= right._Uint,
                    NumericalType.SignedLong => left._Long <= right._Long,
                    NumericalType.UnsignedLong => left._Ulong <= right._Ulong,
                    NumericalType.SinglePrecision => left._Float <= right._Float,
                    NumericalType.DoublePrecision => left._Double <= right._Double,
                    NumericalType.DecimalNumber => left._Decimal <= right._Decimal,
                    _ => throw new InvalidOperationException(),
                };
            }
            if (left.Type.GetCombinedConversions().FastHasFlag(right.Type.GetSimpleConversions()))
            {
                // convert left to right, return right's type.
                return right.Type switch
                {
                    NumericalType.SignedByte => (sbyte)left <= right._Sbyte,
                    NumericalType.UnsignedByte => (byte)left <= right._Byte,
                    NumericalType.SignedShort => (short)left <= right._Short,
                    NumericalType.UnsignedShort => (ushort)left <= right._Ushort,
                    NumericalType.SignedInt => (int)left <= right._Int,
                    NumericalType.UnsignedInt => (uint)left <= right._Uint,
                    NumericalType.SignedLong => (long)left <= right._Long,
                    NumericalType.UnsignedLong => (ulong)left <= right._Ulong,
                    NumericalType.SinglePrecision => (float)left <= right._Float,
                    NumericalType.DoublePrecision => (double)left <= right._Double,
                    NumericalType.DecimalNumber => (decimal)left <= right._Decimal,
                    _ => throw new InvalidOperationException(),
                };
            }
            if (right.Type.GetCombinedConversions().FastHasFlag(left.Type.GetSimpleConversions()))
            {
                // convert right to left, return left's type.
                return left.Type switch
                {
                    NumericalType.SignedByte => (sbyte)right <= left._Sbyte,
                    NumericalType.UnsignedByte => (byte)right <= left._Byte,
                    NumericalType.SignedShort => (short)right <= left._Short,
                    NumericalType.UnsignedShort => (ushort)right <= left._Ushort,
                    NumericalType.SignedInt => (int)right <= left._Int,
                    NumericalType.UnsignedInt => (uint)right <= left._Uint,
                    NumericalType.SignedLong => (long)right <= left._Long,
                    NumericalType.UnsignedLong => (ulong)right <= left._Ulong,
                    NumericalType.SinglePrecision => (float)right <= left._Float,
                    NumericalType.DoublePrecision => (double)right <= left._Double,
                    NumericalType.DecimalNumber => (decimal)right <= left._Decimal,
                    _ => throw new InvalidOperationException(),
                };
            }
            throw new InvalidOperationException();
        }
        public static bool operator >=(Numerical left, Numerical right)
        {
            if (left.Type == right.Type)
            {
                return left.Type switch
                {
                    NumericalType.SignedByte => left._Sbyte >= right._Sbyte,
                    NumericalType.UnsignedByte => left._Byte >= right._Byte,
                    NumericalType.SignedShort => left._Short >= right._Short,
                    NumericalType.UnsignedShort => left._Ushort >= right._Ushort,
                    NumericalType.SignedInt => left._Int >= right._Int,
                    NumericalType.UnsignedInt => left._Uint >= right._Uint,
                    NumericalType.SignedLong => left._Long >= right._Long,
                    NumericalType.UnsignedLong => left._Ulong >= right._Ulong,
                    NumericalType.SinglePrecision => left._Float >= right._Float,
                    NumericalType.DoublePrecision => left._Double >= right._Double,
                    NumericalType.DecimalNumber => left._Decimal >= right._Decimal,
                    _ => throw new InvalidOperationException(),
                };
            }
            if (left.Type.GetCombinedConversions().FastHasFlag(right.Type.GetSimpleConversions()))
            {
                // convert left to right, return right's type.
                return right.Type switch
                {
                    NumericalType.SignedByte => (sbyte)left >= right._Sbyte,
                    NumericalType.UnsignedByte => (byte)left >= right._Byte,
                    NumericalType.SignedShort => (short)left >= right._Short,
                    NumericalType.UnsignedShort => (ushort)left >= right._Ushort,
                    NumericalType.SignedInt => (int)left >= right._Int,
                    NumericalType.UnsignedInt => (uint)left >= right._Uint,
                    NumericalType.SignedLong => (long)left >= right._Long,
                    NumericalType.UnsignedLong => (ulong)left >= right._Ulong,
                    NumericalType.SinglePrecision => (float)left >= right._Float,
                    NumericalType.DoublePrecision => (double)left >= right._Double,
                    NumericalType.DecimalNumber => (decimal)left >= right._Decimal,
                    _ => throw new InvalidOperationException(),
                };
            }
            if (right.Type.GetCombinedConversions().FastHasFlag(left.Type.GetSimpleConversions()))
            {
                // convert right to left, return left's type.
                return left.Type switch
                {
                    NumericalType.SignedByte => (sbyte)right >= left._Sbyte,
                    NumericalType.UnsignedByte => (byte)right >= left._Byte,
                    NumericalType.SignedShort => (short)right >= left._Short,
                    NumericalType.UnsignedShort => (ushort)right >= left._Ushort,
                    NumericalType.SignedInt => (int)right >= left._Int,
                    NumericalType.UnsignedInt => (uint)right >= left._Uint,
                    NumericalType.SignedLong => (long)right >= left._Long,
                    NumericalType.UnsignedLong => (ulong)right >= left._Ulong,
                    NumericalType.SinglePrecision => (float)right >= left._Float,
                    NumericalType.DoublePrecision => (double)right >= left._Double,
                    NumericalType.DecimalNumber => (decimal)right >= left._Decimal,
                    _ => throw new InvalidOperationException(),
                };
            }
            throw new InvalidOperationException();
        }
        public static bool operator <(Numerical left, Numerical right)
        {
            if (left.Type == right.Type)
            {
                return left.Type switch
                {
                    NumericalType.SignedByte => left._Sbyte < right._Sbyte,
                    NumericalType.UnsignedByte => left._Byte < right._Byte,
                    NumericalType.SignedShort => left._Short < right._Short,
                    NumericalType.UnsignedShort => left._Ushort < right._Ushort,
                    NumericalType.SignedInt => left._Int < right._Int,
                    NumericalType.UnsignedInt => left._Uint < right._Uint,
                    NumericalType.SignedLong => left._Long < right._Long,
                    NumericalType.UnsignedLong => left._Ulong < right._Ulong,
                    NumericalType.SinglePrecision => left._Float < right._Float,
                    NumericalType.DoublePrecision => left._Double < right._Double,
                    NumericalType.DecimalNumber => left._Decimal < right._Decimal,
                    _ => throw new InvalidOperationException(),
                };
            }
            if (left.Type.GetCombinedConversions().FastHasFlag(right.Type.GetSimpleConversions()))
            {
                // convert left to right, return right's type.
                return right.Type switch
                {
                    NumericalType.SignedByte => (sbyte)left < right._Sbyte,
                    NumericalType.UnsignedByte => (byte)left < right._Byte,
                    NumericalType.SignedShort => (short)left < right._Short,
                    NumericalType.UnsignedShort => (ushort)left < right._Ushort,
                    NumericalType.SignedInt => (int)left < right._Int,
                    NumericalType.UnsignedInt => (uint)left < right._Uint,
                    NumericalType.SignedLong => (long)left < right._Long,
                    NumericalType.UnsignedLong => (ulong)left < right._Ulong,
                    NumericalType.SinglePrecision => (float)left < right._Float,
                    NumericalType.DoublePrecision => (double)left < right._Double,
                    NumericalType.DecimalNumber => (decimal)left < right._Decimal,
                    _ => throw new InvalidOperationException(),
                };
            }
            if (right.Type.GetCombinedConversions().FastHasFlag(left.Type.GetSimpleConversions()))
            {
                // convert right to left, return left's type.
                return left.Type switch
                {
                    NumericalType.SignedByte => (sbyte)right < left._Sbyte,
                    NumericalType.UnsignedByte => (byte)right < left._Byte,
                    NumericalType.SignedShort => (short)right < left._Short,
                    NumericalType.UnsignedShort => (ushort)right < left._Ushort,
                    NumericalType.SignedInt => (int)right < left._Int,
                    NumericalType.UnsignedInt => (uint)right < left._Uint,
                    NumericalType.SignedLong => (long)right < left._Long,
                    NumericalType.UnsignedLong => (ulong)right < left._Ulong,
                    NumericalType.SinglePrecision => (float)right < left._Float,
                    NumericalType.DoublePrecision => (double)right < left._Double,
                    NumericalType.DecimalNumber => (decimal)right < left._Decimal,
                    _ => throw new InvalidOperationException(),
                };
            }
            throw new InvalidOperationException();
        }
        public static bool operator >(Numerical left, Numerical right)
        {
            if (left.Type == right.Type)
            {
                return left.Type switch
                {
                    NumericalType.SignedByte => left._Sbyte > right._Sbyte,
                    NumericalType.UnsignedByte => left._Byte > right._Byte,
                    NumericalType.SignedShort => left._Short > right._Short,
                    NumericalType.UnsignedShort => left._Ushort > right._Ushort,
                    NumericalType.SignedInt => left._Int > right._Int,
                    NumericalType.UnsignedInt => left._Uint > right._Uint,
                    NumericalType.SignedLong => left._Long > right._Long,
                    NumericalType.UnsignedLong => left._Ulong > right._Ulong,
                    NumericalType.SinglePrecision => left._Float > right._Float,
                    NumericalType.DoublePrecision => left._Double > right._Double,
                    NumericalType.DecimalNumber => left._Decimal > right._Decimal,
                    _ => throw new InvalidOperationException(),
                };
            }
            if (left.Type.GetCombinedConversions().FastHasFlag(right.Type.GetSimpleConversions()))
            {
                // convert left to right, return right's type.
                return right.Type switch
                {
                    NumericalType.SignedByte => (sbyte)left > right._Sbyte,
                    NumericalType.UnsignedByte => (byte)left > right._Byte,
                    NumericalType.SignedShort => (short)left > right._Short,
                    NumericalType.UnsignedShort => (ushort)left > right._Ushort,
                    NumericalType.SignedInt => (int)left > right._Int,
                    NumericalType.UnsignedInt => (uint)left > right._Uint,
                    NumericalType.SignedLong => (long)left > right._Long,
                    NumericalType.UnsignedLong => (ulong)left > right._Ulong,
                    NumericalType.SinglePrecision => (float)left > right._Float,
                    NumericalType.DoublePrecision => (double)left > right._Double,
                    NumericalType.DecimalNumber => (decimal)left > right._Decimal,
                    _ => throw new InvalidOperationException(),
                };
            }
            if (right.Type.GetCombinedConversions().FastHasFlag(left.Type.GetSimpleConversions()))
            {
                // convert right to left, return left's type.
                return left.Type switch
                {
                    NumericalType.SignedByte => (sbyte)right > left._Sbyte,
                    NumericalType.UnsignedByte => (byte)right > left._Byte,
                    NumericalType.SignedShort => (short)right > left._Short,
                    NumericalType.UnsignedShort => (ushort)right > left._Ushort,
                    NumericalType.SignedInt => (int)right > left._Int,
                    NumericalType.UnsignedInt => (uint)right > left._Uint,
                    NumericalType.SignedLong => (long)right > left._Long,
                    NumericalType.UnsignedLong => (ulong)right > left._Ulong,
                    NumericalType.SinglePrecision => (float)right > left._Float,
                    NumericalType.DoublePrecision => (double)right > left._Double,
                    NumericalType.DecimalNumber => (decimal)right > left._Decimal,
                    _ => throw new InvalidOperationException(),
                };
            }
            throw new InvalidOperationException();
        }
        #endregion Comparison Operators


        #region Conversions
        public static implicit operator sbyte(Numerical d) => d.ToSByte();
        public static implicit operator byte(Numerical d) => d.ToByte();
        public static implicit operator short(Numerical d) => d.ToInt16();
        public static implicit operator ushort(Numerical d) => d.ToUInt16();
        public static implicit operator int(Numerical d) => d.ToInt32();
        public static implicit operator uint(Numerical d) => d.ToUInt32();
        public static implicit operator long(Numerical d) => d.ToInt64();
        public static implicit operator ulong(Numerical d) => d.ToUInt64();
        public static implicit operator float(Numerical d) => d.ToSingle();
        public static implicit operator double(Numerical d) => d.ToDouble();
        public static implicit operator decimal(Numerical d) => d.ToDecimal();
        public static implicit operator Numerical(sbyte d) => FromSByte(d);
        public static implicit operator Numerical(byte d) => FromByte(d);
        public static implicit operator Numerical(short d) => FromInt16(d);
        public static implicit operator Numerical(ushort d) => FromUInt16(d);
        public static implicit operator Numerical(int d) => FromInt32(d);
        public static implicit operator Numerical(uint d) => FromUInt32(d);
        public static implicit operator Numerical(long d) => FromInt64(d);
        public static implicit operator Numerical(ulong d) => FromUInt64(d);
        public static implicit operator Numerical(float d) => FromSingle(d);
        public static implicit operator Numerical(double d) => FromDouble(d);
        public static implicit operator Numerical(decimal d) => FromDecimal(d);


        public static Numerical FromSByte(sbyte b) => new Numerical(NumericalType.SignedByte)
        {
            _Sbyte = b
        };

        public static Numerical FromByte(byte b) => new Numerical(NumericalType.UnsignedByte)
        {
            _Byte = b
        };

        public static Numerical FromInt16(short b) => new Numerical(NumericalType.SignedShort)
        {
            _Short = b
        };
        public static Numerical FromUInt16(ushort b) => new Numerical(NumericalType.UnsignedShort)
        {
            _Ushort = b
        };
        public static Numerical FromInt32(int b) => new Numerical(NumericalType.SignedInt)
        {
            _Int = b
        };
        public static Numerical FromUInt32(uint b) => new Numerical(NumericalType.UnsignedInt)
        {
            _Uint = b
        };
        public static Numerical FromInt64(long b) => new Numerical(NumericalType.SignedLong)
        {
            _Long = b
        };
        public static Numerical FromUInt64(ulong b) => new Numerical(NumericalType.UnsignedLong)
        {
            _Ulong = b
        };
        public static Numerical FromSingle(float b) => new Numerical(NumericalType.SinglePrecision)
        {
            _Float = b
        };
        public static Numerical FromDouble(double b) => new Numerical(NumericalType.DoublePrecision)
        {
            _Double = b
        };
        public static Numerical FromDecimal(decimal b) => new Numerical(NumericalType.DecimalNumber)
        {
            _Decimal = b
        };

        public sbyte ToSByte()
        {
            return this.Type switch
            {
                NumericalType.SignedByte => this._Sbyte,
                NumericalType.UnsignedByte => throw new InvalidOperationException(),
                NumericalType.SignedShort => throw new InvalidOperationException(),
                NumericalType.UnsignedShort => throw new InvalidOperationException(),
                NumericalType.SignedInt => throw new InvalidOperationException(),
                NumericalType.UnsignedInt => throw new InvalidOperationException(),
                NumericalType.SignedLong => throw new InvalidOperationException(),
                NumericalType.UnsignedLong => throw new InvalidOperationException(),
                NumericalType.SinglePrecision => throw new InvalidOperationException(),
                NumericalType.DoublePrecision => throw new InvalidOperationException(),
                NumericalType.DecimalNumber => throw new InvalidOperationException(),
                _ => throw new InvalidOperationException(),
            };
        }

        public byte ToByte()
        {
            return this.Type switch
            {
                NumericalType.SignedByte => throw new InvalidOperationException(),
                NumericalType.UnsignedByte => this._Byte,
                NumericalType.SignedShort => throw new InvalidOperationException(),
                NumericalType.UnsignedShort => throw new InvalidOperationException(),
                NumericalType.SignedInt => throw new InvalidOperationException(),
                NumericalType.UnsignedInt => throw new InvalidOperationException(),
                NumericalType.SignedLong => throw new InvalidOperationException(),
                NumericalType.UnsignedLong => throw new InvalidOperationException(),
                NumericalType.SinglePrecision => throw new InvalidOperationException(),
                NumericalType.DoublePrecision => throw new InvalidOperationException(),
                NumericalType.DecimalNumber => throw new InvalidOperationException(),
                _ => throw new InvalidOperationException(),
            };
        }

        public short ToInt16()
        {
            return this.Type switch
            {
                NumericalType.SignedByte => this._Sbyte,
                NumericalType.UnsignedByte => this._Byte,
                NumericalType.SignedShort => this._Short,
                NumericalType.UnsignedShort => throw new InvalidOperationException(),
                NumericalType.SignedInt => throw new InvalidOperationException(),
                NumericalType.UnsignedInt => throw new InvalidOperationException(),
                NumericalType.SignedLong => throw new InvalidOperationException(),
                NumericalType.UnsignedLong => throw new InvalidOperationException(),
                NumericalType.SinglePrecision => throw new InvalidOperationException(),
                NumericalType.DoublePrecision => throw new InvalidOperationException(),
                NumericalType.DecimalNumber => throw new InvalidOperationException(),
                _ => throw new InvalidOperationException(),
            };
        }

        public ushort ToUInt16()
        {
            return this.Type switch
            {
                NumericalType.SignedByte => throw new InvalidOperationException(),
                NumericalType.UnsignedByte => this._Byte,
                NumericalType.SignedShort => throw new InvalidOperationException(),
                NumericalType.UnsignedShort => this._Ushort,
                NumericalType.SignedInt => throw new InvalidOperationException(),
                NumericalType.UnsignedInt => throw new InvalidOperationException(),
                NumericalType.SignedLong => throw new InvalidOperationException(),
                NumericalType.UnsignedLong => throw new InvalidOperationException(),
                NumericalType.SinglePrecision => throw new InvalidOperationException(),
                NumericalType.DoublePrecision => throw new InvalidOperationException(),
                NumericalType.DecimalNumber => throw new InvalidOperationException(),
                _ => throw new InvalidOperationException(),
            };
        }

        public int ToInt32()
        {
            return this.Type switch
            {
                NumericalType.SignedByte => this._Sbyte,
                NumericalType.UnsignedByte => this._Byte,
                NumericalType.SignedShort => this._Short,
                NumericalType.UnsignedShort => this._Ushort,
                NumericalType.SignedInt => this._Int,
                NumericalType.UnsignedInt => throw new InvalidOperationException(),
                NumericalType.SignedLong => throw new InvalidOperationException(),
                NumericalType.UnsignedLong => throw new InvalidOperationException(),
                NumericalType.SinglePrecision => throw new InvalidOperationException(),
                NumericalType.DoublePrecision => throw new InvalidOperationException(),
                NumericalType.DecimalNumber => throw new InvalidOperationException(),
                _ => throw new InvalidOperationException(),
            };
        }

        public uint ToUInt32()
        {
            return this.Type switch
            {
                NumericalType.SignedByte => throw new InvalidOperationException(),
                NumericalType.UnsignedByte => this._Byte,
                NumericalType.SignedShort => throw new InvalidOperationException(),
                NumericalType.UnsignedShort => this._Ushort,
                NumericalType.SignedInt => throw new InvalidOperationException(),
                NumericalType.UnsignedInt => this._Uint,
                NumericalType.SignedLong => throw new InvalidOperationException(),
                NumericalType.UnsignedLong => throw new InvalidOperationException(),
                NumericalType.SinglePrecision => throw new InvalidOperationException(),
                NumericalType.DoublePrecision => throw new InvalidOperationException(),
                NumericalType.DecimalNumber => throw new InvalidOperationException(),
                _ => throw new InvalidOperationException(),
            };
        }

        public long ToInt64()
        {
            return this.Type switch
            {
                NumericalType.SignedByte => this._Sbyte,
                NumericalType.UnsignedByte => this._Byte,
                NumericalType.SignedShort => this._Short,
                NumericalType.UnsignedShort => this._Ushort,
                NumericalType.SignedInt => this._Int,
                NumericalType.UnsignedInt => this._Uint,
                NumericalType.SignedLong => this._Long,
                NumericalType.UnsignedLong => throw new InvalidOperationException(),
                NumericalType.SinglePrecision => throw new InvalidOperationException(),
                NumericalType.DoublePrecision => throw new InvalidOperationException(),
                NumericalType.DecimalNumber => throw new InvalidOperationException(),
                _ => throw new InvalidOperationException(),
            };
        }

        public ulong ToUInt64()
        {
            return this.Type switch
            {
                NumericalType.SignedByte => throw new InvalidOperationException(),
                NumericalType.UnsignedByte => this._Byte,
                NumericalType.SignedShort => throw new InvalidOperationException(),
                NumericalType.UnsignedShort => this._Ushort,
                NumericalType.SignedInt => throw new InvalidOperationException(),
                NumericalType.UnsignedInt => this._Uint,
                NumericalType.SignedLong => throw new InvalidOperationException(),
                NumericalType.UnsignedLong => this._Ulong,
                NumericalType.SinglePrecision => throw new InvalidOperationException(),
                NumericalType.DoublePrecision => throw new InvalidOperationException(),
                NumericalType.DecimalNumber => throw new InvalidOperationException(),
                _ => throw new InvalidOperationException(),
            };
        }

        public float ToSingle()
        {
            return this.Type switch
            {
                NumericalType.SignedByte => this._Sbyte,
                NumericalType.UnsignedByte => this._Byte,
                NumericalType.SignedShort => this._Short,
                NumericalType.UnsignedShort => this._Ushort,
                NumericalType.SignedInt => this._Int,
                NumericalType.UnsignedInt => this._Uint,
                NumericalType.SignedLong => this._Long,
                NumericalType.UnsignedLong => this._Ulong,
                NumericalType.SinglePrecision => this._Float,
                NumericalType.DoublePrecision => throw new InvalidOperationException(),
                NumericalType.DecimalNumber => throw new InvalidOperationException(),
                _ => throw new InvalidOperationException(),
            };
        }

        public double ToDouble()
        {
            return this.Type switch
            {
                NumericalType.SignedByte => this._Sbyte,
                NumericalType.UnsignedByte => this._Byte,
                NumericalType.SignedShort => this._Short,
                NumericalType.UnsignedShort => this._Ushort,
                NumericalType.SignedInt => this._Int,
                NumericalType.UnsignedInt => this._Uint,
                NumericalType.SignedLong => this._Long,
                NumericalType.UnsignedLong => this._Ulong,
                NumericalType.SinglePrecision => this._Float,
                NumericalType.DoublePrecision => this._Double,
                NumericalType.DecimalNumber => throw new InvalidOperationException(),
                _ => throw new InvalidOperationException(),
            };
        }

        public decimal ToDecimal()
        {
            return this.Type switch
            {
                NumericalType.SignedByte => this._Sbyte,
                NumericalType.UnsignedByte => this._Byte,
                NumericalType.SignedShort => this._Short,
                NumericalType.UnsignedShort => this._Ushort,
                NumericalType.SignedInt => this._Int,
                NumericalType.UnsignedInt => this._Uint,
                NumericalType.SignedLong => this._Long,
                NumericalType.UnsignedLong => this._Ulong,
                NumericalType.SinglePrecision => throw new InvalidOperationException(),
                NumericalType.DoublePrecision => throw new InvalidOperationException(),
                NumericalType.DecimalNumber => this._Decimal,
                _ => throw new InvalidOperationException(),
            };
        }

        #endregion Conversions

        #region IEquatable
        public override bool Equals(object obj)
        {
            return obj switch
            {
                sbyte sb => this == sb,
                byte b => this == b,
                short s => this == s,
                ushort us => this == us,
                int i => this == i,
                uint ui => this == ui,
                long l => this == l,
                ulong ul => this == ul,
                float fl => this == fl,
                double dou => this == dou,
                decimal dec => this == dec,
                string str => Numerical.TryParse(str, out var numeric) && this == numeric,
                _ => false,
            };
        }

        public bool Equals(sbyte other) => Equals(FromSByte(other));

        public bool Equals(byte other) => Equals(FromByte(other));

        public bool Equals(short other) => Equals(FromInt16(other));

        public bool Equals(ushort other) => Equals(FromUInt16(other));

        public bool Equals(int other) => Equals(FromInt32(other));

        public bool Equals(uint other) => Equals(FromUInt32(other));

        public bool Equals(long other) => Equals(FromInt64(other));

        public bool Equals(ulong other) => Equals(FromUInt64(other));

        public bool Equals(float other) => Equals(FromSingle(other));

        public bool Equals(double other) => Equals(FromDouble(other));

        public bool Equals(decimal other) => Equals(FromDecimal(other));
        public bool Equals(Numerical other)
        {
            if (this.Type == other.Type)
            {
                return this.Type switch
                {
                    NumericalType.SignedByte => this._Sbyte == other._Sbyte,
                    NumericalType.UnsignedByte => this._Byte == other._Byte,
                    NumericalType.SignedShort => this._Short == other._Short,
                    NumericalType.UnsignedShort => this._Ushort == other._Ushort,
                    NumericalType.SignedInt => this._Int == other._Int,
                    NumericalType.UnsignedInt => this._Uint == other._Uint,
                    NumericalType.SignedLong => this._Long == other._Long,
                    NumericalType.UnsignedLong => this._Ulong == other._Ulong,
                    NumericalType.SinglePrecision => this._Float == other._Float,
                    NumericalType.DoublePrecision => this._Double == other._Double,
                    NumericalType.DecimalNumber => this._Decimal == other._Decimal,
                    _ => false
                };
            }
            if (this.Type.GetCombinedConversions().FastHasFlag(other.Type.GetSimpleConversions()))
            {
                // convert this to other, return other's type.
                return other.Type switch
                {
                    NumericalType.SignedByte => (sbyte)this == other._Sbyte,
                    NumericalType.UnsignedByte => (byte)this == other._Byte,
                    NumericalType.SignedShort => (short)this == other._Short,
                    NumericalType.UnsignedShort => (ushort)this == other._Ushort,
                    NumericalType.SignedInt => (int)this == other._Int,
                    NumericalType.UnsignedInt => (uint)this == other._Uint,
                    NumericalType.SignedLong => (long)this == other._Long,
                    NumericalType.UnsignedLong => (ulong)this == other._Ulong,
                    NumericalType.SinglePrecision => (float)this == other._Float,
                    NumericalType.DoublePrecision => (double)this == other._Double,
                    NumericalType.DecimalNumber => (decimal)this == other._Decimal,
                    _ => false
                };
            }
            if (other.Type.GetCombinedConversions().FastHasFlag(this.Type.GetSimpleConversions()))
            {
                // convert other to this, return this's type.
                return this.Type switch
                {
                    NumericalType.SignedByte => (sbyte)other == this._Sbyte,
                    NumericalType.UnsignedByte => (byte)other == this._Byte,
                    NumericalType.SignedShort => (short)other == this._Short,
                    NumericalType.UnsignedShort => (ushort)other == this._Ushort,
                    NumericalType.SignedInt => (int)other == this._Int,
                    NumericalType.UnsignedInt => (uint)other == this._Uint,
                    NumericalType.SignedLong => (long)other == this._Long,
                    NumericalType.UnsignedLong => (ulong)other == this._Ulong,
                    NumericalType.SinglePrecision => (float)other == this._Float,
                    NumericalType.DoublePrecision => (double)other == this._Double,
                    NumericalType.DecimalNumber => (decimal)other == this._Decimal,
                    _ => false
                };
            }
            return false;
        }


        #endregion IEquatable

        #region IComparable

        public int CompareTo(sbyte other) => CompareTo(FromSByte(other));

        public int CompareTo(byte other) => CompareTo(FromByte(other));

        public int CompareTo(short other) => CompareTo(FromInt16(other));

        public int CompareTo(ushort other) => CompareTo(FromUInt16(other));

        public int CompareTo(int other) => CompareTo(FromInt32(other));

        public int CompareTo(uint other) => CompareTo(FromUInt32(other));

        public int CompareTo(long other) => CompareTo(FromInt64(other));

        public int CompareTo(ulong other) => CompareTo(FromUInt64(other));

        public int CompareTo(float other) => CompareTo(FromSingle(other));

        public int CompareTo(double other) => CompareTo(FromDouble(other));

        public int CompareTo(decimal other) => CompareTo(FromDecimal(other));

        public int CompareTo(Numerical other)
        {
            if (this.Type == other.Type)
            {
                return this.Type switch
                {
                    NumericalType.SignedByte => this._Sbyte.CompareTo(other._Sbyte),
                    NumericalType.UnsignedByte => this._Byte.CompareTo(other._Byte),
                    NumericalType.SignedShort => this._Short.CompareTo(other._Short),
                    NumericalType.UnsignedShort => this._Ushort.CompareTo(other._Ushort),
                    NumericalType.SignedInt => this._Int.CompareTo(other._Int),
                    NumericalType.UnsignedInt => this._Uint.CompareTo(other._Uint),
                    NumericalType.SignedLong => this._Long.CompareTo(other._Long),
                    NumericalType.UnsignedLong => this._Ulong.CompareTo(other._Ulong),
                    NumericalType.SinglePrecision => this._Float.CompareTo(other._Float),
                    NumericalType.DoublePrecision => this._Double.CompareTo(other._Double),
                    NumericalType.DecimalNumber => this._Decimal.CompareTo(other._Decimal),
                    _ => throw new InvalidOperationException(),
                };
            }
            if (this.Type.GetCombinedConversions().FastHasFlag(other.Type.GetSimpleConversions()))
            {
                // convert this to other, return other's type.
                return other.Type switch
                {
                    NumericalType.SignedByte => ((sbyte)this).CompareTo(other._Sbyte),
                    NumericalType.UnsignedByte => ((byte)this).CompareTo(other._Byte),
                    NumericalType.SignedShort => ((short)this).CompareTo(other._Short),
                    NumericalType.UnsignedShort => ((ushort)this).CompareTo(other._Ushort),
                    NumericalType.SignedInt => ((int)this).CompareTo(other._Int),
                    NumericalType.UnsignedInt => ((uint)this).CompareTo(other._Uint),
                    NumericalType.SignedLong => ((long)this).CompareTo(other._Long),
                    NumericalType.UnsignedLong => ((ulong)this).CompareTo(other._Ulong),
                    NumericalType.SinglePrecision => ((float)this).CompareTo(other._Float),
                    NumericalType.DoublePrecision => ((double)this).CompareTo(other._Double),
                    NumericalType.DecimalNumber => ((decimal)this).CompareTo(other._Decimal),
                    _ => throw new InvalidOperationException(),
                };
            }
            if (other.Type.GetCombinedConversions().FastHasFlag(this.Type.GetSimpleConversions()))
            {
                // convert other to this, return this's type.
                return this.Type switch
                {
                    NumericalType.SignedByte => ((sbyte)other).CompareTo(this._Sbyte),
                    NumericalType.UnsignedByte => ((byte)other).CompareTo(this._Byte),
                    NumericalType.SignedShort => ((short)other).CompareTo(this._Short),
                    NumericalType.UnsignedShort => ((ushort)other).CompareTo(this._Ushort),
                    NumericalType.SignedInt => ((int)other).CompareTo(this._Int),
                    NumericalType.UnsignedInt => ((uint)other).CompareTo(this._Uint),
                    NumericalType.SignedLong => ((long)other).CompareTo(this._Long),
                    NumericalType.UnsignedLong => ((ulong)other).CompareTo(this._Ulong),
                    NumericalType.SinglePrecision => ((float)other).CompareTo(this._Float),
                    NumericalType.DoublePrecision => ((double)other).CompareTo(this._Double),
                    NumericalType.DecimalNumber => ((decimal)other).CompareTo(this._Decimal),
                    _ => throw new InvalidOperationException(),
                };
            }
            throw new InvalidOperationException();
        }
        #endregion IComparable


        public override int GetHashCode()
        {
            return Type switch
            {
                NumericalType.Unknown => _Sbyte.GetHashCode(),
                NumericalType.SignedByte => _Sbyte.GetHashCode(),
                NumericalType.UnsignedByte => _Byte.GetHashCode(),
                NumericalType.SignedShort => _Short.GetHashCode(),
                NumericalType.UnsignedShort => _Ushort.GetHashCode(),
                NumericalType.SignedInt => _Int.GetHashCode(),
                NumericalType.UnsignedInt => _Uint.GetHashCode(),
                NumericalType.SignedLong => _Long.GetHashCode(),
                NumericalType.UnsignedLong => _Ulong.GetHashCode(),
                NumericalType.SinglePrecision => _Float.GetHashCode(),
                NumericalType.DoublePrecision => _Double.GetHashCode(),
                NumericalType.DecimalNumber => _Decimal.GetHashCode(),
                _ => _Decimal.GetHashCode()
            };
        }


        internal Numerical Abs()
        {
            return Type switch
            {
                NumericalType.SignedByte => Math.Abs(_Sbyte),
                NumericalType.UnsignedByte => _Byte,
                NumericalType.SignedShort => Math.Abs(_Short),
                NumericalType.UnsignedShort => _Ushort,
                NumericalType.SignedInt => Math.Abs(_Int),
                NumericalType.UnsignedInt => _Uint,
                NumericalType.SignedLong => Math.Abs(_Long),
                NumericalType.UnsignedLong => _Ulong,
                NumericalType.SinglePrecision => Math.Abs(_Float),
                NumericalType.DoublePrecision => Math.Abs(_Double),
                NumericalType.DecimalNumber => Math.Abs(_Decimal),
                _ => throw new InvalidOperationException(),
            };
        }
        public string ToString(IFormatProvider provider)
        {
            return Type switch
            {
                NumericalType.SignedByte => _Sbyte.ToString(provider),
                NumericalType.UnsignedByte => _Byte.ToString(provider),
                NumericalType.SignedShort => _Short.ToString(provider),
                NumericalType.UnsignedShort => _Ushort.ToString(provider),
                NumericalType.SignedInt => _Int.ToString(provider),
                NumericalType.UnsignedInt => _Uint.ToString(provider),
                NumericalType.SignedLong => _Long.ToString(provider),
                NumericalType.UnsignedLong => _Ulong.ToString(provider),
                NumericalType.SinglePrecision => _Float.ToString(provider),
                NumericalType.DoublePrecision => _Double.ToString(provider),
                NumericalType.DecimalNumber => _Decimal.ToString(provider),
                _ => base.ToString()
            };
        }

        public override string ToString()
        {
            return Type switch
            {
                NumericalType.SignedByte => _Sbyte.ToString(CultureInfo.InvariantCulture),
                NumericalType.UnsignedByte => _Byte.ToString(CultureInfo.InvariantCulture),
                NumericalType.SignedShort => _Short.ToString(CultureInfo.InvariantCulture),
                NumericalType.UnsignedShort => _Ushort.ToString(CultureInfo.InvariantCulture),
                NumericalType.SignedInt => _Int.ToString(CultureInfo.InvariantCulture),
                NumericalType.UnsignedInt => _Uint.ToString(CultureInfo.InvariantCulture),
                NumericalType.SignedLong => _Long.ToString(CultureInfo.InvariantCulture),
                NumericalType.UnsignedLong => _Ulong.ToString(CultureInfo.InvariantCulture),
                NumericalType.SinglePrecision => _Float.ToString(CultureInfo.InvariantCulture),
                NumericalType.DoublePrecision => _Double.ToString(CultureInfo.InvariantCulture),
                NumericalType.DecimalNumber => _Decimal.ToString(CultureInfo.InvariantCulture),
                _ => base.ToString()
            };
        }




    }
}
