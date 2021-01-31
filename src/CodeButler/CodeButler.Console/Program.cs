using System;
using System.IO;
using System.Threading.Tasks;
using CodeButler.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeButler
{
    public class Program
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
            CompilationUnitSyntax root = Parse(input);
            var organizedRoot = Reorganize(root);

            await SetOutput(organizedRoot, mode, args).ConfigureAwait(false);
            return 0;
        }

        public static CompilationUnitSyntax Parse(string input)
        {
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(input);
            CompilationUnitSyntax root = syntaxTree.GetCompilationUnitRoot();
            return root;
        }

        public static CompilationUnitSyntax Reorganize(CompilationUnitSyntax compilationUnit)
        {
            return compilationUnit
                .WithReorganizedUsings()
                .WithReorganizeMembers()
                .WithCorrectPadding();
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

        private static async Task SetOutput(CompilationUnitSyntax compilationUnit, Mode mode, string[] args)
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