using System;
using System.Collections.Generic;
using System.Linq;
using CodeButler.Reorganizing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeButler.Syntax
{

    public static class MemberSpacingExtensions
    {
        public static CompilationUnitSyntax WithCorrectSpacing(this CompilationUnitSyntax root)
        {
            var spacingRewriter = new MemberSpacingRewriter();
            return spacingRewriter.EnsurceCorrectSpacing(root);
        }
    }
}