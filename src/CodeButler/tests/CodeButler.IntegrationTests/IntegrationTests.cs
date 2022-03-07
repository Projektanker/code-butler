using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace CodeButler.IntegrationTests
{
    public class IntegrationTests
    {
        public static IEnumerable<object[]> TestCases =>
            Directory.EnumerateDirectories("TestCases")
            .Select(dir => dir.Split(Path.DirectorySeparatorChar)[^1])
            .Select(dirname => new[] { dirname });

        [Theory]
        [MemberData(nameof(TestCases))]
        public void DirectUseOfProgramClass(string folder)
        {
            string originalPath = Path.Combine("TestCases", folder, "original.cs.test");
            string cleanPath = Path.Combine("TestCases", folder, "clean.cs.test");

            string original = File.ReadAllText(originalPath);
            string clean = File.ReadAllText(cleanPath);

            Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax parsedOriginal = Program.Parse(original);
            string reorganized = Program.Reorganize(parsedOriginal).ToFullString();

            _ = reorganized.Should().Be(clean);
        }
    }
}