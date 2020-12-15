using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeButler.Reorganizing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeButler.Syntax
{
    public static class SyntaxListExtensions
    {
        public static SyntaxList<TNode> ToSyntaxList<TNode>(this IEnumerable<TNode> nodes)
            where TNode : SyntaxNode
        {
            return new SyntaxList<TNode>(nodes);
        }
    }
}