using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Common
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Numerical : 
        IEquatable<sbyte>, IEquatable<byte>, IEquatable<short>, IEquatable<ushort>, IEquatable<int>, IEquatable<uint>, IEquatable<long>, IEquatable<ulong>, IEquatable<float>, IEquatable<double>, IEquatable<decimal>, IEquatable<Numerical>,
        IComparable<sbyte>, IComparable<byte>, IComparable<short>, IComparable<ushort>, IComparable<int>, IComparable<uint>, IComparable<long>, IComparable<ulong>, IComparable<float>, IComparable<double>, IComparable<decimal>
    {
        private static Lazy<ConcurrentDictionary<NumericalType, MethodInfo>> _TrueOperators = new Lazy<ConcurrentDictionary<NumericalType, MethodInfo>>();
        private static ConcurrentDictionary<NumericalType, MethodInfo> TrueOperators => _TrueOperators.Value;


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

        private Numerical(sbyte signedByte) : this(NumericalType.SignedByte)
        {
            _sbyte = signedByte;
        }
        private Numerical(byte unsignedByte) : this(NumericalType.UnsignedByte)
        {
            _byte = unsignedByte;
        }
        private Numerical(short signedShort) : this(NumericalType.SignedShort)
        {
            _short = signedShort;
        }
        private Numerical(ushort unsignedShort) : this(NumericalType.UnsignedShort)
        {
            _ushort = unsignedShort;
        }
        private Numerical(int signedInt) : this(NumericalType.SignedInt)
        {
            _int = signedInt;
        }
        private Numerical(ulong unsignedLong) : this(NumericalType.UnsignedLong)
        {
            _ulong = unsignedLong;
        }
        private Numerical(long signedLong) : this(NumericalType.SignedLong)
        {
            _long = signedLong;
        }
        private Numerical(float singlePrecision) : this(NumericalType.SinglePrecision)
        {
            _float = singlePrecision;
        }
        private Numerical(double doublePrecision) : this(NumericalType.DoublePrecision)
        {
            _double = doublePrecision;
        }
        private Numerical(decimal decimalNumber) : this(NumericalType.DecimalNumber)
        {
            _decimal = decimalNumber;
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

        [FieldOffset(8)]
        private NumericalType _Type;
        public NumericalType Type => _Type;

        #endregion Fields

        public static bool TryCreate(object? obj, out Numerical numerical)
        {
            numerical = default;
            if (obj == null) return false;

            switch(obj)
            {
                case sbyte sb:
                    numerical = new Numerical(sb);
                    break;
                case byte b:
                    numerical = new Numerical(b);
                    break;
                case short s:
                    numerical = new Numerical(s);
                    break;
                case ushort us:
                    numerical = new Numerical(us);
                    break;
                case long l:
                    numerical = new Numerical(l);
                    break;
                case ulong ul:
                    numerical = new Numerical(ul);
                    break;
                case float f:
                    numerical = new Numerical(f);
                    break;
                case double d:
                    numerical = new Numerical(d);
                    break;
                case decimal dec:
                    numerical = new Numerical(dec);
                    break;
                default:
                    throw new NotImplementedException();
            }
            return numerical != Unknown;
        }

        #region Unary Operators
        public static Numerical operator +(Numerical obj1) => Plus(obj1);
        public static Numerical operator -(Numerical obj1) => Negate(obj1);
        public static Numerical operator ++(Numerical obj1) => Increment(obj1);
        public static Numerical operator --(Numerical obj1) => Decrement(obj1);
        public static bool operator true(Numerical obj1) => obj1.IsTrue;
        public static bool operator false(Numerical obj1) => !obj1.IsTrue;
        public static Numerical operator !(Numerical obj1) => LogicalNot(obj1);
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
                if(TrueOperators.TryGetValue(Type, out var operatorInfo))
                {
                    return (bool)operatorInfo.Invoke(null, new object[] { this });
                }
            }
        }
        public static Numerical LogicalNot(Numerical item)
        {
            throw new NotImplementedException();
        }
        public static Numerical OnesComplement(Numerical item)
        {
            throw new NotImplementedException();
        }
        #endregion Unary Operators


        public static bool Add(Numerical left, Numerical right)
        {
            throw new NotImplementedException();
        }

        public static bool Subtract(Numerical left, Numerical right)
        {
            throw new NotImplementedException();
        }

        public static bool Multiply(Numerical left, Numerical right)
        {
            throw new NotImplementedException();
        }

        public static bool Divide(Numerical left, Numerical right)
        {
            throw new NotImplementedException();
        }

        public static bool Mod(Numerical left, Numerical right)
        {
            throw new NotImplementedException();
        }

        public static bool BitwiseAnd(Numerical left, Numerical right)
        {
            throw new NotImplementedException();
        }

        public static bool BitwiseOr(Numerical left, Numerical right)
        {
            throw new NotImplementedException();
        }

        public static bool Xor(Numerical left, Numerical right)
        {
            throw new NotImplementedException();
        }

        public static bool LeftShift(Numerical left, Numerical right)
        {
            throw new NotImplementedException();
        }

        public static bool RightShift(Numerical left, Numerical right)
        {
            throw new NotImplementedException();
        }


        #region Arithmatic Operators

        public static bool operator +(Numerical obj1, Numerical obj2)
        {
            throw new NotImplementedException();
        }
        public static bool operator -(Numerical obj1, Numerical obj2)
        {
            throw new NotImplementedException();
        }
        public static bool operator *(Numerical obj1, Numerical obj2)
        {
            throw new NotImplementedException();
        }
        public static bool operator /(Numerical obj1, Numerical obj2)
        {
            throw new NotImplementedException();
        }
        public static bool operator %(Numerical obj1, Numerical obj2)
        {
            throw new NotImplementedException();
        }
        #endregion Arithmatic Operators

        #region Bitwise Operators

        public static bool operator &(Numerical obj1, Numerical obj2)
        {
            throw new NotImplementedException();
        }
        public static bool operator |(Numerical obj1, Numerical obj2)
        {
            throw new NotImplementedException();
        }
        public static bool operator ^(Numerical obj1, Numerical obj2)
        {
            throw new NotImplementedException();
        }
        #endregion Bitwise Operators

        #region Shift Operators
        public static bool operator <<(Numerical obj1, int obj2)
        {
            throw new NotImplementedException();
        }
        public static bool operator >>(Numerical obj1, int obj2)
        {
            throw new NotImplementedException();
        }
        #endregion Shift Operators

        #region Comparison Operators

        public static bool operator ==(Numerical obj1, Numerical obj2)
        {
            throw new NotImplementedException();
        }
        public static bool operator !=(Numerical obj1, Numerical obj2)
        {
            throw new NotImplementedException();
        }
        public static bool operator <=(Numerical obj1, Numerical obj2)
        {
            throw new NotImplementedException();
        }
        public static bool operator >=(Numerical obj1, Numerical obj2)
        {
            throw new NotImplementedException();
        }
        public static bool operator <(Numerical obj1, Numerical obj2)
        {
            throw new NotImplementedException();
        }
        public static bool operator >(Numerical obj1, Numerical obj2)
        {
            throw new NotImplementedException();
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


        public static Numerical FromSByte(sbyte b) => new Numerical(b);
        public static Numerical FromByte(byte b) => new Numerical(b);
        public static Numerical FromInt16(short b) => new Numerical(b);
        public static Numerical FromUInt16(ushort b) => new Numerical(b);
        public static Numerical FromInt32(int b) => new Numerical(b);
        public static Numerical FromUInt32(uint b) => new Numerical(b);
        public static Numerical FromInt64(long b) => new Numerical(b);
        public static Numerical FromUInt64(ulong b) => new Numerical(b);
        public static Numerical FromSingle(float b) => new Numerical(b);
        public static Numerical FromDouble(double b) => new Numerical(b);
        public static Numerical FromDecimal(decimal b) => new Numerical(b);

        public sbyte ToSByte()
        {
            throw new NotImplementedException();
        }

        public byte ToByte()
        {
            throw new NotImplementedException();
        }

        public short ToInt16()
        {
            throw new NotImplementedException();
        }

        public ushort ToUInt16()
        {
            throw new NotImplementedException();
        }

        public int ToInt32()
        {
            throw new NotImplementedException();
        }

        public uint ToUInt32()
        {
            throw new NotImplementedException();
        }

        public long ToInt64()
        {
            throw new NotImplementedException();
        }

        public ulong ToUInt64()
        {
            throw new NotImplementedException();
        }

        public float ToSingle()
        {
            throw new NotImplementedException();
        }

        public double ToDouble()
        {
            throw new NotImplementedException();
        }

        public decimal ToDecimal()
        {
            throw new NotImplementedException();
        }

        #endregion Conversions

        #region IEquatable
        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        public bool Equals(sbyte other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(byte other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(short other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(ushort other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(int other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(uint other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(long other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(ulong other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(float other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(double other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(decimal other)
        {
            throw new NotImplementedException();
        }
        #endregion IEquatable

        #region IComparable
        public int CompareTo(sbyte other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(byte other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(short other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(ushort other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(int other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(uint other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(long other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(ulong other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(float other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(double other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(decimal other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(Numerical other)
        {
            throw new NotImplementedException();
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
                _ => _ulong.GetHashCode()
            };
        }



    }
}
