using System.Collections.Generic;
using UnityEngine;

namespace Utility.ExtensionMethods
{
    public static class ExtensionMethods
    {
        public static void AppendRange<T>(this LinkedList<T> source, IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                source.AddLast(item);
            }
        }

        public static void PrependRange<T>(this LinkedList<T> source, IEnumerable<T> items)
        {
            LinkedListNode<T> first = source.First;
            foreach (T item in items)
            {
                source.AddBefore(first, item);
            }
        }

        public static void Reverse<T>(this LinkedList<T> source)
        {
            var head = source.First;
            while (head.Next != null)
            {
                var next = head.Next;
                source.Remove(next);
                source.AddFirst(next.Value);
            }
        }

        public static bool IsLayerWithinMask(this LayerMask source, int other)
        {
            return source == (source | (1 << other));
        }
    }
}
