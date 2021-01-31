using System.Linq;
using System.Text.RegularExpressions;

namespace CodeButler.Padding
{
    public class PaddingCleaner
    {
        private static readonly Regex _dirtyNewLine = new Regex(@"[\t ]+(\r\n|\n)", RegexOptions.Multiline);
        private static readonly Regex _multiEmptyLine = new Regex(@"^(\r\n|\n)+", RegexOptions.Multiline);

        public string Clean(string input)
        {
            string output = _dirtyNewLine.Replace(input, match => match.Groups.Values.Last().Value);
            output = _multiEmptyLine.Replace(output, match => match.Groups.Values.Last().Value);
            return output;
        }
    }
}