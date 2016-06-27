using UnityEngine;
using System.Collections.Generic;
using Utility.ExtensionMethods;

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
            openChains.Add(new PointChain(ref p0, ref p1));
        }

        public PointChain[] ToArray()
        {
            return closedChains.ToArray();
        }
    }

    public class PointChain
    {
        public bool IsClosed { get; set; }
        public bool IsEmpty { get { return chain.Count == 0; } }
        public Bounds Bounds { get { if (!areBoundsValid) CalcBounds(); return bounds; } set { bounds = value; areBoundsValid = true; } }

        public Vector2 FirstPoint
        {
            get { return chain.First.Value; }
        }
        public Vector2 LastPoint
        {
            get { return chain.Last.Value; }
        }
        public LinkedList<Vector2> chain;

        private bool areBoundsValid;
        private Bounds bounds;

        public PointChain(ref Vector2 p0, ref Vector2 p1, bool isClosed = false)
        {
            chain = new LinkedList<Vector2>();
            chain.AddLast(p0);
            chain.AddLast(p1);
            IsClosed = isClosed;
        }

        public PointChain(IEnumerable<Vector2> verts, bool isClosed = false)
        {
            chain = new LinkedList<Vector2>(verts);

            if (chain.Count < 2)
                throw new System.ArgumentOutOfRangeException("verts", "Verts must contain at least two items.");
            IsClosed = isClosed;
        }

        public bool LinkSegment(ref Vector2 p0, ref Vector2 p1)
        {
            if (p0 == FirstPoint)
            {
                if (p1 == LastPoint)
                    IsClosed = true;
                else
                    chain.AddFirst(p1);

                //Update bounds
                bounds.min = Vector2.Min(bounds.min, p1);
                bounds.max = Vector2.Max(bounds.max, p1);
                return true;
            }
            if (p1 == LastPoint)
            {
                if (p0 == FirstPoint)
                    IsClosed = true;
                else
                    chain.AddLast(p0);

                //Update bounds
                bounds.min = Vector2.Min(bounds.min, p0);
                bounds.max = Vector2.Max(bounds.max, p0);
                return true;
            }
            if (p1 == FirstPoint)
            {
                if (p0 == LastPoint)
                    IsClosed = true;
                else
                    chain.AddFirst(p0);

                //Update bounds
                bounds.min = Vector2.Min(bounds.min, p0);
                bounds.max = Vector2.Max(bounds.max, p0);
                return true;
            }
            if (p0 == LastPoint)
            {
                if (p1 == FirstPoint)
                    IsClosed = true;
                else
                    chain.AddLast(p1);

                //Update bounds
                bounds.min = Vector2.Min(bounds.min, p1);
                bounds.max = Vector2.Max(bounds.max, p1);
                return true;
            }
            return false;
        }

        public bool LinkPointChain(PointChain other)
        {
            if (other.FirstPoint == LastPoint)
            {
                other.chain.RemoveFirst();
                chain.AppendRange(other.chain);
            }
            else if (other.LastPoint == FirstPoint)
            {
                chain.RemoveFirst();
                chain.PrependRange(other.chain);
            }
            else if (other.FirstPoint == FirstPoint)
            {
                chain.RemoveFirst();
                other.chain.Reverse();
                chain.PrependRange(other.chain);
            }
            else if (other.LastPoint == LastPoint)
            {
                chain.RemoveLast();
                other.chain.Reverse();
                chain.AppendRange(other.chain);
            }
            else
                return false; //Other PointChain couldnt be attached

            //Update bounds
            bounds.min = Vector2.Min(bounds.min, other.Bounds.min);
            bounds.max = Vector2.Max(bounds.max, other.Bounds.max);
            return true;
        }

        private void CalcBounds()
        {
            areBoundsValid = true;

            LinkedListNode<Vector2> cNode = chain.First;
            bounds.min = cNode.Value;
            bounds.max = cNode.Value;
            while ((cNode = cNode.Next) != null)
            {
                bounds.min = Vector2.Min(cNode.Value, bounds.min);
                bounds.max = Vector2.Max(cNode.Value, bounds.max);
            }
        }
    }
}
