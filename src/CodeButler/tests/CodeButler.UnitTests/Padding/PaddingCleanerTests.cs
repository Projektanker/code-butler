using CodeButler.Padding;
using FluentAssertions;
using Xunit;

namespace CodeButler.UnitTests.Padding
{
    public class PaddingCleanerTests
    {
        [Theory]
        [InlineData("    \n", "\n")]
        [InlineData("    \n    \n", "\n")]
        [InlineData("    \r\n    \r\n", "\r\n")]
        [InlineData("\n\n\n", "\n")]
        [InlineData("\r\n\r\n\r\n", "\r\n")]
        [InlineData("    // Comment\n\n    public void ...", "    // Comment\n\n    public void ...")]
        [InlineData("    // Comment\r\n\r\n    public void ...", "    // Comment\r\n\r\n    public void ...")]
        [InlineData("    // Comment\n   \n   \n    public void ...", "    // Comment\n\n    public void ...")]
        public void InputShouldBeCleaned(string input, string expected)
        {
            var paddingCleaner = new PaddingCleaner();
            string output = paddingCleaner.Clean(input);

            output.Should().Be(expected);
        }
    }
}