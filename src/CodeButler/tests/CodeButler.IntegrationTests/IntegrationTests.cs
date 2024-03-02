using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace CodeButler.IntegrationTests
{
    public class IntegrationTests
    {
        private const string _testCasesDir = "TestCases";

        public static TheoryData<string> TestCases()
        {
            var testCases = Directory
                .EnumerateDirectories(_testCasesDir)
                .Select(dir => dir.Split(Path.DirectorySeparatorChar)[^1]);

            var data = new TheoryData<string>();
            foreach (var testCase in testCases)
            {
                data.Add(testCase);
            }

            return data;
        }

        [Theory]
        [MemberData(nameof(TestCases))]
        public async Task DirectUseOfProgramClass(string folder)
        {
            // Arrange
            var originalPath = Path.Combine(_testCasesDir, folder, "original.cs.test");
            var cleanPath = Path.Combine(_testCasesDir, folder, "clean.cs.test");

            var testPath = Path.Combine(
                _testCasesDir,
                folder,
                $"{Guid.NewGuid().ToString()[..7]}.cs"
            );

            // Act
            File.Copy(originalPath, testPath);
            var exitCode = await Program.Main([testPath]);
            var result = await File.ReadAllTextAsync(testPath);
            File.Delete(testPath);

            // Assert
            var clean = await File.ReadAllTextAsync(cleanPath);
            exitCode.Should().Be(0);
            result.Should().Be(clean);
        }
    }
}
