using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obsidian
{
    internal readonly struct TemplateInfo : IEquatable<TemplateInfo>
    {
        internal TemplateInfo(string source, string filename, bool upToDate)
        {
            Source = source;
            Filename = filename;
            UpToDate = upToDate;
        }
        internal string Source { get; }
        internal string Filename { get; }
        internal bool UpToDate { get; }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            if (!(obj is TemplateInfo templateInfo)) return false;
            return Equals(templateInfo);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = (int)2166136261;
                hash = (hash * 16777619) ^ Source.GetHashCode();
                hash = (hash * 16777619) ^ Filename.GetHashCode();
                hash = (hash * 16777619) ^ UpToDate.GetHashCode();
                return hash;
            }
        }

        public static bool operator ==(TemplateInfo left, TemplateInfo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TemplateInfo left, TemplateInfo right)
        {
            return !(left == right);
        }

        public bool Equals(TemplateInfo other)
        {
            return other.Source == Source &&
                other.Filename == Filename &&
                other.UpToDate == UpToDate;
        }
    }
}
