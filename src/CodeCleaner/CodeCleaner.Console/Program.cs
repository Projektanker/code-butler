using System;
using System.IO;
using System.Threading.Tasks;
using CodeCleaner.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace CodeCleaner
{
    internal enum Mode
    {
        Console,
        File,
    }

    internal class Program
    {
        public static async Task<int> Main(string[] args)
        {
            Mode mode;
            if (Console.IsInputRedirected)
            {
                mode = Mode.Console;
            }
            else if (args.Length > 0)
            {
                mode = Mode.File;
            }
            else
            {
                Console.Error.WriteLine("No input provided.");
                return -1;
            }

            string input = await GetInput(mode, args).ConfigureAwait(false);

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(input);
            CompilationUnitSyntax root = syntaxTree.GetCompilationUnitRoot();

            var organizedRoot = Reorganize(root);

            await SetOuput(organizedRoot, mode, args).ConfigureAwait(false);
            return 0;
        }

        private static async Task<string> GetInput(Mode mode, string[] args)
        {
            switch (mode)
            {
                case Mode.Console:
                    using (var reader = new StreamReader(Console.OpenStandardInput(), Console.InputEncoding))
                    {
                        return await reader.ReadToEndAsync().ConfigureAwait(false);
                    }

                case Mode.File:
                    return await File.ReadAllTextAsync(args[0]).ConfigureAwait(false);

                default:
                    throw new NotImplementedException($"Mode \"{mode}\" not implemented.");
            }
        }

        private static CompilationUnitSyntax Reorganize(CompilationUnitSyntax compilationUnit)
        {
            return compilationUnit
                .WithReorganizedUsings()
                .WithReorganizeMembers()
                .WithCorrectSpacing();
        }

        private static async Task SetOuput(CompilationUnitSyntax compilationUnit, Mode mode, string[] args)
        {
            switch (mode)
            {
                case Mode.Console:
                    Console.Write(compilationUnit.ToFullString());
                    break;

                case Mode.File:
                    await File.WriteAllTextAsync(args[0], compilationUnit.ToFullString()).ConfigureAwait(false);
                    break;

                default:
                    throw new NotImplementedException($"Mode \"{mode}\" not implemented.");
            }
        }
    }
}