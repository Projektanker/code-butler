using System;
using System.Collections.Generic;
using System.Linq;
using CodeCleaner.Reorganizing;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeCleaner.Syntax
{
    public static class MembersOrganizer
    {
        public static MemberOrderInfo GetMemberOrderInfo(MemberDeclarationSyntax memberDeclaration)
        {
            if (memberDeclaration is null)
            {
                throw new ArgumentNullException(nameof(memberDeclaration));
            }

            return new MemberOrderInfo()
            {
                Identifier = GetMemberName(memberDeclaration),
                AcessModifier = GetMemberAccessModifier(memberDeclaration),
                AdditionalModifier = GetMemberAdditionalModifier(memberDeclaration),
                MemberType = GetMemberType(memberDeclaration),
            };
        }

        public static CompilationUnitSyntax WithReorganizeMembers(this CompilationUnitSyntax root)
        {
            if (root is null)
            {
                throw new ArgumentNullException(nameof(root));
            }

            var organizedMembers = DeepReorganizeMembers(root.Members)
                .ToSyntaxList();

            return root.WithMembers(organizedMembers);
        }

        private static IEnumerable<MemberDeclarationSyntax> DeepReorganizeMembers(IEnumerable<MemberDeclarationSyntax> memberDeclarations)
        {
            var organizedMemberDeclarations = memberDeclarations
                .OrderBy(GetMemberOrderInfo)
                .Select(ReorganizeMembers);

            return organizedMemberDeclarations;
        }

        private static MemberAccessModifier GetMemberAccessModifier(MemberDeclarationSyntax memberDeclaration)
        {
            var modifierKinds = memberDeclaration.Modifiers
                .Select(token => token.Kind())
                .ToHashSet();

            if (modifierKinds.Count == 0)
            {
                return MemberAccessModifier.None;
            }
            else if (modifierKinds.Contains(SyntaxKind.PublicKeyword))
            {
                // public
                return MemberAccessModifier.Public;
            }
            else if (modifierKinds.Contains(SyntaxKind.InternalKeyword))
            {
                // (?) internal
                if (modifierKinds.Contains(SyntaxKind.ProtectedKeyword))
                {
                    // protected internal
                    return MemberAccessModifier.ProtectedInternal;
                }
                else
                {
                    // internal
                    return MemberAccessModifier.Internal;
                }
            }
            else if (modifierKinds.Contains(SyntaxKind.ProtectedKeyword))
            {
                // (?) protected (?)
                if (modifierKinds.Contains(SyntaxKind.InternalKeyword))
                {
                    // protected internal
                    return MemberAccessModifier.ProtectedInternal;
                }
                else if (modifierKinds.Contains(SyntaxKind.PrivateKeyword))
                {
                    // private protected
                    return MemberAccessModifier.PrivateProtected;
                }
                else
                {
                    // protected
                    return MemberAccessModifier.Protected;
                }
            }
            else if (modifierKinds.Contains(SyntaxKind.PrivateKeyword))
            {
                return MemberAccessModifier.Private;
            }
            else
            {
                return MemberAccessModifier.None;
            }
        }

        private static MemberAdditionalModifier GetMemberAdditionalModifier(MemberDeclarationSyntax memberDeclaration)
        {
            var modifierKinds = memberDeclaration.Modifiers
                .Select(token => token.Kind())
                .ToHashSet();

            if (modifierKinds.Count == 0)
            {
                return MemberAdditionalModifier.None;
            }
            else if (modifierKinds.Contains(SyntaxKind.ConstKeyword))
            {
                return MemberAdditionalModifier.Const;
            }
            else if (modifierKinds.Contains(SyntaxKind.StaticKeyword) && modifierKinds.Contains(SyntaxKind.ReadOnlyKeyword))
            {
                return MemberAdditionalModifier.StaticReadonly;
            }
            else if (modifierKinds.Contains(SyntaxKind.ReadOnlyKeyword))
            {
                return MemberAdditionalModifier.Readonly;
            }
            else if (modifierKinds.Contains(SyntaxKind.StaticKeyword))
            {
                return MemberAdditionalModifier.Static;
            }
            else
            {
                return MemberAdditionalModifier.None;
            }
        }

        private static string GetMemberName(MemberDeclarationSyntax memberDeclaration)
        {
            var name = memberDeclaration switch
            {
                NamespaceDeclarationSyntax declaration => declaration.Name.ToString(),
                FieldDeclarationSyntax declaration => string.Join(", ", declaration.Declaration.Variables.Select(variableDeclaration => variableDeclaration.Identifier)),
                DelegateDeclarationSyntax declaration => declaration.Identifier.ToString(),
                EventDeclarationSyntax declaration => declaration.Identifier.ToString(),
                EnumDeclarationSyntax declaration => declaration.Identifier.ToString(),
                InterfaceDeclarationSyntax declaration => declaration.Identifier.ToString(),
                PropertyDeclarationSyntax declaration => declaration.Identifier.ToString(),
                MethodDeclarationSyntax declaration => declaration.Identifier.ToString(),
                StructDeclarationSyntax declaration => declaration.Identifier.ToString(),
                ClassDeclarationSyntax declaration => declaration.Identifier.ToString(),
                _ => string.Empty,
            };

            return name;
        }

        private static MemberType GetMemberType(MemberDeclarationSyntax memberDeclaration)
        {
            SyntaxKind kind = memberDeclaration.Kind();
            return kind switch
            {
                SyntaxKind.FieldDeclaration => MemberType.Field,
                SyntaxKind.ConstructorDeclaration => MemberType.Constructor,
                SyntaxKind.DestructorDeclaration => MemberType.Destructor,
                SyntaxKind.DelegateDeclaration => MemberType.Delegate,
                SyntaxKind.EventDeclaration => MemberType.Event,
                SyntaxKind.EventFieldDeclaration => MemberType.Event,
                SyntaxKind.EnumDeclaration => MemberType.Enum,
                SyntaxKind.InterfaceDeclaration => MemberType.Interface,
                SyntaxKind.PropertyDeclaration => MemberType.Property,
                SyntaxKind.IndexerDeclaration => MemberType.Indexer,
                SyntaxKind.OperatorDeclaration => MemberType.Indexer,
                SyntaxKind.MethodDeclaration => MemberType.Method,
                SyntaxKind.StructDeclaration => MemberType.Struct,
                SyntaxKind.ClassDeclaration => MemberType.Class,
                _ => MemberType.None,
            };
        }

        private static MemberDeclarationSyntax ReorganizeMembers(MemberDeclarationSyntax memberDeclaration)
        {
            return memberDeclaration switch
            {
                NamespaceDeclarationSyntax declaration => declaration.WithReorganizeMembers(),
                InterfaceDeclarationSyntax declaration => declaration.WithReorganizeMembers(),
                ClassDeclarationSyntax declaration => declaration.WithReorganizeMembers(),
                StructDeclarationSyntax declaration => declaration.WithReorganizeMembers(),
                _ => memberDeclaration,
            };
        }

        private static NamespaceDeclarationSyntax WithReorganizeMembers(this NamespaceDeclarationSyntax declaration)
        {
            var organizedMembers = DeepReorganizeMembers(declaration.Members)
                .ToSyntaxList();

            return declaration.WithMembers(organizedMembers);
        }

        private static InterfaceDeclarationSyntax WithReorganizeMembers(this InterfaceDeclarationSyntax declaration)
        {
            var organizedMembers = DeepReorganizeMembers(declaration.Members)
                .ToSyntaxList();

            return declaration.WithMembers(organizedMembers);
        }

        private static ClassDeclarationSyntax WithReorganizeMembers(this ClassDeclarationSyntax declaration)
        {
            var organizedMembers = DeepReorganizeMembers(declaration.Members)
                .ToSyntaxList();

            return declaration.WithMembers(organizedMembers);
        }

        private static StructDeclarationSyntax WithReorganizeMembers(this StructDeclarationSyntax declaration)
        {
            var organizedMembers = DeepReorganizeMembers(declaration.Members)
                .ToSyntaxList();

            return declaration.WithMembers(organizedMembers);
        }
    }
}