using System;
using CodeButler.Reorganizing;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeButler.Syntax
{
    public static class MembersOrganizerExtensions
    {
        public static CompilationUnitSyntax Reorganize(
            this CompilationUnitSyntax root,
            MemberSortConfiguration memberSortConfiguration
        )
        {
            var organizer = new SyntaxReorganizerRewriter(memberSortConfiguration);
            return root.Accept(organizer) as CompilationUnitSyntax
                ?? throw new Exception(
                    $"Reorganized root is null or not of type {typeof(CompilationUnitSyntax)}."
                );
        }
    }
}
