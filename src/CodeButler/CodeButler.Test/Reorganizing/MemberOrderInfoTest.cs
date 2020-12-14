using System;
using CodeButler.Reorganizing;
using FluentAssertions;
using Xunit;

namespace CodeButler.Test.Reorganizing
{
    public class MemberOrderInfoTest
    {
        [Theory]
        [InlineData("A", "A", 0)]
        [InlineData("a", "a", 0)]
        [InlineData("A", "B", -1)]
        [InlineData("C", "B", 1)]
        [InlineData("a", "A", 1)]
        [InlineData("A", "a", -1)]
        [InlineData("a", "aa", -1)]
        [InlineData("aA", "Aa", 1)]
        public void OrderByIdentifier(string left, string right, int expected)
        {
            var leftOrderInfo = new MemberOrderInfo() { Identifier = left };
            var rightOrderInfo = new MemberOrderInfo() { Identifier = right };

            int result = leftOrderInfo.CompareTo(rightOrderInfo);

            AssertCompare(expected, result);
        }

        [Theory]
        [InlineData(MemberType.Field, MemberAccessModifier.Private, MemberType.Property, MemberAccessModifier.Public, -1)]
        public void OrderByMemberTypeThenByAccessModifier(MemberType leftType, MemberAccessModifier leftAccess, MemberType rightType, MemberAccessModifier rightAccess, int expected)
        {
            var leftOrderInfo = new MemberOrderInfo() { MemberType = leftType, AcessModifier = leftAccess };
            var rightOrderInfo = new MemberOrderInfo() { MemberType = rightType, AcessModifier = rightAccess };

            int result = leftOrderInfo.CompareTo(rightOrderInfo);

            AssertCompare(expected, result);
        }

        [Theory]
        [InlineData(MemberAccessModifier.Private, MemberAdditionalModifier.Const, MemberAccessModifier.Public, MemberAdditionalModifier.Static, 1)]
        public void OrderByAccessModifierThenByAdditionalModifier(MemberAccessModifier leftAccess, MemberAdditionalModifier leftAdditional, MemberAccessModifier rightAccess, MemberAdditionalModifier rightAdditional, int expected)
        {
            var leftOrderInfo = new MemberOrderInfo() { AcessModifier = leftAccess, AdditionalModifier = leftAdditional };
            var rightOrderInfo = new MemberOrderInfo() { AcessModifier = rightAccess, AdditionalModifier = rightAdditional };

            int result = leftOrderInfo.CompareTo(rightOrderInfo);

            AssertCompare(expected, result);
        }

        [Theory]
        [InlineData(MemberAdditionalModifier.Const, "B", MemberAdditionalModifier.Static, "A", -1)]
        public void OrderByAdditionalModifierThenByIdentifier(MemberAdditionalModifier leftAdditional, string leftIdentifier, MemberAdditionalModifier rightAdditional, string rightIdentifier, int expected)
        {
            var leftOrderInfo = new MemberOrderInfo() { AdditionalModifier = leftAdditional, Identifier = leftIdentifier };
            var rightOrderInfo = new MemberOrderInfo() { AdditionalModifier = rightAdditional, Identifier = rightIdentifier };

            int result = leftOrderInfo.CompareTo(rightOrderInfo);

            AssertCompare(expected, result);
        }

        [Theory]
        [InlineData(MemberAdditionalModifier.None, MemberAdditionalModifier.None, 0)]
        [InlineData(MemberAdditionalModifier.Const, MemberAdditionalModifier.Readonly, -1)]
        [InlineData(MemberAdditionalModifier.Readonly, MemberAdditionalModifier.StaticReadonly, 1)]
        [InlineData(MemberAdditionalModifier.None, MemberAdditionalModifier.Const, -1)]
        public void OrderByAdditionalModifier(MemberAdditionalModifier left, MemberAdditionalModifier right, int expected)
        {
            var leftOrderInfo = new MemberOrderInfo() { AdditionalModifier = left };
            var rightOrderInfo = new MemberOrderInfo() { AdditionalModifier = right };

            int result = leftOrderInfo.CompareTo(rightOrderInfo);

            AssertCompare(expected, result);
        }

        [Theory]
        [InlineData(MemberAccessModifier.None, MemberAccessModifier.None, 0)]
        [InlineData(MemberAccessModifier.Private, MemberAccessModifier.Private, 0)]
        [InlineData(MemberAccessModifier.Public, MemberAccessModifier.Protected, -1)]
        [InlineData(MemberAccessModifier.Internal, MemberAccessModifier.ProtectedInternal, -1)]
        [InlineData(MemberAccessModifier.Protected, MemberAccessModifier.None, 1)]
        [InlineData(MemberAccessModifier.PrivateProtected, MemberAccessModifier.Public, 1)]
        public void OrderByAccessModifier(MemberAccessModifier left, MemberAccessModifier right, int expected)
        {
            var leftOrderInfo = new MemberOrderInfo() { AcessModifier = left };
            var rightOrderInfo = new MemberOrderInfo() { AcessModifier = right };

            int result = leftOrderInfo.CompareTo(rightOrderInfo);

            AssertCompare(expected, result);
        }

        [Theory]
        [InlineData(MemberType.None, MemberType.None, 0)]
        [InlineData(MemberType.Field, MemberType.Field, 0)]
        [InlineData(MemberType.Constructor, MemberType.Destructor, -1)]
        [InlineData(MemberType.Interface, MemberType.Method, -1)]
        [InlineData(MemberType.Event, MemberType.Delegate, 1)]
        [InlineData(MemberType.Property, MemberType.Enum, 1)]
        public void OrderByType(MemberType left, MemberType right, int expected)
        {
            var leftOrderInfo = new MemberOrderInfo() { MemberType = left };
            var rightOrderInfo = new MemberOrderInfo() { MemberType = right };

            int result = leftOrderInfo.CompareTo(rightOrderInfo);

            AssertCompare(expected, result);
        }

        private static void AssertCompare(int expected, int result)
        {
            if (expected == 0)
            {
                result.Should().Be(0);
            }
            else if (expected < 0)
            {
                result.Should().BeNegative();
            }
            else if (expected > 0)
            {
                result.Should().BePositive();
            }
            else
            {
                throw new Exception("CompareAssert badly implemented.");
            }
        }
    }
}