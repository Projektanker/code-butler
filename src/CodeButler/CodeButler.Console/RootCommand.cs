using System;
using System.CommandLine;
using System.IO;
using System.Threading.Tasks;
using CodeButler.Padding;
using CodeButler.Reorganizing;
using CodeButler.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeButler;

public class RootCommandHandler
{
    public static async Task Handle(RootCommandConfiguration configuration)
    {
        var input = await GetInput(configuration).ConfigureAwait(false);
        CompilationUnitSyntax root = Parse(input);
        var organizedRoot = Reorganize(
            root,
            new MemberSortConfiguration { SortByAlphabet = configuration.SortMemberByAlphabet }
        );

        await SetOutput(organizedRoot, configuration).ConfigureAwait(false);
    }

    public static CompilationUnitSyntax Parse(string input)
    {
        var paddingCleaner = new PaddingCleaner();
        var cleanInput = paddingCleaner.Clean(input);
        var syntaxTree = CSharpSyntaxTree.ParseText(cleanInput);
        var root = syntaxTree.GetCompilationUnitRoot();
        return root;
    }

    public static CompilationUnitSyntax Reorganize(
        CompilationUnitSyntax compilationUnit,
        MemberSortConfiguration memberSortConfiguration
    )
    {
        return compilationUnit.Reorganize(memberSortConfiguration);
    }

    private static async Task<string> GetInput(RootCommandConfiguration configuration)
    {
        switch (configuration.Mode)
        {
            case Mode.Console:
                using (
                    var reader = new StreamReader(
                        Console.OpenStandardInput(),
                        Console.InputEncoding
                    )
                )
                {
                    return await reader.ReadToEndAsync().ConfigureAwait(false);
                }

            case Mode.File:
                return await File.ReadAllTextAsync(configuration.File!.FullName)
                    .ConfigureAwait(false);

            default:
                throw new NotImplementedException(
                    $"Mode \"{configuration.Mode}\" not implemented."
                );
        }
    }

    private static async Task SetOutput(
        CompilationUnitSyntax compilationUnit,
        RootCommandConfiguration configuration
    )
    {
        switch (configuration.Mode)
        {
            case Mode.Console:
                configuration.Console.Write(compilationUnit.ToFullString());
                break;

            case Mode.File:
                await File.WriteAllTextAsync(
                        configuration.File!.FullName,
                        compilationUnit.ToFullString()
                    )
                    .ConfigureAwait(false);
                break;

            default:
                throw new NotImplementedException(
                    $"Mode \"{configuration.Mode}\" not implemented."
                );
        }
    }
}
