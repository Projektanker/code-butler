using System.IO;
using System.Text;
using CodeButler.Syntax;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;
using static System.Math;
using static Xunit.Assert;

using TestAssert = Xunit.Assert;

namespace CodeButler.Test.Syntax
{
    public class UsingDirectiveSyntaxTest
    {
        [Theory]
        [InlineData("System", null, false)]
        [InlineData("System.Math", null, true)]
        [InlineData("System.Test", "Test", false)]
        [InlineData("System.Test", "Test", true)]
        public void GetUsingOrderInfoTest(string name, string alias, bool isStatic)
        {
            var usingDirective = SyntaxFactory.UsingDirective(
                staticKeyword: SyntaxFactory.Token(isStatic ? SyntaxKind.StaticKeyword : SyntaxKind.None),
                alias: alias is null ? null : SyntaxFactory.NameEquals(alias),
                name: SyntaxFactory.ParseName(name));

            var usingOrderInfo = UsingsOrganizer.GetUsingOrderInfo(usingDirective);
            usingOrderInfo.Name.Should().Be(name);
            usingOrderInfo.Alias.Should().Be(alias);
            usingOrderInfo.IsStatic.Should().Be(isStatic);
        }
    }
}