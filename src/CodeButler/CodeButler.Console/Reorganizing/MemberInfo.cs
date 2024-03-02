using System;
using System.Collections.Generic;

namespace CodeButler.Reorganizing;

public class MemberInfo : IEquatable<MemberInfo?>
{
    public MemberInfo()
    {
        AccessModifier = MemberAccessModifier.None;
        AdditionalModifier = MemberAdditionalModifier.None;
        MemberType = MemberType.None;
    }

    public MemberAccessModifier AccessModifier { get; init; }

    public MemberAdditionalModifier AdditionalModifier { get; init; }

    public string? Identifier { get; init; }

    public MemberType MemberType { get; init; }

    public static bool operator !=(MemberInfo? left, MemberInfo? right)
    {
        return !(left == right);
    }

    public static bool operator ==(MemberInfo? left, MemberInfo? right)
    {
        return EqualityComparer<MemberInfo>.Default.Equals(left, right);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as MemberInfo);
    }

    public bool Equals(MemberInfo? other)
    {
        return other != null
            && MemberType == other.MemberType
            && Identifier == other.Identifier
            && AccessModifier == other.AccessModifier
            && AdditionalModifier == other.AdditionalModifier;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(MemberType, Identifier, AccessModifier, AdditionalModifier);
    }
}
