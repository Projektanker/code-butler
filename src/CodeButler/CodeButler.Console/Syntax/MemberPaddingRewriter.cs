using System;
using System.Collections.Generic;
using System.Linq;
using CodeButler.Reorganizing;
using CodeButler.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeButler.Syntax
{
    public class MemberPaddingRewriter : CSharpSyntaxRewriter
    {
        public CompilationUnitSyntax EnsureCorrectPadding(CompilationUnitSyntax compilationUnit)
        {
            if (compilationUnit is null)
            {
                throw new ArgumentNullException(nameof(compilationUnit));
            }

            var members = EnsureCorrectPadding(compilationUnit.Members, forceFirstMemberHasLeadingEndOfLine: compilationUnit.Usings.Count > 0)
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

            var members = EnsureCorrectPadding(node.Members)
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

            var members = EnsureCorrectPadding(node.Members)
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

            var members = EnsureCorrectPadding(node.Members)
              .ToSyntaxList();

            return node.WithMembers(members);
        }

        /// <inheritdoc/>
        public override SyntaxNode? VisitStructDeclaration(StructDeclarationSyntax node)
        {
            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            var members = EnsureCorrectPadding(node.Members)
                .ToSyntaxList();

            return node.WithMembers(members);
        }

        private static void CleanTrivia(ref List<SyntaxTrivia> leadingTrivia)
        {
            // Remove whitespace before end of line
            int i = 0;
            while (i < leadingTrivia.Count - 1)
            {
                var current = leadingTrivia[i];
                var next = leadingTrivia[i + 1];
                if (current.IsKind(SyntaxKind.WhitespaceTrivia)
                    && next.IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    leadingTrivia.RemoveAt(i);
                }

                i++;
            }

            // Remove all duplicate leading end of line
            i = 0;
            while (i < leadingTrivia.Count - 1)
            {
                var current = leadingTrivia[i];
                var next = leadingTrivia[i + 1];
                if (current.IsKind(SyntaxKind.EndOfLineTrivia)
                    && next.IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    leadingTrivia.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }

            // Remove all leading end of line
            while (leadingTrivia.Count > 0 && leadingTrivia[0].IsKind(SyntaxKind.EndOfLineTrivia))
            {
                leadingTrivia.RemoveAt(0);
            }
        }

        private static MemberDeclarationSyntax EnsureCorrectPadding(LinkedListNode<MemberDeclarationSyntax> node, bool forceLeadingEndOfLine)
        {
            var leadingTrivia = new List<SyntaxTrivia>(node.Value.GetLeadingTrivia());

            CleanTrivia(ref leadingTrivia);

            bool shouldHaveLeadingEndOfLine = forceLeadingEndOfLine || ShouldHaveLeadingEndOfLine(node);

            if (shouldHaveLeadingEndOfLine
                && !leadingTrivia.FirstOrDefault().IsKind(SyntaxKind.EndOfLineTrivia))
            {
                // Insert a leading end of line
                leadingTrivia.Insert(0, SyntaxFactory.CarriageReturnLineFeed);
            }
            else if (!shouldHaveLeadingEndOfLine
                && leadingTrivia.FirstOrDefault().IsKind(SyntaxKind.EndOfLineTrivia))
            {
                // Remove a leading end of line
                leadingTrivia.RemoveAt(0);
            }

            return node.Value.WithLeadingTrivia(leadingTrivia);
        }

        private static bool HasTriviaJustifyingLeadingEndOfLine(MemberDeclarationSyntax? memberDeclaration)
        {
            return memberDeclaration is not null
                && memberDeclaration
                    .GetLeadingTrivia()
                    .Any(trivia => !(trivia.IsKind(SyntaxKind.WhitespaceTrivia) || trivia.IsKind(SyntaxKind.EndOfLineTrivia)));
        }

        private static bool ShouldHaveLeadingEndOfLine(LinkedListNode<MemberDeclarationSyntax> node)
        {
            if (node.Previous is null)
            {
                // First member
                return false;
            }
            else if (HasTriviaJustifyingLeadingEndOfLine(node.Value))
            {
                // At least one trivia justifying a leading blank line exists.
                return true;
            }
            else if (HasTriviaJustifyingLeadingEndOfLine(node.Previous.Value))
            {
                // Previous member had at least one trivia justifying a leading blank line.
                return true;
            }
            else if (node.Value.GetMemberOrderInfo().MemberType == MemberType.Field)
            {
                // Do not insert blank line between fields (except the ones having justifying trivia)
                return false;
            }
            else
            {
                return true;
            }
        }

        private IEnumerable<MemberDeclarationSyntax> EnsureCorrectPadding(IEnumerable<MemberDeclarationSyntax> memberDeclarations, bool forceFirstMemberHasLeadingEndOfLine = false)
        {
            var linkedMemberDeclarations = new LinkedList<MemberDeclarationSyntax>(memberDeclarations
                .Select(member => member.Accept(this))
                .Where(member => member is not null)
                .Cast<MemberDeclarationSyntax>());

            if (linkedMemberDeclarations.Count == 0)
            {
                return linkedMemberDeclarations;
            }

            var forceLeadingEndOfLineFlags = Enumerable
                .Repeat(false, linkedMemberDeclarations.Count - 1)
                .Prepend(forceFirstMemberHasLeadingEndOfLine);

            var membersWithCorrectSpacing = linkedMemberDeclarations
                .EnumerateLinkedListNodes()
                .Zip(forceLeadingEndOfLineFlags, (node, forceLeadingEndOfLine) => (node, forceLeadingEndOfLine))
                .Select(zipped => EnsureCorrectPadding(zipped.node, zipped.forceLeadingEndOfLine));

            return membersWithCorrectSpacing;
        }
    }
}