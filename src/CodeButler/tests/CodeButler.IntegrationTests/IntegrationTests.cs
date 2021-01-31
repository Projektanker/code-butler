using System;
using System.IO;
using FluentAssertions;
using Xunit;

namespace CodeButler.IntegrationTests
{
    public class IntegrationTests
    {
        [Theory()]
        [InlineData("fields")]
        [InlineData("usings")]
        public void DirectUseOfProgramClass(string folder)
        {
            string originalPath = Path.Combine("TestCases", folder, "original.cs.test");
            string cleanPath = Path.Combine("TestCases", folder, "clean.cs.test");

            string original = File.ReadAllText(originalPath);
            string clean = File.ReadAllText(cleanPath);

            var parsedOriginal = Program.Parse(original);
            var reorganized = Program.Reorganize(parsedOriginal).ToFullString();

            reorganized.Should().Be(clean);
        }
    }
}