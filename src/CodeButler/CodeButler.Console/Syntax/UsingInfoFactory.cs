using CodeButler.Reorganizing;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeButler.Syntax;

public static class UsingInfoFactory
{
    public static UsingInfo GetUsingInfo(this UsingDirectiveSyntax usingDirective)
    {
        return new UsingInfo(usingDirective.Name.ToString())
        {
            Alias = usingDirective.Alias?.Name.ToString(),
            IsStatic = usingDirective.StaticKeyword != default,
            IsGlobal = usingDirective.GlobalKeyword != default,
        };
    }
}
