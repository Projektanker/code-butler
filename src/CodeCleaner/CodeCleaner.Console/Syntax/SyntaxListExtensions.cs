using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCleaner.Reorganizing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeCleaner.Syntax
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