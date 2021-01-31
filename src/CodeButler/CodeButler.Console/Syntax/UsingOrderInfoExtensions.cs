using System;
using CodeButler.Reorganizing;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeButler.Syntax
{
    public static class UsingOrderInfoExtensions
    {
        public static UsingOrderInfo GetUsingOrderInfo(this UsingDirectiveSyntax usingDirective)
        {
            return new UsingOrderInfo(usingDirective.Name.ToString())
            {
                Alias = usingDirective.Alias?.Name.ToString(),
                IsStatic = usingDirective.StaticKeyword != default,
            };
        }
    }
}