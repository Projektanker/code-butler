using System;
using System.Linq;
using CodeButler.Reorganizing;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeButler.Syntax;

public static class MemberInfoFactory
{
    public static MemberInfo GetMemberInfo(this MemberDeclarationSyntax memberDeclaration)
    {
        ArgumentNullException.ThrowIfNull(memberDeclaration);

        return new MemberInfo()
        {
            Identifier = GetMemberName(memberDeclaration),
            AccessModifier = GetMemberAccessModifier(memberDeclaration),
            AdditionalModifier = GetMemberAdditionalModifier(memberDeclaration),
            MemberType = GetMemberType(memberDeclaration),
        };
    }

    private static string GetMemberName(MemberDeclarationSyntax memberDeclaration)
    {
        var name = memberDeclaration switch
        {
            NamespaceDeclarationSyntax declaration => declaration.Name.ToString(),
            FieldDeclarationSyntax declaration
                => string.Join(
                    ", ",
                    declaration.Declaration.Variables.Select(variableDeclaration =>
                        variableDeclaration.Identifier
                    )
                ),
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

    private static MemberAccessModifier GetMemberAccessModifier(
        MemberDeclarationSyntax memberDeclaration
    )
    {
        var modifierKinds = memberDeclaration.Modifiers.Select(token => token.Kind()).ToHashSet();

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

    private static MemberAdditionalModifier GetMemberAdditionalModifier(
        MemberDeclarationSyntax memberDeclaration
    )
    {
        var modifierKinds = memberDeclaration.Modifiers.Select(token => token.Kind()).ToHashSet();

        if (modifierKinds.Count == 0)
        {
            return MemberAdditionalModifier.None;
        }
        else if (modifierKinds.Contains(SyntaxKind.ConstKeyword))
        {
            return MemberAdditionalModifier.Const;
        }
        else if (
            modifierKinds.Contains(SyntaxKind.StaticKeyword)
            && modifierKinds.Contains(SyntaxKind.ReadOnlyKeyword)
        )
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
}
