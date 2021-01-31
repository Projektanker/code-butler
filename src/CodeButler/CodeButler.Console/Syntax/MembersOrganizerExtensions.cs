using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeButler.Syntax
{
    public static class MembersOrganizerExtensions
    {
        public static CompilationUnitSyntax Reorganize(this CompilationUnitSyntax root)
        {
            var organizer = new SyntaxReorganizerRewriter();
            return root.Accept(organizer) is CompilationUnitSyntax reorganizedRoot
                ? reorganizedRoot
                : throw new System.Exception("Reorganized root is null or not of type CompilationUnitSyntax.");
        }
    }
}