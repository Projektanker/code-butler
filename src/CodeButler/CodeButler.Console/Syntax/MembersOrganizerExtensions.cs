using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeButler.Syntax
{
    public static class MembersOrganizerExtensions
    {
        public static CompilationUnitSyntax WithReorganizeMembers(this CompilationUnitSyntax root)
        {
            var organizer = new MembersOrganizerRewriter();
            return organizer.OrganizeMembers(root);
        }
    }
}