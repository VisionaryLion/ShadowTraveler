using UnityEngine;
using System.Collections.Generic;
using Utility.ExtensionMethods;
using System;

namespace Utility.Polygon2D
{
    internal class PointChain
    {
        const double fudge = 0.0001;

        public bool IsClosed { get; set; }
        public bool IsEmpty { get { return chain.Count == 0; } }
        public Boundsd Bounds { get { if (!areBoundsValid) CalcBounds(); return bounds; } set { bounds = value; areBoundsValid = true; } }

        public Vector2d FirstPoint
        {
            get { return chain.First.Value; }
        }
        public Vector2d LastPoint
        {
            get { return chain.Last.Value; }
        }
        public LinkedList<Vector2d> chain;

        private bool areBoundsValid;
        private Boundsd bounds;

        public PointChain(ref Vector2d p0, ref Vector2d p1, bool isClosed = false)
        {
            chain = new LinkedList<Vector2d>();
            chain.AddLast(p0);
            chain.AddLast(p1);
            IsClosed = isClosed;
        }

        public PointChain(IEnumerable<Vector2d> verts, bool isClosed = false)
        {
            chain = new LinkedList<Vector2d>(verts);
            if (chain.Count < 2)
                throw new System.ArgumentOutOfRangeException("verts", "Verts must contain at least two items.");

            IsClosed = isClosed;
        }

        public bool LinkSegment(ref Vector2d p0, ref Vector2d p1)
        {
            /*if (p0 == FirstPoint)
            {
                if (p1 == LastPoint)
                    IsClosed = true;
                else
                {
                    chain.AddFirst(p1);

                    //Update bounds
                    bounds.min = Vector2.Min(bounds.min, p1);
                    bounds.max = Vector2.Max(bounds.max, p1);
                }
                return true;
            }
            if (p1 == LastPoint)
            {
                if (p0 == FirstPoint)
                    IsClosed = true;
                else
                {
                    chain.AddLast(p0);

                    //Update bounds
                    bounds.min = Vector2.Min(bounds.min, p0);
                    bounds.max = Vector2.Max(bounds.max, p0);
                }
                return true;
            }*/
            if (p1 == FirstPoint)
            {
                if (p0 == LastPoint)
                    IsClosed = true;
                else
                {
                    chain.AddFirst(p0);

                    //Update bounds
                    bounds.min = Vector2d.Min(bounds.min, p0);
                    bounds.max = Vector2d.Max(bounds.max, p0);
                }
                return true;
            }
            if (p0 == LastPoint)
            {
                if (p1 == FirstPoint)
                    IsClosed = true;
                else
                {
                    chain.AddLast(p1);

                    //Update bounds
                    bounds.min = Vector2d.Min(bounds.min, p1);
                    bounds.max = Vector2d.Max(bounds.max, p1);
                }
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
            /*else if (other.FirstPoint == FirstPoint)
            {
                Debug.Log("Shouldn't happen (firstPoint == Firstpoint) and will lead to a wrapping issue!");
                chain.RemoveFirst();
                other.chain.Reverse();
                chain.PrependRange(other.chain);
            }
            else if (other.LastPoint == LastPoint)
            {
                Debug.Log("Shouldn't happen (LastPoint == LastPoint) and will lead to a wrapping issue!");
                chain.RemoveLast();
                other.chain.Reverse();
                chain.AppendRange(other.chain);
            }*/
            else
                return false; //Other PointChain couldnt be attached

            //Update bounds
            bounds.min = Vector2d.Min(bounds.min, other.Bounds.min);
            bounds.max = Vector2d.Max(bounds.max, other.Bounds.max);
            return true;
        }

        public void VisualDebug()
        {
            LinkedListNode<Vector2d> chainNode = chain.First;
            while ((chainNode = chainNode.Next) != null)
            {
                Debug.DrawLine((Vector2)chainNode.Previous.Value, (Vector2)chainNode.Value);
                DebugExtension.DebugCircle((Vector2)chainNode.Previous.Value, Vector3.forward, 0.1f);

            }
            if (IsClosed)
            {
                Debug.DrawLine((Vector2)chain.Last.Value, (Vector2)chain.First.Value);
                DebugExtension.DebugCircle((Vector2)chain.Last.Value, Vector3.forward, 0.1f);
            }
        }

        private void CalcBounds()
        {
            areBoundsValid = true;

            LinkedListNode<Vector2d> cNode = chain.First;
            bounds.min = cNode.Value;
            bounds.max = cNode.Value;
            while ((cNode = cNode.Next) != null)
            {
                bounds.min = Vector2d.Min(cNode.Value, bounds.min);
                bounds.max = Vector2d.Max(cNode.Value, bounds.max);
            }
        }
    }
}
