using System;
using System.Linq;
using CodeButler.Reorganizing;
using FluentAssertions;
using Xunit;

namespace CodeButler.UnitTests.Reorganizing;

public class UsingInfoTest
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
        var leftUsingInfo = new UsingInfo(string.Empty) { Alias = left };
        var rightUsingInfo = new UsingInfo(string.Empty) { Alias = right };

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
    public void OrderByAliasThenByName(
        string leftAlias,
        string leftName,
        string rightAlias,
        string rightName,
        int expected
    )
    {
        var leftUsingInfo = new UsingInfo(leftName) { Alias = leftAlias };
        var rightUsingInfo = new UsingInfo(rightName) { Alias = rightAlias };

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
    public void OrderByIsStaticThenByAlias(
        bool leftIsStatic,
        string leftAlias,
        bool rightIsStatic,
        string rightAlias,
        int expected
    )
    {
        var leftUsingInfo = new UsingInfo(string.Empty)
        {
            IsStatic = leftIsStatic,
            Alias = leftAlias
        };
        var rightUsingInfo = new UsingInfo(string.Empty)
        {
            IsStatic = rightIsStatic,
            Alias = rightAlias
        };

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
        var leftUsingInfo = new UsingInfo(left);
        var rightUsingInfo = new UsingInfo(right);

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
        var leftUsingInfo = new UsingInfo(string.Empty) { IsStatic = left };
        var rightUsingInfo = new UsingInfo(string.Empty) { IsStatic = right };

        int result = leftUsingInfo.CompareByIsStatic(rightUsingInfo);

        AssertCompare(expected, result);
    }

    [Theory]
    [InlineData(true, false, -1)]
    [InlineData(false, true, 1)]
    [InlineData(false, false, 0)]
    [InlineData(true, true, 0)]
    public void OrderByGlobal(bool left, bool right, int expected)
    {
        var leftUsingInfo = new UsingInfo(string.Empty) { IsGlobal = left };
        var rightUsingInfo = new UsingInfo(string.Empty) { IsGlobal = right };

        int result = leftUsingInfo.CompareByIsGlobal(rightUsingInfo);

        AssertCompare(expected, result);
    }

    [Fact]
    public void Order_Should_Be_As_Expected()
    {
        var expected = new UsingInfo[]
        {
            new("System.Linq") { IsGlobal = true },
            new("System"),
            new("System.Text"),
            new("Xunit"),
            new("Xunit.Fact") { Alias = "FakeNews" },
            new("System.Math") { IsStatic = true },
        };

        var rnd = new Random();
        var input = expected.OrderBy(_ => rnd.Next()).ToArray();
        var sorted = input.OrderBy(x => x).ToArray();

        for (int i = 0; i < expected.Length; i++)
        {
            sorted[i].Should().BeSameAs(expected[i]);
        }
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
