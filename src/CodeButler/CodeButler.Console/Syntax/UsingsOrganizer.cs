using System;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeButler.Syntax
{
    public static class UsingsOrganizer
    {
        public static CompilationUnitSyntax WithReorganizedUsings(this CompilationUnitSyntax root)
        {
            if (root is null)
            {
                throw new ArgumentNullException(nameof(root));
            }

            var organizedUsingDeclarations = root.Usings
                .OrderBy(UsingOrderInfoExtensions.GetUsingOrderInfo)
                .ToSyntaxList();

            return root.WithUsings(organizedUsingDeclarations);
        }

        public static NamespaceDeclarationSyntax WithReorganizedUsings(this NamespaceDeclarationSyntax root)
        {
            if (root is null)
            {
                throw new ArgumentNullException(nameof(root));
            }

            var organizedUsingDeclarations = root.Usings
                .OrderBy(UsingOrderInfoExtensions.GetUsingOrderInfo)
                .ToSyntaxList();

            return root.WithUsings(organizedUsingDeclarations);
        }
    }
}