using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeCleaner.Reorganizing;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeCleaner.Syntax
{
    public static class UsingsOrganizer
    {
        public static UsingOrderInfo GetUsingOrderInfo(UsingDirectiveSyntax usingDirective)
        {
            if (usingDirective is null)
            {
                throw new ArgumentNullException(nameof(usingDirective));
            }

            return new UsingOrderInfo(usingDirective.Name.ToString())
            {
                Alias = usingDirective.Alias?.Name.ToString(),
                IsStatic = usingDirective.StaticKeyword != default,
            };
        }

        public static CompilationUnitSyntax WithReorganizedUsings(this CompilationUnitSyntax root)
        {
            if (root is null)
            {
                throw new ArgumentNullException(nameof(root));
            }

            var organizedUsingDeclarations = root.Usings
                .OrderBy(GetUsingOrderInfo)
                .ToSyntaxList();

            return root.WithUsings(organizedUsingDeclarations);
        }
    }
}