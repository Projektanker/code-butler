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
        var input = await ReadInput(configuration).ConfigureAwait(false);

        var clean = Clean(input);
        var root = Parse(clean);
        var organizedRoot = Reorganize(root, configuration);

        await WriteOutput(organizedRoot, configuration).ConfigureAwait(false);
    }

    private static string Clean(string input)
    {
        var paddingCleaner = new PaddingCleaner();
        return paddingCleaner.Clean(input);
    }

    private static CompilationUnitSyntax Parse(string input)
    {
        var paddingCleaner = new PaddingCleaner();
        var cleanInput = paddingCleaner.Clean(input);
        var syntaxTree = CSharpSyntaxTree.ParseText(cleanInput);
        var root = syntaxTree.GetCompilationUnitRoot();
        return root;
    }

    private static CompilationUnitSyntax Reorganize(
        CompilationUnitSyntax compilationUnit,
        RootCommandConfiguration configuration
    )
    {
        var comparer = new MemberInfoComparer(
            new MemberSortConfiguration { SortByAlphabet = configuration.SortMemberByAlphabet }
        );

        var organizer = new SyntaxReorganizerRewriter(comparer);
        return compilationUnit.Accept(organizer) as CompilationUnitSyntax
            ?? throw new Exception(
                $"Reorganized root is null or not of type {typeof(CompilationUnitSyntax)}."
            );
    }

    private static async Task<string> ReadInput(RootCommandConfiguration configuration)
    {
        switch (configuration.Mode)
        {
            case InputOutputMode.Console:
                using (
                    var reader = new StreamReader(
                        Console.OpenStandardInput(),
                        Console.InputEncoding
                    )
                )
                {
                    return await reader.ReadToEndAsync().ConfigureAwait(false);
                }

            case InputOutputMode.File:
                return await File.ReadAllTextAsync(configuration.File!.FullName)
                    .ConfigureAwait(false);

            default:
                throw new NotImplementedException(
                    $"Mode \"{configuration.Mode}\" not implemented."
                );
        }
    }

    private static async Task WriteOutput(
        CompilationUnitSyntax compilationUnit,
        RootCommandConfiguration configuration
    )
    {
        switch (configuration.Mode)
        {
            case InputOutputMode.Console:
                configuration.Console.Write(compilationUnit.ToFullString());
                break;

            case InputOutputMode.File:
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
