using System;
using System.Collections.Generic;

namespace CodeCleaner.Utilities
{
    public static class LinkedListExtensions
    {
        public static IEnumerable<LinkedListNode<T>> EnumerateLinkedListNodes<T>(this LinkedList<T> linkedList)
        {
            if (linkedList is null)
            {
                throw new ArgumentNullException(nameof(linkedList));
            }

            for (var node = linkedList.First; node is not null; node = node.Next)
            {
                yield return node;
            }
        }
    }
}