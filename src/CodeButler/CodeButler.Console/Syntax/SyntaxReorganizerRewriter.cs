using System.Collections.Generic;
using System.Linq;
using CodeButler.Reorganizing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeButler.Syntax;

public class SyntaxReorganizerRewriter : CSharpSyntaxRewriter
{
    private MemberInfoComparer _memberInfoComparer;

    public SyntaxReorganizerRewriter(MemberSortConfiguration memberSortConfiguration)
    {
        _memberInfoComparer = new(memberSortConfiguration);
    }

    public override SyntaxNode? VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        var members = OrganizeMembers(node.Members);
        return node.WithMembers(members);
    }

    public override SyntaxNode? VisitCompilationUnit(CompilationUnitSyntax node)
    {
        var usings = OrganizeUsings(node.Usings);
        var members = OrganizeMembers(node.Members);
        return node.WithUsings(usings).WithMembers(members);
    }

    public override SyntaxNode? VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
    {
        var members = OrganizeMembers(node.Members);
        return node.WithMembers(members);
    }

    public override SyntaxNode? VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
    {
        var usings = OrganizeUsings(node.Usings);
        var members = OrganizeMembers(node.Members);
        return node.WithUsings(usings).WithMembers(members);
    }

    public override SyntaxNode? VisitStructDeclaration(StructDeclarationSyntax node)
    {
        var members = OrganizeMembers(node.Members);
        return node.WithMembers(members);
    }

    private static SyntaxList<UsingDirectiveSyntax> OrganizeUsings(
        IEnumerable<UsingDirectiveSyntax> usingDirectives
    )
    {
        var sorted = usingDirectives.OrderBy(UsingInfoFactory.GetUsingInfo).ToSyntaxList();
        return sorted;
    }

    private SyntaxList<MemberDeclarationSyntax> OrganizeMembers(
        IEnumerable<MemberDeclarationSyntax> memberDeclarations
    )
    {
        return memberDeclarations
            .Select(member => member.Accept(this))
            .OfType<MemberDeclarationSyntax>()
            .OrderBy(MemberInfoFactory.GetMemberInfo, _memberInfoComparer)
            .ToSyntaxList();
    }
}
