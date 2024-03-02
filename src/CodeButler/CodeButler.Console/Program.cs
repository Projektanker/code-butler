using System;
using System.CommandLine;
using System.IO;
using System.Threading.Tasks;

namespace CodeButler;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("Reorganises, sorts and cleans up the provided C# file.");

        var noSortMemberByAlphabetOption = new Option<bool>(
            name: "--no-sort-member-by-alphabet",
            description: "Disables sorting members by alphabet.",
            getDefaultValue: () => false
        );

        var inputFileArg = Console.IsInputRedirected
            ? null
            : new Argument<FileInfo>(
                name: "Input",
                description: "Path to input file or piped input."
            );

        rootCommand.AddOption(noSortMemberByAlphabetOption);
        if (inputFileArg is not null)
        {
            rootCommand.AddArgument(inputFileArg);
        }

        rootCommand.SetHandler(
            RootCommandHandler.Handle,
            new RootCommandConfigurationBinder(noSortMemberByAlphabetOption, inputFileArg)
        );

        var exitCode = await rootCommand.InvokeAsync(args);
        return exitCode;
    }
}
