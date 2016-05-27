using UnityEngine;
using Utility;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Polygon2D
{
    public class PolygonClipper
    {
        public enum BoolOpType { INTERSECTION, UNION, DIFFERENCE, XOR };

        enum EdgeType { NORMAL, NON_CONTRIBUTING, SAME_TRANSITION, DIFFERENT_TRANSITION };
        enum PolygonType { SUBJECT, CLIPPING };

        public static Polygon Compute(ref Polygon sp, ref Polygon cp, BoolOpType op)
        {
            //Trivial case: At least one polygon is empty
            if (sp.IsEmpty || cp.IsEmpty)
            {
                if (op == BoolOpType.DIFFERENCE)
                    return sp;
                if (op == BoolOpType.UNION)
                    return (sp.IsEmpty) ? cp : sp;
                //Return null for INTERSECTION and XOR operations;
                return new Polygon();
            }

            //Trivial case: The polygons cannot intersect each other.
            if (!sp.Bounds.Intersects(cp.Bounds))
            {
                if (op == BoolOpType.DIFFERENCE)
                    return sp;
                if (op == BoolOpType.UNION)
                {
                    Polygon result = new Polygon(sp);
                    result.AddContours(cp);
                    return result;
                }
                //Return null for INTERSECTION and XOR operations;
                return new Polygon();
            }

            //Init the event queue with the polygon edges
            HeapPriorityQueue<SweepEvent> eventQueue = new HeapPriorityQueue<SweepEvent>(sp.TotalVertexCount + cp.TotalVertexCount);
            InsertPolygon(eventQueue, sp, PolygonType.SUBJECT);
            InsertPolygon(eventQueue, cp, PolygonType.CLIPPING);

            SweepRay sweepRay = new SweepRay(20);
            SweepEvent cEvent;
            float minRightBounds = Mathf.Min(sp.Bounds.max.x, cp.Bounds.max.x);

            while (eventQueue.Count != 0)
            {
                cEvent = eventQueue.Dequeue();

                if ((op == BoolOpType.INTERSECTION && (cEvent.p.x > minRightBounds)) || (op == BoolOpType.DIFFERENCE && cEvent.p.x > sp.Bounds.max.x))
                {
                    //Exit the loop. No more intersections are to be found.
                    // Create a polygon out of the pointchain
                    return;
                }
                if ((op == BoolOpType.UNION && (cEvent.p.x > minRightBounds)))
                {
                    if (!cEvent.left)
                    {
                        //Add this edge to the connector
                    }
                    while (eventQueue.Count != 0)
                    {
                        cEvent = eventQueue.Dequeue();
                        if (!cEvent.left)
                        {
                            // Add this edge to the connector
                        }
                    }
                    return;
                }

                if (cEvent.left)
                {
                    int pos = sweepRay.Add(cEvent);
                    SweepEvent prevEvent = sweepRay.Previous(pos);
                    if (prevEvent == null)
                        cEvent.inside = cEvent.inOut = false;
                    else if (prevEvent.type != EdgeType.NORMAL)
                    {
                        if (pos == 0)
                        {
                            cEvent.inside = true;
                            cEvent.inOut = false;
                        }
                        else {
                            SweepEvent sliEvent = sweepRay.Previous(pos - 1);
                            if (prevEvent.pl == cEvent.pl)
                            {
                                cEvent.inOut = !prevEvent.inOut;
                                cEvent.inside = !sliEvent.inOut;
                            }
                            else
                            {
                                cEvent.inOut = !sliEvent.inOut;
                                cEvent.inside = !prevEvent.inOut;
                            }
                        }
                    }
                    SweepEvent nextEvent = sweepRay.Next(pos);


                }
            }

        }

        private static void InsertPolygon(HeapPriorityQueue<SweepEvent> eventQueue, Polygon p, PolygonType pType)
        {
            Vector2 v2;
            Vector2 v1;
            SweepEvent se1;
            SweepEvent se2;

            foreach (Contour c in p)
            {
                v1 = c.verticies[0];
                for (int iVert = 1; iVert < c.VertexCount; iVert += 2)
                {
                    v2 = c.verticies[iVert];

                    if (v1.x < v2.x || (v1.x == v2.x && v1.y < v2.y))
                    {
                        se1 = new SweepEvent(v1, true, pType);
                        se2 = new SweepEvent(v2, false, pType);
                    }
                    else
                    {
                        se1 = new SweepEvent(v1, false, pType);
                        se2 = new SweepEvent(v2, true, pType);
                    }
                    se1.other = se2;
                    se2.other = se1;
                    eventQueue.Enqueue(se1);
                    eventQueue.Enqueue(se2);
                    v1 = v2;
                }
                //Insert last connection [count - 1] -> [0]
                v2 = c.verticies[0];
                if (v1.x < v2.x || (v1.x == v2.x && v1.y < v2.y))
                {
                    se1 = new SweepEvent(v1, true, pType);
                    se2 = new SweepEvent(v2, false, pType);
                }
                else
                {
                    se1 = new SweepEvent(v1, false, pType);
                    se2 = new SweepEvent(v2, true, pType);
                }
                se1.other = se2;
                se2.other = se1;
                eventQueue.Enqueue(se1);
                eventQueue.Enqueue(se2);
            }
        }

        const float threshold = 0.01f;
        private void HandlePossibleIntersection(SweepEvent eventA, SweepEvent eventB)
        {
            if (eventB == null || eventA == null)
                return;

            if (!eventA.left)
                eventA = eventA.other;
            if (!eventB.left)
                eventB = eventB.other;
            Debug.Log("Try possible intersection againts " + sRay.Find(eventB) + ", " + eventB.ToString());
            Vector2 pA = eventA.point - eventA.other.point;
            Vector2 pB = eventB.point - eventB.other.point;
            float c = pA.x * pB.y - pA.y * pB.x;
            if (Mathf.Abs(c) < 0.01f)
                return;
            else
            {
                float a = eventA.point.x * eventA.other.point.y - eventA.point.y * eventA.other.point.x;
                float b = eventB.point.x * eventB.other.point.y - eventB.point.y * eventB.other.point.x;

                Vector2 intersection = (a * pB - b * pA) / c;

                //Check if point is on segment
                bool isInSegment = false;
                if (eventA.isVertical || eventB.isVertical)
                {
                    isInSegment = eventA.point.y - threshold <= intersection.y && intersection.y <= eventA.other.point.y + threshold && eventB.point.y - threshold <= intersection.y && intersection.y <= eventB.other.point.y + threshold;
                }
                else
                    isInSegment = eventA.point.x - threshold <= intersection.x && intersection.x <= eventA.other.point.x + threshold && eventB.point.x - threshold <= intersection.x && intersection.x <= eventB.other.point.x + threshold;

                if (isInSegment)
                {
                    //Check if point is exactly on an edgeEndPoint
                    if (eventA.point == intersection || eventB.point == intersection || eventA.other.point == intersection || eventB.other.point == intersection)
                    {
                        Debug.Log("Failed point check! " + intersection);
                        return;
                    }

                    //Found a valid intersection! Now subdivide the edges accordingly.
                    Debug.Log("Subdivision approved!");
                    SubDivideEdge(eventA, intersection, false);
                    SubDivideEdge(eventB, intersection, true);
                }
                else
                {
                    Debug.Log("Failed width check!" + eventA.point.x + " <= " + intersection.x + " <=" + eventA.other.point.x + " = " + (eventA.point.x - threshold <= intersection.x && intersection.x <= eventA.other.point.x + threshold));
                    Debug.Log("Failed height check!" + eventA.point.y + " <=" + intersection.y + " <=" + eventA.other.point.y + " = " + (eventA.point.y - threshold <= intersection.y && intersection.y <= eventA.other.point.y + threshold));
                    Debug.Log("Failed width check!" + eventB.point.x + " <=" + intersection.x + " <=" + eventB.other.point.x + " = " + (eventB.point.x - threshold <= intersection.x && intersection.x <= eventB.other.point.x + threshold));
                    Debug.Log("Failed height check!" + eventB.point.y + " <=" + intersection.y + " <=" + eventB.other.point.y + " = " + (eventB.point.y - threshold <= intersection.y && intersection.y <= eventB.other.point.y + threshold));
                    Debug.Log("---");
                }
            }
        }

        class SweepEvent : PriorityQueueNode
        {
            public Vector2 p;           // point associated with the event
            public bool left;         // is the point the left endpoint of the segment (p, other->p)?
            public PolygonType pl;    // Polygon to which the associated segment belongs to
            public SweepEvent other; // Event associated to the other endpoint of the segment
                                     /**  Does the segment (p, other->p) represent an inside-outside transition in the polygon for a vertical ray from (p.x, -infinite) that crosses the segment? */
            public bool inOut;
            public EdgeType type;
            public bool inside; // Only used in "left" events. Is the segment (p, other->p) inside the other polygon?
            public int posS; // Only used in "left" events. Position of the event (line segment) in S

            /** Class constructor */
            public SweepEvent(Vector2 point, bool left, PolygonType pTyp, EdgeType t = EdgeType.NORMAL)
            {
                p = point;
                this.left = left;
                pl = pTyp;
                type = t;
            }

            /** Is the line segment (p, other->p) below point x */
            public bool IsBelow(Vector2 o) { return (left) ? ExtendedMathf.SignedArea(p, other.p, o) > 0 : ExtendedMathf.SignedArea(other.p, p, o) > 0; }
            /** Is the line segment (p, other->p) above point x */
            public bool IsAbove(Vector2 o) { return !IsBelow(p); }

            public override int CompareTo(PriorityQueueNode other)
            {
                if (other.GetType() == typeof(SweepEvent))
                {
                    SweepEvent so = (SweepEvent)other;
                    if (p.x > so.p.x)
                        return 1;
                    if (p.x < so.p.x)
                        return -1;
                    if (p.y > so.p.y)
                        return 1;
                    if (p.y < so.p.y)
                        return -1;
                    if (left != so.left)
                    {
                        if (left)
                            return 1;
                        return -1;
                    }
                    if (IsBelow(so.p))
                        return 1;
                    return -1;
                }
                return base.CompareTo(other);

            }

            public static bool operator ==(SweepEvent se1, SweepEvent se2)
            {
                if (object.ReferenceEquals(se1, null))
                    return object.ReferenceEquals(se2, null);
                return (se1.p == se2.p && se1.left == se2.left);
            }

            public static bool operator !=(SweepEvent se1, SweepEvent se2)
            {
                return !(se1 == se2);
            }
        }

        class SweepRay
        {
            List<SweepEvent> s;

            public SweepRay(int capacity)
            {
                s = new List<SweepEvent>(capacity);
            }

            public int Add(SweepEvent e)
            {
                for (int i = 0; i < s.Count; i++)
                {
                    SweepEvent se = s[i];
                    if (IsEventOneMoreImportant(se, e))
                        continue;
                    s.Insert(i - 1, e);
                    return i - 1;
                }
                s.Add(e);
                return s.Count - 1;
            }

            public int Find(SweepEvent e)
            {
                return s.IndexOf(e);
            }

            public void RemoveAt(int index)
            {
                s.RemoveAt(index);
            }

            public SweepEvent Next(int index)
            {
                index++;
                if (index < s.Count)
                    return s[index];
                return null;
            }

            public SweepEvent Previous(int index)
            {
                index--;
                if (index >= 0)
                    return s[index];
                return null;
            }

            public override string ToString()
            {
                string result = "[" + s.Count + "] ";
                foreach (SweepEvent se in s)
                    result += ((se.left) ? "l" : "r") + se.ToString() + ", ";
                return result;
            }

            private bool IsEventOneMoreImportant(SweepEvent se1, SweepEvent se2)
            {
                if (se1 == se2)
                    return false;
                if (ExtendedMathf.SignedArea(se1.p, se1.other.p, se2.p) != 0 || ExtendedMathf.SignedArea(se1.p, se1.other.p, se2.other.p) != 0)
                {
                    if (se1.p == se2.p)
                        return se1.IsBelow(se2.other.p);

                    if (se1.CompareTo(se2) > 0)
                        return se2.IsAbove(se1.p);
                    return se1.IsBelow(se2.p);
                }
                if (se1.p == se2.p)
                    return false; //Not sure here. Seems like lines exactly overlap each other. Didnt found the < operator though.
                return se1.CompareTo(se2) > 0;
            }
        }
    }
}
