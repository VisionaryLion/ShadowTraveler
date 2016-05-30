using UnityEngine;
using System.Collections.Generic;

namespace Polygon2D
{
    public class Connector
    {
        List<PointChain> openChains;
        List<PointChain> closedChains;

        public Connector(int capacity)
        {
            openChains = new List<PointChain>(capacity);
            closedChains = new List<PointChain>(capacity);
        }

        public void Add(Vector2 p0, Vector2 p1)
        {
            Debug.Log("Try to add " + p0 + " and " + p1);
            for (int i = 0; i < openChains.Count; i++)
            {
                PointChain c = openChains[i];
                if (c.LinkSegment(ref p0, ref p1))
                {
                    if (c.IsClosed)
                    {
                        closedChains.Add(c);
                        openChains.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        for (int k = i + 1; k < openChains.Count; k++)
                        {
                            if (openChains[k].LinkPointChain(c))
                            {
                                openChains.RemoveAt(i);
                                i--;
                                break;
                            }
                        }
                    }
                    return;
                }
            }
            Debug.Log("Created new Chain!");
            openChains.Add(new PointChain(ref p0, ref p1));
        }

        public Polygon ToPolygon()
        {
            Polygon result = new Polygon();
            for (int i = 0; i < closedChains.Count; i++)
            {
                Vector2[] verts = new Vector2[closedChains[i].chain.Count];
                closedChains[i].chain.CopyTo(verts, 0);
                result.AddContour(new Contour(verts));
            }
            return result;
        }
    }

    class PointChain
    {
        public bool IsClosed { get; set; }
        public Vector2 Front {
            get { return chain.First.Value; }
        }
        public Vector2 Back
        {
            get { return chain.Last.Value; }
        }
        public LinkedList<Vector2> chain;

        public PointChain(ref Vector2 p0, ref Vector2 p1)
        {
            chain = new LinkedList<Vector2>();
            chain.AddLast(p0);
            chain.AddLast(p1);
        }


        {
            Vector2 front = Front;
            Vector2 back = Back;
            if (p0 == front)
            {
                if (p1 == back)
                    IsClosed = true;
                else
                    chain.AddFirst(p1);
                return true;
            }
            if (p1 == back)
            {
                if (p0 == front)
                    IsClosed = true;
                else
                    chain.AddLast(p0);
                return true;
            }
            if (p1 == front)
            {
                if (p0 == back)
                    IsClosed = true;
                else
                    chain.AddFirst(p0);
                return true;
            }
            if (p0 == back)
            {
                if (p1 == front)
                    IsClosed = true;
                else
                    chain.AddLast(p1);
                return true;
            }
            return false;
        }

        public bool LinkPointChain(PointChain other)
        {
            Vector2 front = Front;
            Vector2 back = Back;
            Vector2 otherFront = other.Front;
            Vector2 otherBack = other.Back;
            if (otherFront == back)
            {
                other.chain.RemoveFirst();
                AppendRange(chain, other.chain);
                return true;
            }
            if (otherBack == front)
            {
                chain.RemoveFirst();
                PrependRange(chain, other.chain);
                return true;
            }
            if (otherFront == front)
            {
                chain.RemoveFirst();
                Reverse(other.chain);
                PrependRange(chain, other.chain);
                return true;
            }
            if (otherBack == back)
            {
                chain.RemoveLast();
                Reverse(other.chain);
                AppendRange(chain, other.chain);
                return true;
            }
            return false;
        }

        public static void AppendRange<T>(LinkedList<T> source,
                                      IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                source.AddLast(item);
            }
        }

        public static void PrependRange<T>(LinkedList<T> source,
                                           IEnumerable<T> items)
        {
            LinkedListNode<T> first = source.First;
            foreach (T item in items)
            {
                source.AddBefore(first, item);
            }
        }

        public static void Reverse<T>(LinkedList<T> source)
        {
            var head = source.First;
            while (head.Next != null)
            {
                var next = head.Next;
                source.Remove(next);
                source.AddFirst(next.Value);
            }
        }
    }
}
