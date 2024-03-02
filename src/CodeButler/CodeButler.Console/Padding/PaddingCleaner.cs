using System.Text.RegularExpressions;

namespace CodeButler.Padding;

public partial class PaddingCleaner
{
    private static readonly Regex _dirtyNewLine = GenerateDirtyNewLineRegex();
    private static readonly Regex _multiEmptyLine = GenerateMultiEmptyLineRegex();

    public virtual string Clean(string input)
    {
        string output = _dirtyNewLine.Replace(input, match => match.Groups[^1].Value);
        output = _multiEmptyLine.Replace(output, match => match.Groups[^1].Value);
        return output;
    }

    [GeneratedRegex(@"[\t ]+(\r\n|\n)", RegexOptions.Multiline)]
    private static partial Regex GenerateDirtyNewLineRegex();

    [GeneratedRegex(@"^(\r\n|\n)+", RegexOptions.Multiline)]
    private static partial Regex GenerateMultiEmptyLineRegex();
}
