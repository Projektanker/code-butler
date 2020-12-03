using System;
using System.IO;
using System.Threading.Tasks;
using CodeCleaner.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeCleaner
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            string file = await File.ReadAllTextAsync(args[0]).ConfigureAwait(false);
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(file);
            CompilationUnitSyntax root = syntaxTree.GetCompilationUnitRoot();
            var organizedRoot = root
                .WithReorganizedUsings()
                .WithReorganizeMembers();

            Console.WriteLine(organizedRoot);
        }
    }
}