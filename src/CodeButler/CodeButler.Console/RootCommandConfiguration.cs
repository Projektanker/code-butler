using System.CommandLine;
using System.IO;

namespace CodeButler;

public class RootCommandConfiguration
{
    public bool SortMemberByAlphabet { get; init; }
    public Mode Mode { get; init; }
    public FileInfo? File { get; init; }
    public IConsole Console { get; init; } = null!;
}
