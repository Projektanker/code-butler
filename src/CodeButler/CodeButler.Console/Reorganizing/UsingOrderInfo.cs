using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeButler.Reorganizing
{
    public sealed class UsingOrderInfo : IEquatable<UsingOrderInfo?>, IComparable<UsingOrderInfo>
    {
        private const string _systemUsing = "System";

        private readonly Func<UsingOrderInfo?, int>[] _compareMethods;

        public UsingOrderInfo(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _compareMethods = new Func<UsingOrderInfo?, int>[]
            {
                CompareByIsGlobal,
                CompareByIsStatic,
                CompareByAlias,
                CompareByName,
            };
        }

        public string? Alias { get; set; }

        public bool IsStatic { get; set; }
        public bool IsGlobal { get; set; }

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
            if (other is null)
            {
                return -1;
            }

            return string.Compare(Alias, other.Alias, StringComparison.Ordinal);
        }

        public int CompareByIsStatic(UsingOrderInfo? other)
        {
            if (other is null)
            {
                return -1;
            }

            return (IsStatic, other.IsStatic) switch
            {
                (false, true) => -1,
                (true, false) => 1,
                (_, _) => 0,
            };
        }

        public int CompareByIsGlobal(UsingOrderInfo? other)
        {
            if (other is null)
            {
                return -1;
            }

            return (IsGlobal, other.IsGlobal) switch
            {
                (true, false) => -1,
                (false, true) => 1,
                (_, _) => 0,
            };
        }

        public int CompareByName(UsingOrderInfo? other)
        {
            if (other is null)
            {
                return -1;
            }

            return (IsSystemUsing(this), IsSystemUsing(other)) switch
            {
                (true, false) => -1,
                (false, true) => 1,
                (_, _) => string.Compare(Name, other.Name, StringComparison.Ordinal),
            };
        }

        public int CompareTo(UsingOrderInfo? other)
        {
            return _compareMethods
                   .Select(compareMethod => compareMethod(other))
                   .FirstOrDefault(result => result != 0);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as UsingOrderInfo);
        }

        public bool Equals(UsingOrderInfo? other)
        {
            return other != null &&
                   Name == other.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (IsGlobal)
            {
                sb.Append("global ");
            }

            sb.Append("using");

            if (!string.IsNullOrEmpty(Alias))
            {
                sb.Append($" {Alias} =");
            }

            if (IsStatic)
            {
                sb.Append(" static");
            }

            sb.Append($" {Name};");

            return sb.ToString();
        }

        private static bool IsSystemUsing(UsingOrderInfo usingOrderInfo)
        {
            return usingOrderInfo.Name.StartsWith(_systemUsing, StringComparison.Ordinal);
        }
    }
}