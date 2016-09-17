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

        public static bool Contains(this Rect r, Rect other)
        {
            if (r.max.x < other.max.x || r.min.x > other.min.x)
                return false;

            if (r.max.y < other.max.y || r.min.y > other.min.y)
                return false;

            return true;
        }
    }
}
