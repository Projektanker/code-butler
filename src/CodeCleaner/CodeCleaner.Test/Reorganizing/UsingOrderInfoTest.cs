using System;
using CodeCleaner.Reorganizing;
using FluentAssertions;
using Xunit;

namespace CodeCleaner.Test.Reorganizing
{
    public class UsingOrderInfoTest
    {
        [Theory]
        [InlineData("System", "System", 0)]
        [InlineData("System.Test", "System", 1)]
        [InlineData("System", "System.Test", -1)]
        [InlineData("System", "Xunit", -1)]
        [InlineData("Xunit", "System", 1)]
        [InlineData("System", "FluentAssertions", 1)]
        [InlineData("Xunit", "FluentAssertions", 1)]
        [InlineData(null, "A", -1)]
        [InlineData(null, null, 0)]
        [InlineData("A", null, 1)]
        public void OrderByAlias(string left, string right, int expected)
        {
            var leftUsingInfo = new UsingOrderInfo(string.Empty) { Alias = left };
            var rightUsingInfo = new UsingOrderInfo(string.Empty) { Alias = right };

            int result = leftUsingInfo.CompareByAlias(rightUsingInfo);

            AssertCompare(expected, result);
        }

        [Theory]
        [InlineData("Alias-A", "Name-B", "Alias-B", "Name-A", -1)]
        [InlineData("Alias-B", "Name-B", "Alias-A", "Name-A", 1)]
        [InlineData("Alias", "Name-B", "Alias", "Name-A", 1)]
        [InlineData("Alias", "Name-A", "Alias", "Name-B", -1)]
        [InlineData(null, "Name-A", null, "Name-B", -1)]
        [InlineData(null, "Name-A", null, "Name-A", 0)]
        public void OrderByAliasThenByName(string leftAlias, string leftName, string rightAlias, string rightName, int expected)
        {
            var leftUsingInfo = new UsingOrderInfo(leftName) { Alias = leftAlias };
            var rightUsingInfo = new UsingOrderInfo(rightName) { Alias = rightAlias };

            int result = leftUsingInfo.CompareTo(rightUsingInfo);

            AssertCompare(expected, result);
        }

        [Theory]
        [InlineData(false, "B", true, "A", -1)]
        [InlineData(true, "A", false, "B", 1)]
        [InlineData(false, "A", false, "B", -1)]
        [InlineData(true, "A", true, "B", -1)]
        [InlineData(true, "B", true, "A", 1)]
        [InlineData(false, "A", false, "A", 0)]
        [InlineData(true, null, true, null, 0)]
        public void OrderByIsStaticThenByAlias(bool leftIsStatic, string leftAlias, bool rightIsStatic, string rightAlias, int expected)
        {
            var leftUsingInfo = new UsingOrderInfo(string.Empty) { IsStatic = leftIsStatic, Alias = leftAlias };
            var rightUsingInfo = new UsingOrderInfo(string.Empty) { IsStatic = rightIsStatic, Alias = rightAlias };

            int result = leftUsingInfo.CompareTo(rightUsingInfo);

            AssertCompare(expected, result);
        }

        [Theory]
        [InlineData("System", "System", 0)]
        [InlineData("System.Test", "System", 1)]
        [InlineData("System", "System.Test", -1)]
        [InlineData("System", "Xunit", -1)]
        [InlineData("Xunit", "System", 1)]
        [InlineData("System", "FluentAssertions", -1)]
        [InlineData("Xunit", "FluentAssertions", 1)]
        public void OrderByName(string left, string right, int expected)
        {
            var leftUsingInfo = new UsingOrderInfo(left);
            var rightUsingInfo = new UsingOrderInfo(right);

            int result = leftUsingInfo.CompareByName(rightUsingInfo);

            AssertCompare(expected, result);
        }

        [Theory]
        [InlineData(false, false, 0)]
        [InlineData(false, true, -1)]
        [InlineData(true, false, 1)]
        [InlineData(true, true, 0)]
        public void OrderByStatic(bool left, bool right, int expected)
        {
            var leftUsingInfo = new UsingOrderInfo(string.Empty) { IsStatic = left };
            var rightUsingInfo = new UsingOrderInfo(string.Empty) { IsStatic = right };

            int result = leftUsingInfo.CompareByIsStatic(rightUsingInfo);

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