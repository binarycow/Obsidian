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
        private static Lazy<ConcurrentDictionary<NumericalType, MethodInfo?>> _TrueOperators = new Lazy<ConcurrentDictionary<NumericalType, MethodInfo?>>();
        private static ConcurrentDictionary<NumericalType, MethodInfo?> TrueOperators => _TrueOperators.Value;


        public static Numerical Unknown => new Numerical(NumericalType.Unknown);

        #region Constructors
        

        private Numerical(NumericalType type)
        {
            _Type = type;
            _sbyte = default;
            _byte = default;
            _short = default;
            _ushort = default;
            _int = default;
            _uint = default;
            _long = default;
            _ulong = default;
            _float = default;
            _double = default;
            _decimal = default;
        }
        public static Numerical Copy(Numerical numerical)
        {
            var newObj = new Numerical(numerical.Type);
            newObj._decimal = numerical._decimal;
            return newObj;
        }

        #endregion Constructors

        #region Fields


        [FieldOffset(0)]
        private sbyte _sbyte;
        [FieldOffset(0)]
        private byte _byte;
        [FieldOffset(0)]
        private short _short;
        [FieldOffset(0)]
        private ushort _ushort;
        [FieldOffset(0)]
        private int _int;
        [FieldOffset(0)]
        private uint _uint;
        [FieldOffset(0)]
        private long _long;
        [FieldOffset(0)]
        private ulong _ulong;
        [FieldOffset(0)]
        private float _float;
        [FieldOffset(0)]
        private double _double;
        [FieldOffset(0)]
        private decimal _decimal;

        [FieldOffset(16)]
        private NumericalType _Type;
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
                NumericalType.SignedByte => +item._sbyte,
                NumericalType.UnsignedByte => +item._byte,
                NumericalType.SignedShort => +item._short,
                NumericalType.UnsignedShort => +item._ushort,
                NumericalType.SignedInt => +item._int,
                NumericalType.UnsignedInt => +item._uint,
                NumericalType.SignedLong => +item._long,
                NumericalType.UnsignedLong => +item._ulong,
                NumericalType.SinglePrecision => +item._float,
                NumericalType.DoublePrecision => +item._double,
                NumericalType.DecimalNumber => +item._decimal,
                _ => throw new InvalidOperationException(),
            };
        }
        public static Numerical Negate(Numerical item)
        {
            return item.Type switch
            {
                NumericalType.SignedByte => -item._sbyte,
                NumericalType.UnsignedByte => -item._byte,
                NumericalType.SignedShort => -item._short,
                NumericalType.UnsignedShort => -item._ushort,
                NumericalType.SignedInt => -item._int,
                NumericalType.UnsignedInt => -item._uint,
                NumericalType.SignedLong => -item._long,
                NumericalType.UnsignedLong => TryNegateULong(),
                NumericalType.SinglePrecision => -item._float,
                NumericalType.DoublePrecision => -item._double,
                NumericalType.DecimalNumber => -item._decimal,
                _ => throw new InvalidOperationException(),
            };

            Numerical TryNegateULong()
            {
                if (item._ulong == 9_223_372_036_854_775_808)
                    return -9_223_372_036_854_775_808;
                if (item._ulong < long.MaxValue)
                    return -(long)item._ulong;
                throw new OverflowException();
            }
        }
        public static Numerical Increment(Numerical item)
        {
            return item.Type switch
            {
                NumericalType.SignedByte => ++item._sbyte,
                NumericalType.UnsignedByte => ++item._byte,
                NumericalType.SignedShort => ++item._short,
                NumericalType.UnsignedShort => ++item._ushort,
                NumericalType.SignedInt => ++item._int,
                NumericalType.UnsignedInt => ++item._uint,
                NumericalType.SignedLong => ++item._long,
                NumericalType.UnsignedLong => ++item._ulong,
                NumericalType.SinglePrecision => ++item._float,
                NumericalType.DoublePrecision => ++item._double,
                NumericalType.DecimalNumber => ++item._decimal,
                _ => throw new InvalidOperationException(),
            };
        }
        public static Numerical Decrement(Numerical item)
        {
            return item.Type switch
            {
                NumericalType.SignedByte => --item._sbyte,
                NumericalType.UnsignedByte => --item._byte,
                NumericalType.SignedShort => --item._short,
                NumericalType.UnsignedShort => --item._ushort,
                NumericalType.SignedInt => --item._int,
                NumericalType.UnsignedInt => --item._uint,
                NumericalType.SignedLong => --item._long,
                NumericalType.UnsignedLong => --item._ulong,
                NumericalType.SinglePrecision => --item._float,
                NumericalType.DoublePrecision => --item._double,
                NumericalType.DecimalNumber => --item._decimal,
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
                NumericalType.SignedByte => ~item._sbyte,
                NumericalType.UnsignedByte => ~item._byte,
                NumericalType.SignedShort => ~item._short,
                NumericalType.UnsignedShort => ~item._ushort,
                NumericalType.SignedInt => ~item._int,
                NumericalType.UnsignedInt => ~item._uint,
                NumericalType.SignedLong => ~item._long,
                NumericalType.UnsignedLong => ~item._ulong,
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
                    NumericalType.SignedByte => left._sbyte + right._sbyte,
                    NumericalType.UnsignedByte => left._byte + right._byte,
                    NumericalType.SignedShort => left._short + right._short,
                    NumericalType.UnsignedShort => left._ushort + right._ushort,
                    NumericalType.SignedInt => left._int + right._int,
                    NumericalType.UnsignedInt => left._uint + right._uint,
                    NumericalType.SignedLong => left._long + right._long,
                    NumericalType.UnsignedLong => left._ulong + right._ulong,
                    NumericalType.SinglePrecision => left._float + right._float,
                    NumericalType.DoublePrecision => left._double + right._double,
                    NumericalType.DecimalNumber => left._decimal + right._decimal,
                    _ => throw new InvalidOperationException(),
                };
            }
            if (left.Type.GetCombinedConversions().FastHasFlag(right.Type.GetSimpleConversions()))
            {
                // convert left to right, return right's type.
                return right.Type switch
                {
                    NumericalType.SignedByte => (sbyte)left + right._sbyte,
                    NumericalType.UnsignedByte => (byte)left + right._byte,
                    NumericalType.SignedShort => (short)left + right._short,
                    NumericalType.UnsignedShort => (ushort)left + right._ushort,
                    NumericalType.SignedInt => (int)left + right._int,
                    NumericalType.UnsignedInt => (uint)left + right._uint,
                    NumericalType.SignedLong => (long)left + right._long,
                    NumericalType.UnsignedLong => (ulong)left + right._ulong,
                    NumericalType.SinglePrecision => (float)left + right._float,
                    NumericalType.DoublePrecision => (double)left + right._double,
                    NumericalType.DecimalNumber => (decimal)left + right._decimal,
                    _ => throw new InvalidOperationException(),
                };
            }
            if (right.Type.GetCombinedConversions().FastHasFlag(left.Type.GetSimpleConversions()))
            {
                // convert right to left, return left's type.
                return left.Type switch
                {
                    NumericalType.SignedByte => (sbyte)right + left._sbyte,
                    NumericalType.UnsignedByte => (byte)right + left._byte,
                    NumericalType.SignedShort => (short)right + left._short,
                    NumericalType.UnsignedShort => (ushort)right + left._ushort,
                    NumericalType.SignedInt => (int)right + left._int,
                    NumericalType.UnsignedInt => (uint)right + left._uint,
                    NumericalType.SignedLong => (long)right + left._long,
                    NumericalType.UnsignedLong => (ulong)right + left._ulong,
                    NumericalType.SinglePrecision => (float)right + left._float,
                    NumericalType.DoublePrecision => (double)right + left._double,
                    NumericalType.DecimalNumber => (decimal)right + left._decimal,
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
                    NumericalType.SignedByte => left._sbyte - right._sbyte,
                    NumericalType.UnsignedByte => left._byte - right._byte,
                    NumericalType.SignedShort => left._short - right._short,
                    NumericalType.UnsignedShort => left._ushort - right._ushort,
                    NumericalType.SignedInt => left._int - right._int,
                    NumericalType.UnsignedInt => left._uint - right._uint,
                    NumericalType.SignedLong => left._long - right._long,
                    NumericalType.UnsignedLong => left._ulong - right._ulong,
                    NumericalType.SinglePrecision => left._float - right._float,
                    NumericalType.DoublePrecision => left._double - right._double,
                    NumericalType.DecimalNumber => left._decimal - right._decimal,
                    _ => throw new InvalidOperationException(),
                };
            }
            if (left.Type.GetCombinedConversions().FastHasFlag(right.Type.GetSimpleConversions()))
            {
                // convert left to right, return right's type.
                return right.Type switch
                {
                    NumericalType.SignedByte => (sbyte)left - right._sbyte,
                    NumericalType.UnsignedByte => (byte)left - right._byte,
                    NumericalType.SignedShort => (short)left - right._short,
                    NumericalType.UnsignedShort => (ushort)left - right._ushort,
                    NumericalType.SignedInt => (int)left - right._int,
                    NumericalType.UnsignedInt => (uint)left - right._uint,
                    NumericalType.SignedLong => (long)left - right._long,
                    NumericalType.UnsignedLong => (ulong)left - right._ulong,
                    NumericalType.SinglePrecision => (float)left - right._float,
                    NumericalType.DoublePrecision => (double)left - right._double,
                    NumericalType.DecimalNumber => (decimal)left - right._decimal,
                    _ => throw new InvalidOperationException(),
                };
            }
            if (right.Type.GetCombinedConversions().FastHasFlag(left.Type.GetSimpleConversions()))
            {
                // convert right to left, return left's type.
                return left.Type switch
                {
                    NumericalType.SignedByte => (sbyte)right - left._sbyte,
                    NumericalType.UnsignedByte => (byte)right - left._byte,
                    NumericalType.SignedShort => (short)right - left._short,
                    NumericalType.UnsignedShort => (ushort)right - left._ushort,
                    NumericalType.SignedInt => (int)right - left._int,
                    NumericalType.UnsignedInt => (uint)right - left._uint,
                    NumericalType.SignedLong => (long)right - left._long,
                    NumericalType.UnsignedLong => (ulong)right - left._ulong,
                    NumericalType.SinglePrecision => (float)right - left._float,
                    NumericalType.DoublePrecision => (double)right - left._double,
                    NumericalType.DecimalNumber => (decimal)right - left._decimal,
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
                    NumericalType.SignedByte => left._sbyte * right._sbyte,
                    NumericalType.UnsignedByte => left._byte * right._byte,
                    NumericalType.SignedShort => left._short * right._short,
                    NumericalType.UnsignedShort => left._ushort * right._ushort,
                    NumericalType.SignedInt => left._int * right._int,
                    NumericalType.UnsignedInt => left._uint * right._uint,
                    NumericalType.SignedLong => left._long * right._long,
                    NumericalType.UnsignedLong => left._ulong * right._ulong,
                    NumericalType.SinglePrecision => left._float * right._float,
                    NumericalType.DoublePrecision => left._double * right._double,
                    NumericalType.DecimalNumber => left._decimal * right._decimal,
                    _ => throw new InvalidOperationException(),
                };
            }
            if (left.Type.GetCombinedConversions().FastHasFlag(right.Type.GetSimpleConversions()))
            {
                // convert left to right, return right's type.
                return right.Type switch
                {
                    NumericalType.SignedByte => (sbyte)left * right._sbyte,
                    NumericalType.UnsignedByte => (byte)left * right._byte,
                    NumericalType.SignedShort => (short)left * right._short,
                    NumericalType.UnsignedShort => (ushort)left * right._ushort,
                    NumericalType.SignedInt => (int)left * right._int,
                    NumericalType.UnsignedInt => (uint)left * right._uint,
                    NumericalType.SignedLong => (long)left * right._long,
                    NumericalType.UnsignedLong => (ulong)left * right._ulong,
                    NumericalType.SinglePrecision => (float)left * right._float,
                    NumericalType.DoublePrecision => (double)left * right._double,
                    NumericalType.DecimalNumber => (decimal)left * right._decimal,
                    _ => throw new InvalidOperationException(),
                };
            }
            if (right.Type.GetCombinedConversions().FastHasFlag(left.Type.GetSimpleConversions()))
            {
                // convert right to left, return left's type.
                return left.Type switch
                {
                    NumericalType.SignedByte => (sbyte)right * left._sbyte,
                    NumericalType.UnsignedByte => (byte)right * left._byte,
                    NumericalType.SignedShort => (short)right * left._short,
                    NumericalType.UnsignedShort => (ushort)right * left._ushort,
                    NumericalType.SignedInt => (int)right * left._int,
                    NumericalType.UnsignedInt => (uint)right * left._uint,
                    NumericalType.SignedLong => (long)right * left._long,
                    NumericalType.UnsignedLong => (ulong)right * left._ulong,
                    NumericalType.SinglePrecision => (float)right * left._float,
                    NumericalType.DoublePrecision => (double)right * left._double,
                    NumericalType.DecimalNumber => (decimal)right * left._decimal,
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
                    NumericalType.SignedByte => left._sbyte / right._sbyte,
                    NumericalType.UnsignedByte => left._byte / right._byte,
                    NumericalType.SignedShort => left._short / right._short,
                    NumericalType.UnsignedShort => left._ushort / right._ushort,
                    NumericalType.SignedInt => left._int / right._int,
                    NumericalType.UnsignedInt => left._uint / right._uint,
                    NumericalType.SignedLong => left._long / right._long,
                    NumericalType.UnsignedLong => left._ulong / right._ulong,
                    NumericalType.SinglePrecision => left._float / right._float,
                    NumericalType.DoublePrecision => left._double / right._double,
                    NumericalType.DecimalNumber => left._decimal / right._decimal,
                    _ => throw new InvalidOperationException(),
                };
            }
            if (left.Type.GetCombinedConversions().FastHasFlag(right.Type.GetSimpleConversions()))
            {
                // convert left to right, return right's type.
                return right.Type switch
                {
                    NumericalType.SignedByte => (sbyte)left / right._sbyte,
                    NumericalType.UnsignedByte => (byte)left / right._byte,
                    NumericalType.SignedShort => (short)left / right._short,
                    NumericalType.UnsignedShort => (ushort)left / right._ushort,
                    NumericalType.SignedInt => (int)left / right._int,
                    NumericalType.UnsignedInt => (uint)left / right._uint,
                    NumericalType.SignedLong => (long)left / right._long,
                    NumericalType.UnsignedLong => (ulong)left / right._ulong,
                    NumericalType.SinglePrecision => (float)left / right._float,
                    NumericalType.DoublePrecision => (double)left / right._double,
                    NumericalType.DecimalNumber => (decimal)left / right._decimal,
                    _ => throw new InvalidOperationException(),
                };
            }
            if (right.Type.GetCombinedConversions().FastHasFlag(left.Type.GetSimpleConversions()))
            {
                // convert right to left, return left's type.
                return left.Type switch
                {
                    NumericalType.SignedByte => (sbyte)right / left._sbyte,
                    NumericalType.UnsignedByte => (byte)right / left._byte,
                    NumericalType.SignedShort => (short)right / left._short,
                    NumericalType.UnsignedShort => (ushort)right / left._ushort,
                    NumericalType.SignedInt => (int)right / left._int,
                    NumericalType.UnsignedInt => (uint)right / left._uint,
                    NumericalType.SignedLong => (long)right / left._long,
                    NumericalType.UnsignedLong => (ulong)right / left._ulong,
                    NumericalType.SinglePrecision => (float)right / left._float,
                    NumericalType.DoublePrecision => (double)right / left._double,
                    NumericalType.DecimalNumber => (decimal)right / left._decimal,
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
                    NumericalType.SignedByte => left._sbyte % right._sbyte,
                    NumericalType.UnsignedByte => left._byte % right._byte,
                    NumericalType.SignedShort => left._short % right._short,
                    NumericalType.UnsignedShort => left._ushort % right._ushort,
                    NumericalType.SignedInt => left._int % right._int,
                    NumericalType.UnsignedInt => left._uint % right._uint,
                    NumericalType.SignedLong => left._long % right._long,
                    NumericalType.UnsignedLong => left._ulong % right._ulong,
                    NumericalType.SinglePrecision => left._float % right._float,
                    NumericalType.DoublePrecision => left._double % right._double,
                    NumericalType.DecimalNumber => left._decimal % right._decimal,
                    _ => throw new InvalidOperationException(),
                };
            }
            if (left.Type.GetCombinedConversions().FastHasFlag(right.Type.GetSimpleConversions()))
            {
                // convert left to right, return right's type.
                return right.Type switch
                {
                    NumericalType.SignedByte => (sbyte)left % right._sbyte,
                    NumericalType.UnsignedByte => (byte)left % right._byte,
                    NumericalType.SignedShort => (short)left % right._short,
                    NumericalType.UnsignedShort => (ushort)left % right._ushort,
                    NumericalType.SignedInt => (int)left % right._int,
                    NumericalType.UnsignedInt => (uint)left % right._uint,
                    NumericalType.SignedLong => (long)left % right._long,
                    NumericalType.UnsignedLong => (ulong)left % right._ulong,
                    NumericalType.SinglePrecision => (float)left % right._float,
                    NumericalType.DoublePrecision => (double)left % right._double,
                    NumericalType.DecimalNumber => (decimal)left % right._decimal,
                    _ => throw new InvalidOperationException(),
                };
            }
            if (right.Type.GetCombinedConversions().FastHasFlag(left.Type.GetSimpleConversions()))
            {
                // convert right to left, return left's type.
                return left.Type switch
                {
                    NumericalType.SignedByte => (sbyte)right % left._sbyte,
                    NumericalType.UnsignedByte => (byte)right % left._byte,
                    NumericalType.SignedShort => (short)right % left._short,
                    NumericalType.UnsignedShort => (ushort)right % left._ushort,
                    NumericalType.SignedInt => (int)right % left._int,
                    NumericalType.UnsignedInt => (uint)right % left._uint,
                    NumericalType.SignedLong => (long)right % left._long,
                    NumericalType.UnsignedLong => (ulong)right % left._ulong,
                    NumericalType.SinglePrecision => (float)right % left._float,
                    NumericalType.DoublePrecision => (double)right % left._double,
                    NumericalType.DecimalNumber => (decimal)right % left._decimal,
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
                    NumericalType.SignedByte => left._sbyte & right._sbyte,
                    NumericalType.UnsignedByte => left._byte & right._byte,
                    NumericalType.SignedShort => left._short & right._short,
                    NumericalType.UnsignedShort => left._ushort & right._ushort,
                    NumericalType.SignedInt => left._int & right._int,
                    NumericalType.UnsignedInt => left._uint & right._uint,
                    NumericalType.SignedLong => left._long & right._long,
                    NumericalType.UnsignedLong => left._ulong & right._ulong,
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
                    NumericalType.SignedByte => (sbyte)left & right._sbyte,
                    NumericalType.UnsignedByte => (byte)left & right._byte,
                    NumericalType.SignedShort => (short)left & right._short,
                    NumericalType.UnsignedShort => (ushort)left & right._ushort,
                    NumericalType.SignedInt => (int)left & right._int,
                    NumericalType.UnsignedInt => (uint)left & right._uint,
                    NumericalType.SignedLong => (long)left & right._long,
                    NumericalType.UnsignedLong => (ulong)left & right._ulong,
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
                    NumericalType.SignedByte => (sbyte)right & left._sbyte,
                    NumericalType.UnsignedByte => (byte)right & left._byte,
                    NumericalType.SignedShort => (short)right & left._short,
                    NumericalType.UnsignedShort => (ushort)right & left._ushort,
                    NumericalType.SignedInt => (int)right & left._int,
                    NumericalType.UnsignedInt => (uint)right & left._uint,
                    NumericalType.SignedLong => (long)right & left._long,
                    NumericalType.UnsignedLong => (ulong)right & left._ulong,
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
                    NumericalType.SignedByte => left._sbyte | right._sbyte,
                    NumericalType.UnsignedByte => left._byte | right._byte,
                    NumericalType.SignedShort => left._short | right._short,
                    NumericalType.UnsignedShort => left._ushort | right._ushort,
                    NumericalType.SignedInt => left._int | right._int,
                    NumericalType.UnsignedInt => left._uint | right._uint,
                    NumericalType.SignedLong => left._long | right._long,
                    NumericalType.UnsignedLong => left._ulong | right._ulong,
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
                    NumericalType.SignedByte => (sbyte)left | right._sbyte,
                    NumericalType.UnsignedByte => (byte)left | right._byte,
                    NumericalType.SignedShort => (short)left | right._short,
                    NumericalType.UnsignedShort => (ushort)left | right._ushort,
                    NumericalType.SignedInt => (int)left | right._int,
                    NumericalType.UnsignedInt => (uint)left | right._uint,
                    NumericalType.SignedLong => (long)left | right._long,
                    NumericalType.UnsignedLong => (ulong)left | right._ulong,
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
                    NumericalType.SignedByte => (sbyte)right | left._sbyte,
                    NumericalType.UnsignedByte => (byte)right | left._byte,
                    NumericalType.SignedShort => (short)right | left._short,
                    NumericalType.UnsignedShort => (ushort)right | left._ushort,
                    NumericalType.SignedInt => (int)right | left._int,
                    NumericalType.UnsignedInt => (uint)right | left._uint,
                    NumericalType.SignedLong => (long)right | left._long,
                    NumericalType.UnsignedLong => (ulong)right | left._ulong,
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
                    NumericalType.SignedByte => left._sbyte ^ right._sbyte,
                    NumericalType.UnsignedByte => left._byte ^ right._byte,
                    NumericalType.SignedShort => left._short ^ right._short,
                    NumericalType.UnsignedShort => left._ushort ^ right._ushort,
                    NumericalType.SignedInt => left._int ^ right._int,
                    NumericalType.UnsignedInt => left._uint ^ right._uint,
                    NumericalType.SignedLong => left._long ^ right._long,
                    NumericalType.UnsignedLong => left._ulong ^ right._ulong,
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
                    NumericalType.SignedByte => (sbyte)left ^ right._sbyte,
                    NumericalType.UnsignedByte => (byte)left ^ right._byte,
                    NumericalType.SignedShort => (short)left ^ right._short,
                    NumericalType.UnsignedShort => (ushort)left ^ right._ushort,
                    NumericalType.SignedInt => (int)left ^ right._int,
                    NumericalType.UnsignedInt => (uint)left ^ right._uint,
                    NumericalType.SignedLong => (long)left ^ right._long,
                    NumericalType.UnsignedLong => (ulong)left ^ right._ulong,
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
                    NumericalType.SignedByte => (sbyte)right ^ left._sbyte,
                    NumericalType.UnsignedByte => (byte)right ^ left._byte,
                    NumericalType.SignedShort => (short)right ^ left._short,
                    NumericalType.UnsignedShort => (ushort)right ^ left._ushort,
                    NumericalType.SignedInt => (int)right ^ left._int,
                    NumericalType.UnsignedInt => (uint)right ^ left._uint,
                    NumericalType.SignedLong => (long)right ^ left._long,
                    NumericalType.UnsignedLong => (ulong)right ^ left._ulong,
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
                NumericalType.SignedByte => left._sbyte << right,
                NumericalType.UnsignedByte => left._byte << right,
                NumericalType.SignedShort => left._short << right,
                NumericalType.UnsignedShort => left._ushort << right,
                NumericalType.SignedInt => left._int << right,
                NumericalType.UnsignedInt => left._uint << right,
                NumericalType.SignedLong => left._long << right,
                NumericalType.UnsignedLong => left._ulong << right,
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
                NumericalType.SignedByte => left._sbyte >> right,
                NumericalType.UnsignedByte => left._byte >> right,
                NumericalType.SignedShort => left._short >> right,
                NumericalType.UnsignedShort => left._ushort >> right,
                NumericalType.SignedInt => left._int >> right,
                NumericalType.UnsignedInt => left._uint >> right,
                NumericalType.SignedLong => left._long >> right,
                NumericalType.UnsignedLong => left._ulong >> right,
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
                    NumericalType.SignedByte => left._sbyte <= right._sbyte,
                    NumericalType.UnsignedByte => left._byte <= right._byte,
                    NumericalType.SignedShort => left._short <= right._short,
                    NumericalType.UnsignedShort => left._ushort <= right._ushort,
                    NumericalType.SignedInt => left._int <= right._int,
                    NumericalType.UnsignedInt => left._uint <= right._uint,
                    NumericalType.SignedLong => left._long <= right._long,
                    NumericalType.UnsignedLong => left._ulong <= right._ulong,
                    NumericalType.SinglePrecision => left._float <= right._float,
                    NumericalType.DoublePrecision => left._double <= right._double,
                    NumericalType.DecimalNumber => left._decimal <= right._decimal,
                    _ => throw new InvalidOperationException(),
                };
            }
            if (left.Type.GetCombinedConversions().FastHasFlag(right.Type.GetSimpleConversions()))
            {
                // convert left to right, return right's type.
                return right.Type switch
                {
                    NumericalType.SignedByte => (sbyte)left <= right._sbyte,
                    NumericalType.UnsignedByte => (byte)left <= right._byte,
                    NumericalType.SignedShort => (short)left <= right._short,
                    NumericalType.UnsignedShort => (ushort)left <= right._ushort,
                    NumericalType.SignedInt => (int)left <= right._int,
                    NumericalType.UnsignedInt => (uint)left <= right._uint,
                    NumericalType.SignedLong => (long)left <= right._long,
                    NumericalType.UnsignedLong => (ulong)left <= right._ulong,
                    NumericalType.SinglePrecision => (float)left <= right._float,
                    NumericalType.DoublePrecision => (double)left <= right._double,
                    NumericalType.DecimalNumber => (decimal)left <= right._decimal,
                    _ => throw new InvalidOperationException(),
                };
            }
            if (right.Type.GetCombinedConversions().FastHasFlag(left.Type.GetSimpleConversions()))
            {
                // convert right to left, return left's type.
                return left.Type switch
                {
                    NumericalType.SignedByte => (sbyte)right <= left._sbyte,
                    NumericalType.UnsignedByte => (byte)right <= left._byte,
                    NumericalType.SignedShort => (short)right <= left._short,
                    NumericalType.UnsignedShort => (ushort)right <= left._ushort,
                    NumericalType.SignedInt => (int)right <= left._int,
                    NumericalType.UnsignedInt => (uint)right <= left._uint,
                    NumericalType.SignedLong => (long)right <= left._long,
                    NumericalType.UnsignedLong => (ulong)right <= left._ulong,
                    NumericalType.SinglePrecision => (float)right <= left._float,
                    NumericalType.DoublePrecision => (double)right <= left._double,
                    NumericalType.DecimalNumber => (decimal)right <= left._decimal,
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
                    NumericalType.SignedByte => left._sbyte >= right._sbyte,
                    NumericalType.UnsignedByte => left._byte >= right._byte,
                    NumericalType.SignedShort => left._short >= right._short,
                    NumericalType.UnsignedShort => left._ushort >= right._ushort,
                    NumericalType.SignedInt => left._int >= right._int,
                    NumericalType.UnsignedInt => left._uint >= right._uint,
                    NumericalType.SignedLong => left._long >= right._long,
                    NumericalType.UnsignedLong => left._ulong >= right._ulong,
                    NumericalType.SinglePrecision => left._float >= right._float,
                    NumericalType.DoublePrecision => left._double >= right._double,
                    NumericalType.DecimalNumber => left._decimal >= right._decimal,
                    _ => throw new InvalidOperationException(),
                };
            }
            if (left.Type.GetCombinedConversions().FastHasFlag(right.Type.GetSimpleConversions()))
            {
                // convert left to right, return right's type.
                return right.Type switch
                {
                    NumericalType.SignedByte => (sbyte)left >= right._sbyte,
                    NumericalType.UnsignedByte => (byte)left >= right._byte,
                    NumericalType.SignedShort => (short)left >= right._short,
                    NumericalType.UnsignedShort => (ushort)left >= right._ushort,
                    NumericalType.SignedInt => (int)left >= right._int,
                    NumericalType.UnsignedInt => (uint)left >= right._uint,
                    NumericalType.SignedLong => (long)left >= right._long,
                    NumericalType.UnsignedLong => (ulong)left >= right._ulong,
                    NumericalType.SinglePrecision => (float)left >= right._float,
                    NumericalType.DoublePrecision => (double)left >= right._double,
                    NumericalType.DecimalNumber => (decimal)left >= right._decimal,
                    _ => throw new InvalidOperationException(),
                };
            }
            if (right.Type.GetCombinedConversions().FastHasFlag(left.Type.GetSimpleConversions()))
            {
                // convert right to left, return left's type.
                return left.Type switch
                {
                    NumericalType.SignedByte => (sbyte)right >= left._sbyte,
                    NumericalType.UnsignedByte => (byte)right >= left._byte,
                    NumericalType.SignedShort => (short)right >= left._short,
                    NumericalType.UnsignedShort => (ushort)right >= left._ushort,
                    NumericalType.SignedInt => (int)right >= left._int,
                    NumericalType.UnsignedInt => (uint)right >= left._uint,
                    NumericalType.SignedLong => (long)right >= left._long,
                    NumericalType.UnsignedLong => (ulong)right >= left._ulong,
                    NumericalType.SinglePrecision => (float)right >= left._float,
                    NumericalType.DoublePrecision => (double)right >= left._double,
                    NumericalType.DecimalNumber => (decimal)right >= left._decimal,
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
                    NumericalType.SignedByte => left._sbyte < right._sbyte,
                    NumericalType.UnsignedByte => left._byte < right._byte,
                    NumericalType.SignedShort => left._short < right._short,
                    NumericalType.UnsignedShort => left._ushort < right._ushort,
                    NumericalType.SignedInt => left._int < right._int,
                    NumericalType.UnsignedInt => left._uint < right._uint,
                    NumericalType.SignedLong => left._long < right._long,
                    NumericalType.UnsignedLong => left._ulong < right._ulong,
                    NumericalType.SinglePrecision => left._float < right._float,
                    NumericalType.DoublePrecision => left._double < right._double,
                    NumericalType.DecimalNumber => left._decimal < right._decimal,
                    _ => throw new InvalidOperationException(),
                };
            }
            if (left.Type.GetCombinedConversions().FastHasFlag(right.Type.GetSimpleConversions()))
            {
                // convert left to right, return right's type.
                return right.Type switch
                {
                    NumericalType.SignedByte => (sbyte)left < right._sbyte,
                    NumericalType.UnsignedByte => (byte)left < right._byte,
                    NumericalType.SignedShort => (short)left < right._short,
                    NumericalType.UnsignedShort => (ushort)left < right._ushort,
                    NumericalType.SignedInt => (int)left < right._int,
                    NumericalType.UnsignedInt => (uint)left < right._uint,
                    NumericalType.SignedLong => (long)left < right._long,
                    NumericalType.UnsignedLong => (ulong)left < right._ulong,
                    NumericalType.SinglePrecision => (float)left < right._float,
                    NumericalType.DoublePrecision => (double)left < right._double,
                    NumericalType.DecimalNumber => (decimal)left < right._decimal,
                    _ => throw new InvalidOperationException(),
                };
            }
            if (right.Type.GetCombinedConversions().FastHasFlag(left.Type.GetSimpleConversions()))
            {
                // convert right to left, return left's type.
                return left.Type switch
                {
                    NumericalType.SignedByte => (sbyte)right < left._sbyte,
                    NumericalType.UnsignedByte => (byte)right < left._byte,
                    NumericalType.SignedShort => (short)right < left._short,
                    NumericalType.UnsignedShort => (ushort)right < left._ushort,
                    NumericalType.SignedInt => (int)right < left._int,
                    NumericalType.UnsignedInt => (uint)right < left._uint,
                    NumericalType.SignedLong => (long)right < left._long,
                    NumericalType.UnsignedLong => (ulong)right < left._ulong,
                    NumericalType.SinglePrecision => (float)right < left._float,
                    NumericalType.DoublePrecision => (double)right < left._double,
                    NumericalType.DecimalNumber => (decimal)right < left._decimal,
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
                    NumericalType.SignedByte => left._sbyte > right._sbyte,
                    NumericalType.UnsignedByte => left._byte > right._byte,
                    NumericalType.SignedShort => left._short > right._short,
                    NumericalType.UnsignedShort => left._ushort > right._ushort,
                    NumericalType.SignedInt => left._int > right._int,
                    NumericalType.UnsignedInt => left._uint > right._uint,
                    NumericalType.SignedLong => left._long > right._long,
                    NumericalType.UnsignedLong => left._ulong > right._ulong,
                    NumericalType.SinglePrecision => left._float > right._float,
                    NumericalType.DoublePrecision => left._double > right._double,
                    NumericalType.DecimalNumber => left._decimal > right._decimal,
                    _ => throw new InvalidOperationException(),
                };
            }
            if (left.Type.GetCombinedConversions().FastHasFlag(right.Type.GetSimpleConversions()))
            {
                // convert left to right, return right's type.
                return right.Type switch
                {
                    NumericalType.SignedByte => (sbyte)left > right._sbyte,
                    NumericalType.UnsignedByte => (byte)left > right._byte,
                    NumericalType.SignedShort => (short)left > right._short,
                    NumericalType.UnsignedShort => (ushort)left > right._ushort,
                    NumericalType.SignedInt => (int)left > right._int,
                    NumericalType.UnsignedInt => (uint)left > right._uint,
                    NumericalType.SignedLong => (long)left > right._long,
                    NumericalType.UnsignedLong => (ulong)left > right._ulong,
                    NumericalType.SinglePrecision => (float)left > right._float,
                    NumericalType.DoublePrecision => (double)left > right._double,
                    NumericalType.DecimalNumber => (decimal)left > right._decimal,
                    _ => throw new InvalidOperationException(),
                };
            }
            if (right.Type.GetCombinedConversions().FastHasFlag(left.Type.GetSimpleConversions()))
            {
                // convert right to left, return left's type.
                return left.Type switch
                {
                    NumericalType.SignedByte => (sbyte)right > left._sbyte,
                    NumericalType.UnsignedByte => (byte)right > left._byte,
                    NumericalType.SignedShort => (short)right > left._short,
                    NumericalType.UnsignedShort => (ushort)right > left._ushort,
                    NumericalType.SignedInt => (int)right > left._int,
                    NumericalType.UnsignedInt => (uint)right > left._uint,
                    NumericalType.SignedLong => (long)right > left._long,
                    NumericalType.UnsignedLong => (ulong)right > left._ulong,
                    NumericalType.SinglePrecision => (float)right > left._float,
                    NumericalType.DoublePrecision => (double)right > left._double,
                    NumericalType.DecimalNumber => (decimal)right > left._decimal,
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


        public static Numerical FromSByte(sbyte b)
        {
            var newObj = new Numerical(NumericalType.SignedByte);
            newObj._sbyte = b;
            return newObj;
        }
        public static Numerical FromByte(byte b)
        {
            var newObj = new Numerical(NumericalType.UnsignedByte);
            newObj._byte = b;
            return newObj;
        }
        public static Numerical FromInt16(short b)
        {
            var newObj = new Numerical(NumericalType.SignedShort);
            newObj._short = b;
            return newObj;
        }
        public static Numerical FromUInt16(ushort b)
        {
            var newObj = new Numerical(NumericalType.UnsignedShort);
            newObj._ushort = b;
            return newObj;
        }
        public static Numerical FromInt32(int b)
        {
            var newObj = new Numerical(NumericalType.SignedInt);
            newObj._int = b;
            return newObj;
        }
        public static Numerical FromUInt32(uint b)
        {
            var newObj = new Numerical(NumericalType.UnsignedInt);
            newObj._uint = b;
            return newObj;
        }
        public static Numerical FromInt64(long b)
        {
            var newObj = new Numerical(NumericalType.SignedLong);
            newObj._long = b;
            return newObj;
        }
        public static Numerical FromUInt64(ulong b)
        {
            var newObj = new Numerical(NumericalType.UnsignedLong);
            newObj._ulong = b;
            return newObj;
        }
        public static Numerical FromSingle(float b)
        {
            var newObj = new Numerical(NumericalType.SinglePrecision);
            newObj._float = b;
            return newObj;
        }
        public static Numerical FromDouble(double b)
        {
            var newObj = new Numerical(NumericalType.DoublePrecision);
            newObj._double = b;
            return newObj;
        }
        public static Numerical FromDecimal(decimal b)
        {
            var newObj = new Numerical(NumericalType.DecimalNumber);
            newObj._decimal = b;
            return newObj;
        }

        public sbyte ToSByte()
        {
            return this.Type switch
            {
                NumericalType.SignedByte => this._sbyte,
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
                NumericalType.UnsignedByte => this._byte,
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
                NumericalType.SignedByte => this._sbyte,
                NumericalType.UnsignedByte => this._byte,
                NumericalType.SignedShort => this._short,
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
                NumericalType.UnsignedByte => this._byte,
                NumericalType.SignedShort => throw new InvalidOperationException(),
                NumericalType.UnsignedShort => this._ushort,
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
                NumericalType.SignedByte => this._sbyte,
                NumericalType.UnsignedByte => this._byte,
                NumericalType.SignedShort => this._short,
                NumericalType.UnsignedShort => this._ushort,
                NumericalType.SignedInt => this._int,
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
                NumericalType.UnsignedByte => this._byte,
                NumericalType.SignedShort => throw new InvalidOperationException(),
                NumericalType.UnsignedShort => this._ushort,
                NumericalType.SignedInt => throw new InvalidOperationException(),
                NumericalType.UnsignedInt => this._uint,
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
                NumericalType.SignedByte => this._sbyte,
                NumericalType.UnsignedByte => this._byte,
                NumericalType.SignedShort => this._short,
                NumericalType.UnsignedShort => this._ushort,
                NumericalType.SignedInt => this._int,
                NumericalType.UnsignedInt => this._uint,
                NumericalType.SignedLong => this._long,
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
                NumericalType.UnsignedByte => this._byte,
                NumericalType.SignedShort => throw new InvalidOperationException(),
                NumericalType.UnsignedShort => this._ushort,
                NumericalType.SignedInt => throw new InvalidOperationException(),
                NumericalType.UnsignedInt => this._uint,
                NumericalType.SignedLong => throw new InvalidOperationException(),
                NumericalType.UnsignedLong => this._ulong,
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
                NumericalType.SignedByte => this._sbyte,
                NumericalType.UnsignedByte => this._byte,
                NumericalType.SignedShort => this._short,
                NumericalType.UnsignedShort => this._ushort,
                NumericalType.SignedInt => this._int,
                NumericalType.UnsignedInt => this._uint,
                NumericalType.SignedLong => this._long,
                NumericalType.UnsignedLong => this._ulong,
                NumericalType.SinglePrecision => this._float,
                NumericalType.DoublePrecision => throw new InvalidOperationException(),
                NumericalType.DecimalNumber => throw new InvalidOperationException(),
                _ => throw new InvalidOperationException(),
            };
        }

        public double ToDouble()
        {
            return this.Type switch
            {
                NumericalType.SignedByte => this._sbyte,
                NumericalType.UnsignedByte => this._byte,
                NumericalType.SignedShort => this._short,
                NumericalType.UnsignedShort => this._ushort,
                NumericalType.SignedInt => this._int,
                NumericalType.UnsignedInt => this._uint,
                NumericalType.SignedLong => this._long,
                NumericalType.UnsignedLong => this._ulong,
                NumericalType.SinglePrecision => this._float,
                NumericalType.DoublePrecision => this._double,
                NumericalType.DecimalNumber => throw new InvalidOperationException(),
                _ => throw new InvalidOperationException(),
            };
        }

        public decimal ToDecimal()
        {
            return this.Type switch
            {
                NumericalType.SignedByte => this._sbyte,
                NumericalType.UnsignedByte => this._byte,
                NumericalType.SignedShort => this._short,
                NumericalType.UnsignedShort => this._ushort,
                NumericalType.SignedInt => this._int,
                NumericalType.UnsignedInt => this._uint,
                NumericalType.SignedLong => this._long,
                NumericalType.UnsignedLong => this._ulong,
                NumericalType.SinglePrecision => throw new InvalidOperationException(),
                NumericalType.DoublePrecision => throw new InvalidOperationException(),
                NumericalType.DecimalNumber => this._decimal,
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
                    NumericalType.SignedByte => this._sbyte == other._sbyte,
                    NumericalType.UnsignedByte => this._byte == other._byte,
                    NumericalType.SignedShort => this._short == other._short,
                    NumericalType.UnsignedShort => this._ushort == other._ushort,
                    NumericalType.SignedInt => this._int == other._int,
                    NumericalType.UnsignedInt => this._uint == other._uint,
                    NumericalType.SignedLong => this._long == other._long,
                    NumericalType.UnsignedLong => this._ulong == other._ulong,
                    NumericalType.SinglePrecision => this._float == other._float,
                    NumericalType.DoublePrecision => this._double == other._double,
                    NumericalType.DecimalNumber => this._decimal == other._decimal,
                    _ => false
                };
            }
            if (this.Type.GetCombinedConversions().FastHasFlag(other.Type.GetSimpleConversions()))
            {
                // convert this to other, return other's type.
                return other.Type switch
                {
                    NumericalType.SignedByte => (sbyte)this == other._sbyte,
                    NumericalType.UnsignedByte => (byte)this == other._byte,
                    NumericalType.SignedShort => (short)this == other._short,
                    NumericalType.UnsignedShort => (ushort)this == other._ushort,
                    NumericalType.SignedInt => (int)this == other._int,
                    NumericalType.UnsignedInt => (uint)this == other._uint,
                    NumericalType.SignedLong => (long)this == other._long,
                    NumericalType.UnsignedLong => (ulong)this == other._ulong,
                    NumericalType.SinglePrecision => (float)this == other._float,
                    NumericalType.DoublePrecision => (double)this == other._double,
                    NumericalType.DecimalNumber => (decimal)this == other._decimal,
                    _ => false
                };
            }
            if (other.Type.GetCombinedConversions().FastHasFlag(this.Type.GetSimpleConversions()))
            {
                // convert other to this, return this's type.
                return this.Type switch
                {
                    NumericalType.SignedByte => (sbyte)other == this._sbyte,
                    NumericalType.UnsignedByte => (byte)other == this._byte,
                    NumericalType.SignedShort => (short)other == this._short,
                    NumericalType.UnsignedShort => (ushort)other == this._ushort,
                    NumericalType.SignedInt => (int)other == this._int,
                    NumericalType.UnsignedInt => (uint)other == this._uint,
                    NumericalType.SignedLong => (long)other == this._long,
                    NumericalType.UnsignedLong => (ulong)other == this._ulong,
                    NumericalType.SinglePrecision => (float)other == this._float,
                    NumericalType.DoublePrecision => (double)other == this._double,
                    NumericalType.DecimalNumber => (decimal)other == this._decimal,
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
                    NumericalType.SignedByte => this._sbyte.CompareTo(other._sbyte),
                    NumericalType.UnsignedByte => this._byte.CompareTo(other._byte),
                    NumericalType.SignedShort => this._short.CompareTo(other._short),
                    NumericalType.UnsignedShort => this._ushort.CompareTo(other._ushort),
                    NumericalType.SignedInt => this._int.CompareTo(other._int),
                    NumericalType.UnsignedInt => this._uint.CompareTo(other._uint),
                    NumericalType.SignedLong => this._long.CompareTo(other._long),
                    NumericalType.UnsignedLong => this._ulong.CompareTo(other._ulong),
                    NumericalType.SinglePrecision => this._float.CompareTo(other._float),
                    NumericalType.DoublePrecision => this._double.CompareTo(other._double),
                    NumericalType.DecimalNumber => this._decimal.CompareTo(other._decimal),
                    _ => throw new InvalidOperationException(),
                };
            }
            if (this.Type.GetCombinedConversions().FastHasFlag(other.Type.GetSimpleConversions()))
            {
                // convert this to other, return other's type.
                return other.Type switch
                {
                    NumericalType.SignedByte => ((sbyte)this).CompareTo(other._sbyte),
                    NumericalType.UnsignedByte => ((byte)this).CompareTo(other._byte),
                    NumericalType.SignedShort => ((short)this).CompareTo(other._short),
                    NumericalType.UnsignedShort => ((ushort)this).CompareTo(other._ushort),
                    NumericalType.SignedInt => ((int)this).CompareTo(other._int),
                    NumericalType.UnsignedInt => ((uint)this).CompareTo(other._uint),
                    NumericalType.SignedLong => ((long)this).CompareTo(other._long),
                    NumericalType.UnsignedLong => ((ulong)this).CompareTo(other._ulong),
                    NumericalType.SinglePrecision => ((float)this).CompareTo(other._float),
                    NumericalType.DoublePrecision => ((double)this).CompareTo(other._double),
                    NumericalType.DecimalNumber => ((decimal)this).CompareTo(other._decimal),
                    _ => throw new InvalidOperationException(),
                };
            }
            if (other.Type.GetCombinedConversions().FastHasFlag(this.Type.GetSimpleConversions()))
            {
                // convert other to this, return this's type.
                return this.Type switch
                {
                    NumericalType.SignedByte => ((sbyte)other).CompareTo(this._sbyte),
                    NumericalType.UnsignedByte => ((byte)other).CompareTo(this._byte),
                    NumericalType.SignedShort => ((short)other).CompareTo(this._short),
                    NumericalType.UnsignedShort => ((ushort)other).CompareTo(this._ushort),
                    NumericalType.SignedInt => ((int)other).CompareTo(this._int),
                    NumericalType.UnsignedInt => ((uint)other).CompareTo(this._uint),
                    NumericalType.SignedLong => ((long)other).CompareTo(this._long),
                    NumericalType.UnsignedLong => ((ulong)other).CompareTo(this._ulong),
                    NumericalType.SinglePrecision => ((float)other).CompareTo(this._float),
                    NumericalType.DoublePrecision => ((double)other).CompareTo(this._double),
                    NumericalType.DecimalNumber => ((decimal)other).CompareTo(this._decimal),
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
                NumericalType.Unknown => _sbyte.GetHashCode(),
                NumericalType.SignedByte => _sbyte.GetHashCode(),
                NumericalType.UnsignedByte => _byte.GetHashCode(),
                NumericalType.SignedShort => _short.GetHashCode(),
                NumericalType.UnsignedShort => _ushort.GetHashCode(),
                NumericalType.SignedInt => _int.GetHashCode(),
                NumericalType.UnsignedInt => _uint.GetHashCode(),
                NumericalType.SignedLong => _long.GetHashCode(),
                NumericalType.UnsignedLong => _ulong.GetHashCode(),
                NumericalType.SinglePrecision => _float.GetHashCode(),
                NumericalType.DoublePrecision => _double.GetHashCode(),
                NumericalType.DecimalNumber => _decimal.GetHashCode(),
                _ => _decimal.GetHashCode()
            };
        }


        public Numerical Abs()
        {
            return Type switch
            {
                NumericalType.SignedByte => Math.Abs(_sbyte),
                NumericalType.UnsignedByte => _byte,
                NumericalType.SignedShort => Math.Abs(_short),
                NumericalType.UnsignedShort => _ushort,
                NumericalType.SignedInt => Math.Abs(_int),
                NumericalType.UnsignedInt => _uint,
                NumericalType.SignedLong => Math.Abs(_long),
                NumericalType.UnsignedLong => _ulong,
                NumericalType.SinglePrecision => Math.Abs(_float),
                NumericalType.DoublePrecision => Math.Abs(_double),
                NumericalType.DecimalNumber => Math.Abs(_decimal),
                _ => throw new InvalidOperationException(),
            };
        }
        public string ToString(IFormatProvider provider)
        {
            return Type switch
            {
                NumericalType.SignedByte => _sbyte.ToString(provider),
                NumericalType.UnsignedByte => _byte.ToString(provider),
                NumericalType.SignedShort => _short.ToString(provider),
                NumericalType.UnsignedShort => _ushort.ToString(provider),
                NumericalType.SignedInt => _int.ToString(provider),
                NumericalType.UnsignedInt => _uint.ToString(provider),
                NumericalType.SignedLong => _long.ToString(provider),
                NumericalType.UnsignedLong => _ulong.ToString(provider),
                NumericalType.SinglePrecision => _float.ToString(provider),
                NumericalType.DoublePrecision => _double.ToString(provider),
                NumericalType.DecimalNumber => _decimal.ToString(provider),
                _ => base.ToString()
            };
        }

        public override string ToString()
        {
            return Type switch
            {
                NumericalType.SignedByte => _sbyte.ToString(),
                NumericalType.UnsignedByte => _byte.ToString(),
                NumericalType.SignedShort => _short.ToString(),
                NumericalType.UnsignedShort => _ushort.ToString(),
                NumericalType.SignedInt => _int.ToString(),
                NumericalType.UnsignedInt => _uint.ToString(),
                NumericalType.SignedLong => _long.ToString(),
                NumericalType.UnsignedLong => _ulong.ToString(),
                NumericalType.SinglePrecision => _float.ToString(),
                NumericalType.DoublePrecision => _double.ToString(),
                NumericalType.DecimalNumber => _decimal.ToString(),
                _ => base.ToString()
            };
        }
    }
}
