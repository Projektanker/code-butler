using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeButler.Syntax
{
    public static class MemberPaddingExtensions
    {
        public static CompilationUnitSyntax WithCorrectPadding(this CompilationUnitSyntax root)
        {
            var spacingRewriter = new MemberPaddingRewriter();
            return spacingRewriter.EnsureCorrectPadding(root);
        }
    }
}