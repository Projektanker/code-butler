using System;
using System.IO;
using FluentAssertions;
using Xunit;

namespace CodeButler.IntegrationTests
{
    public class IntegrationTests
    {
        [Theory()]
        [InlineData("Fields")]
        [InlineData("LeadingTrivia")]
        [InlineData("InnerUsings")]
        [InlineData("OuterUsings")]
        public void DirectUseOfProgramClass(string folder)
        {
            string originalPath = Path.Combine("TestCases", folder, "original.cs");
            string cleanPath = Path.Combine("TestCases", folder, "clean.cs");

            string original = File.ReadAllText(originalPath);
            string clean = File.ReadAllText(cleanPath);

            var parsedOriginal = Program.Parse(original);
            var reorganized = Program.Reorganize(parsedOriginal).ToFullString();

            reorganized.Should().Be(clean);
        }
    }
}