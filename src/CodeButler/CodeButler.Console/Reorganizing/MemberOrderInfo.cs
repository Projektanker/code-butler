using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodeButler.Reorganizing
{
    public class MemberOrderInfo : IComparable<MemberOrderInfo>, IEquatable<MemberOrderInfo?>
    {
        private readonly Func<MemberOrderInfo?, int>[] _compareMethods;

        public MemberOrderInfo()
        {
            AcessModifier = MemberAccessModifier.None;
            AdditionalModifier = MemberAdditionalModifier.None;
            MemberType = MemberType.None;

            _compareMethods = new Func<MemberOrderInfo?, int>[]
            {
                CompareByMemberType,
                CompareByAccessModifier,
                CompareByAdditionalModifier,
                CompareByIdentifier,
            };
        }

        public MemberAccessModifier AcessModifier { get; set; }

        public MemberAdditionalModifier AdditionalModifier { get; set; }

        public string? Identifier { get; set; }

        public MemberType MemberType { get; set; }

        public static bool operator !=(MemberOrderInfo? left, MemberOrderInfo? right)
        {
            return !(left == right);
        }

        public static bool operator <(MemberOrderInfo left, MemberOrderInfo right)
        {
            return left is null ? right is not null : left.CompareTo(right) < 0;
        }

        public static bool operator <=(MemberOrderInfo left, MemberOrderInfo right)
        {
            return left is null || left.CompareTo(right) <= 0;
        }

        public static bool operator ==(MemberOrderInfo? left, MemberOrderInfo? right)
        {
            return EqualityComparer<MemberOrderInfo>.Default.Equals(left, right);
        }

        public static bool operator >(MemberOrderInfo left, MemberOrderInfo right)
        {
            return left is not null && left.CompareTo(right) > 0;
        }

        public static bool operator >=(MemberOrderInfo left, MemberOrderInfo right)
        {
            return left is null ? right is null : left.CompareTo(right) >= 0;
        }

        public int CompareByAccessModifier(MemberOrderInfo? other)
        {
            return other is null ? -1 : AcessModifier - other.AcessModifier;
        }

        public int CompareByAdditionalModifier(MemberOrderInfo? other)
        {
            return other is null ? -1 : AdditionalModifier - other.AdditionalModifier;
        }

        public int CompareByIdentifier(MemberOrderInfo? other)
        {
            return (other is null) ? -1 : string.Compare(Identifier, other.Identifier, StringComparison.Ordinal);
        }

        public int CompareByMemberType(MemberOrderInfo? other)
        {
            return other is null ? -1 : MemberType - other.MemberType;
        }

        /// <inheritdoc/>
        public int CompareTo(MemberOrderInfo? other)
        {
            return _compareMethods
                .Select(compareMethod => compareMethod(other))
                .FirstOrDefault(result => result != 0);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return Equals(obj as MemberOrderInfo);
        }

        /// <inheritdoc/>
        public bool Equals(MemberOrderInfo? other)
        {
            return other != null &&
                   MemberType == other.MemberType &&
                   Identifier == other.Identifier &&
                   AcessModifier == other.AcessModifier &&
                   AdditionalModifier == other.AdditionalModifier;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(MemberType, Identifier, AcessModifier, AdditionalModifier);
        }
    }
}