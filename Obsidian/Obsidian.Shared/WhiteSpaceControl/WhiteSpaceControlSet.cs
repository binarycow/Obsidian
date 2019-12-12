using System;
using System.Collections.Generic;
using System.Text;

namespace Obsidian.WhiteSpaceControl
{
    internal struct WhiteSpaceControlSet : IEquatable<WhiteSpaceControlSet>
    {
        internal WhiteSpaceControlSet(WhiteSpaceMode? start = null, WhiteSpaceMode? end = null)
        {
            Start = start ?? WhiteSpaceMode.Default;
            End = end ?? WhiteSpaceMode.Default;
        }
        internal WhiteSpaceMode Start { get; }
        internal WhiteSpaceMode End { get; }

        public override bool Equals(object? obj)
        {
            return obj is WhiteSpaceControlSet set && Equals(set);
        }

        public bool Equals(WhiteSpaceControlSet other)
        {
            return Start == other.Start &&
                   End == other.End;
        }

        public override int GetHashCode()
        {
            var hashCode = -1676728671;
            hashCode = hashCode * -1521134295 + Start.GetHashCode();
            hashCode = hashCode * -1521134295 + End.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(WhiteSpaceControlSet left, WhiteSpaceControlSet right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(WhiteSpaceControlSet left, WhiteSpaceControlSet right)
        {
            return !(left == right);
        }
    }
}
