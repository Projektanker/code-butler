using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeCleaner.Reorganizing
{
    public class UsingOrderInfo : IEquatable<UsingOrderInfo?>, IComparable<UsingOrderInfo>
    {
        private const string _systemUsing = "System";

        private readonly Func<UsingOrderInfo?, int>[] _compareMethods;

        public UsingOrderInfo(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _compareMethods = new Func<UsingOrderInfo?, int>[]
            {
                CompareByIsStatic,
                CompareByAlias,
                CompareByName,
            };
        }

        public string? Alias { get; set; }

        public bool IsStatic { get; set; }

        public string Name { get; }

        public static bool operator !=(UsingOrderInfo? left, UsingOrderInfo? right)
        {
            return !(left == right);
        }

        public static bool operator <(UsingOrderInfo left, UsingOrderInfo right)
        {
            return left is null ? right is not null : left.CompareTo(right) < 0;
        }

        public static bool operator <=(UsingOrderInfo left, UsingOrderInfo right)
        {
            return left is null || left.CompareTo(right) <= 0;
        }

        public static bool operator ==(UsingOrderInfo? left, UsingOrderInfo? right)
        {
            return EqualityComparer<UsingOrderInfo>.Default.Equals(left, right);
        }

        public static bool operator >(UsingOrderInfo left, UsingOrderInfo right)
        {
            return left is not null && left.CompareTo(right) > 0;
        }

        public static bool operator >=(UsingOrderInfo left, UsingOrderInfo right)
        {
            return left is null ? right is null : left.CompareTo(right) >= 0;
        }

        public int CompareByAlias(UsingOrderInfo? other)
        {
            if (other is null
                || (Alias is null && other.Alias is not null))
            {
                return -1;
            }
            else if (Alias is not null && other.Alias is null)
            {
                return 1;
            }
            else
            {
                return string.Compare(Alias, other.Alias, StringComparison.Ordinal);
            }
        }

        public int CompareByIsStatic(UsingOrderInfo? other)
        {
            if (other is null
                || (!IsStatic && other.IsStatic))
            {
                return -1;
            }
            else if (IsStatic && !other.IsStatic)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public int CompareByName(UsingOrderInfo? other)
        {
            if (other is null ||
                (IsSystemUsing(this) && !IsSystemUsing(other)))
            {
                return -1;
            }
            else if (!IsSystemUsing(this) && IsSystemUsing(other))
            {
                return 1;
            }
            else
            {
                return string.Compare(Name, other.Name, StringComparison.Ordinal);
            }
        }

        /// <inheritdoc/>
        public int CompareTo(UsingOrderInfo? other)
        {
            return _compareMethods
                   .Select(compareMethod => compareMethod(other))
                   .FirstOrDefault(result => result != 0);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return Equals(obj as UsingOrderInfo);
        }

        /// <inheritdoc/>
        public bool Equals(UsingOrderInfo? other)
        {
            return other != null &&
                   Name == other.Name;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }

        private static bool IsSystemUsing(UsingOrderInfo usingOrderInfo)
        {
            return usingOrderInfo.Name.StartsWith(_systemUsing, StringComparison.Ordinal);
        }
    }
}