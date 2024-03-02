using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace CodeButler.Syntax;

public static class SyntaxListExtensions
{
    public static SyntaxList<TNode> ToSyntaxList<TNode>(this IEnumerable<TNode> nodes)
        where TNode : SyntaxNode
    {
        return new SyntaxList<TNode>(nodes);
    }
}
