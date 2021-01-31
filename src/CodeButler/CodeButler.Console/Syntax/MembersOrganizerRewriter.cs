using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeButler.Syntax
{
    public class MembersOrganizerRewriter : CSharpSyntaxRewriter
    {
        public CompilationUnitSyntax OrganizeMembers(CompilationUnitSyntax compilationUnit)
        {
            if (compilationUnit is null)
            {
                throw new ArgumentNullException(nameof(compilationUnit));
            }

            var members = OrganizeMembers(compilationUnit.Members)
                .ToSyntaxList();

            return compilationUnit.WithMembers(members);
        }

        /// <inheritdoc/>
        public override SyntaxNode? VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            var members = OrganizeMembers(node.Members)
                .ToSyntaxList();

            return node.WithMembers(members);
        }

        /// <inheritdoc/>
        public override SyntaxNode? VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            var members = OrganizeMembers(node.Members)
                .ToSyntaxList();

            return node.WithMembers(members);
        }

        /// <inheritdoc/>
        public override SyntaxNode? VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            var members = OrganizeMembers(node.Members)
              .ToSyntaxList();

            return node
                .WithReorganizedUsings()
                .WithMembers(members);
        }

        /// <inheritdoc/>
        public override SyntaxNode? VisitStructDeclaration(StructDeclarationSyntax node)
        {
            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            var members = OrganizeMembers(node.Members)
                .ToSyntaxList();

            return node.WithMembers(members);
        }

        private IEnumerable<MemberDeclarationSyntax> OrganizeMembers(IEnumerable<MemberDeclarationSyntax> memberDeclarations)
        {
            return memberDeclarations
                .Select(member => member.Accept(this))
                .Where(member => member is not null)
                .Cast<MemberDeclarationSyntax>()
                .OrderBy(MemberOrderInfoExtensions.GetMemberOrderInfo);
        }
    }
}