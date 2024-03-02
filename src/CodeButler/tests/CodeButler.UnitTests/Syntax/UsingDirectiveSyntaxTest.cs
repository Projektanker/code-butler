using CodeButler.Syntax;
using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace CodeButler.UnitTests.Syntax;

public class UsingDirectiveSyntaxTest
{
    [Theory]
    [InlineData("System", null, false, false)]
    [InlineData("System", null, false, true)]
    [InlineData("System.Math", null, true, false)]
    [InlineData("System.Math", null, true, true)]
    [InlineData("System.Test", "Test", false, false)]
    [InlineData("System.Test", "Test", true, false)]
    public void GetUsingOrderInfoTest(string name, string alias, bool isStatic, bool isGlobal)
    {
        var usingDirective = SyntaxFactory.UsingDirective(
            globalKeyword: SyntaxFactory.Token(
                isGlobal ? SyntaxKind.GlobalKeyword : SyntaxKind.None
            ),
            usingKeyword: SyntaxFactory.Token(SyntaxKind.UsingKeyword),
            staticKeyword: SyntaxFactory.Token(
                isStatic ? SyntaxKind.StaticKeyword : SyntaxKind.None
            ),
            alias: alias is null ? null : SyntaxFactory.NameEquals(alias),
            name: SyntaxFactory.ParseName(name),
            semicolonToken: SyntaxFactory.Token(SyntaxKind.SemicolonToken)
        );

        var usingOrderInfo = usingDirective.GetUsingInfo();
        usingOrderInfo.Name.Should().Be(name);
        usingOrderInfo.Alias.Should().Be(alias);
        usingOrderInfo.IsStatic.Should().Be(isStatic);
        usingOrderInfo.IsGlobal.Should().Be(isGlobal);
    }
}
