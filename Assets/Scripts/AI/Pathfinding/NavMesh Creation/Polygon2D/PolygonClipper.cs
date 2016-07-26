using UnityEngine;
using Utility;
using System.Collections.Generic;

namespace Polygon2D
{
    public static class PolygonClipper
    {
        public enum BoolOpType { INTERSECTION, UNION, DIFFERENCE, XOR };
        public enum ResultType { Error_NotClosedChain, OnePolygonIsEmpty, BoundingBoxTestFailed, SuccesfullyCliped, NoChangesMade };

        enum EdgeType { NORMAL, NON_CONTRIBUTING, SAME_TRANSITION, DIFFERENT_TRANSITION };
        enum PolygonType { SUBJECT, CLIPPING };

        public static ResultType Compute(PointChain sp, PointChain cp, BoolOpType op, out PointChain[] result)
        {
            result = null;
            //Bad data: At least one PointChain isn't closed
            if (!sp.IsClosed || !cp.IsClosed)
            {
                Debug.Log("One, or both PointChains aren't closed. sp.IsClosed = " + sp.IsClosed + ", cp.IsClosed = " + cp.IsClosed);
                return ResultType.Error_NotClosedChain;
            }

            //Trivial case: At least one polygon is empty
            if (sp.IsEmpty || cp.IsEmpty)
            {
                Debug.Log("One, or both are empty. sp.IsEmpty = " + sp.IsEmpty + ", cp.IsEmpty = " + cp.IsEmpty);
                if (op == BoolOpType.DIFFERENCE)
                {
                    result = new PointChain[] { sp };
                }
                else if (op == BoolOpType.UNION)
                {
                    result = new PointChain[] { (sp.IsEmpty) ? cp : sp };
                }
                //Return null for INTERSECTION and XOR operations;
                return ResultType.OnePolygonIsEmpty;
            }

            //Trivial case: The polygons cannot intersect each other.
            if (!sp.Bounds.Intersects(cp.Bounds))
            {
                Debug.Log("They don't intersect!");
                if (op == BoolOpType.DIFFERENCE)
                    result = new PointChain[] { sp };
                if (op == BoolOpType.UNION)
                {
                    result = new PointChain[] { sp, cp };
                }
                //Return null for INTERSECTION and XOR operations;
                return ResultType.BoundingBoxTestFailed;
            }

            //Init the event queue with the polygon edges
            HeapPriorityQueue<SweepEvent> eventQueue = new HeapPriorityQueue<SweepEvent>((sp.chain.Count + cp.chain.Count) * 3);
            InsertPolygon(eventQueue, sp, PolygonType.SUBJECT);
            InsertPolygon(eventQueue, cp, PolygonType.CLIPPING);

            SweepRay sweepRay = new SweepRay(20);
            SweepEvent cEvent;
            float minRightBounds = Mathf.Min(sp.Bounds.max.x, cp.Bounds.max.x);
            Connector connector = new Connector(10);
            while (eventQueue.Count != 0)
            {
                cEvent = eventQueue.Dequeue();
                if ((op == BoolOpType.INTERSECTION && (cEvent.p.x > minRightBounds)) || (op == BoolOpType.DIFFERENCE && cEvent.p.x > sp.Bounds.max.x))
                {
                    //Exit the loop. No more intersections are to be found.
                    // Create a polygon out of the pointchain
                    Debug.Log("<color=green>Ended early. Op = 2, " + (eventQueue.Count + 1) + " events skiped</color>");
                    result = connector.ToArray();
                    return (NoChangesMade(sp, cp, result)) ? ResultType.NoChangesMade : ResultType.SuccesfullyCliped;
                }
                if ((op == BoolOpType.UNION && (cEvent.p.x > minRightBounds)))
                {
                    if (!cEvent.left)
                    {
                        connector.Add(cEvent.other.p, cEvent.p);
                    }
                    while (eventQueue.Count != 0)
                    {
                        cEvent = eventQueue.Dequeue();
                        if (!cEvent.left)
                        {
                            connector.Add(cEvent.other.p, cEvent.p);
                        }
                    }
                    Debug.Log("<color=green>Ended early. Op = 3, " + (eventQueue.Count + 1) + " events skiped </color>");
                    result = connector.ToArray();
                    return (NoChangesMade(sp, cp, result)) ? ResultType.NoChangesMade : ResultType.SuccesfullyCliped;
                }

                if (cEvent.left)
                {// the line segment must be inserted into S
                    int pos = sweepRay.Add(cEvent);
                    SweepEvent prev = sweepRay.Previous(pos);
                    if (prev == null)
                        cEvent.inside = cEvent.inOut = false;
                    else if (prev.type != EdgeType.NORMAL)
                    {
                        if (pos - 1 == 0)
                        {
                            cEvent.inside = true;
                            cEvent.inOut = false;
                        }
                        else
                        {
                            SweepEvent sliEvent = sweepRay.Previous(pos - 1);
                            if (prev.pl == cEvent.pl)
                            {
                                cEvent.inOut = !prev.inOut;
                                cEvent.inside = !sliEvent.inOut;
                            }
                            else
                            {
                                cEvent.inOut = !sliEvent.inOut;
                                cEvent.inside = !prev.inOut;
                            }
                        }
                    }
                    else if (cEvent.pl == prev.pl)
                    { // previous line segment in S belongs to the same polygon that "cEvent" belongs to
                        cEvent.inside = prev.inside;
                        cEvent.inOut = !prev.inOut;
                    }
                    else
                    {                          // previous line segment in S belongs to a different polygon that "cEvent" belongs to
                        cEvent.inside = !prev.inOut;
                        cEvent.inOut = prev.inside;
                    }

                    SweepEvent nextEvent = sweepRay.Next(pos);
                    if (nextEvent != null)
                        HandlePossibleIntersection(eventQueue, cEvent, nextEvent);
                    if (prev != null)
                        HandlePossibleIntersection(eventQueue, cEvent, prev);
                }
                else
                {// the line segment must be removed from S
                    int pos = sweepRay.Find(cEvent.other);
                    switch (cEvent.type)
                    {
                        case (EdgeType.NORMAL):
                            switch (op)
                            {
                                case (BoolOpType.INTERSECTION):
                                    if (cEvent.other.inside)
                                        connector.Add(cEvent.other.p, cEvent.p);
                                    break;
                                case (BoolOpType.UNION):
                                    if (!cEvent.other.inside)
                                        connector.Add(cEvent.other.p, cEvent.p);
                                    break;
                                case (BoolOpType.DIFFERENCE):
                                    if (((cEvent.pl == PolygonType.SUBJECT) && (!cEvent.other.inside)) || (cEvent.pl == PolygonType.CLIPPING && cEvent.other.inside))
                                        connector.Add(cEvent.other.p, cEvent.p);
                                    break;
                                case (BoolOpType.XOR):
                                    connector.Add(cEvent.other.p, cEvent.p);
                                    break;
                            }
                            break;
                        case (EdgeType.SAME_TRANSITION):
                            if (op == BoolOpType.INTERSECTION || op == BoolOpType.UNION)
                                connector.Add(cEvent.other.p, cEvent.p);
                            break;
                        case (EdgeType.DIFFERENT_TRANSITION):
                            if (op == BoolOpType.DIFFERENCE)
                                connector.Add(cEvent.other.p, cEvent.p);
                            break;
                    }
                    // delete line segment associated to e from S and check for intersection between the neighbors of "e" in S
                    SweepEvent next = sweepRay.Next(pos), prev = sweepRay.Previous(pos);
                    sweepRay.RemoveAt(pos);
                    if (next != null && prev != null)
                        HandlePossibleIntersection(eventQueue, prev, next);
                }
            }
            result = connector.ToArray();
            return (NoChangesMade(sp, cp, result))? ResultType.NoChangesMade : ResultType.SuccesfullyCliped;
        }

        public static PointChain[] RemoveSelfIntersections(PointChain sp)
        {
            //Init the event queue with the polygon edges
            HeapPriorityQueue<SweepEvent> eventQueue = new HeapPriorityQueue<SweepEvent>(sp.chain.Count * 2);
            InsertPolygon(eventQueue, sp, PolygonType.SUBJECT);

            SweepRay sweepRay = new SweepRay(20);
            SweepEvent cEvent;
            Connector connector = new Connector(10);
            while (eventQueue.Count != 0)
            {
                cEvent = eventQueue.Dequeue();

                if (cEvent.left)
                {// the line segment must be inserted into S
                    int pos = sweepRay.Add(cEvent);
                    SweepEvent prev = sweepRay.Previous(pos);
                    if (prev == null)
                        cEvent.inside = cEvent.inOut = false;
                    else if (prev.type != EdgeType.NORMAL)
                    {
                        if (pos - 1 == 0)
                        {
                            cEvent.inside = true;
                            cEvent.inOut = false;
                        }
                        else
                        {
                            SweepEvent sliEvent = sweepRay.Previous(pos - 1);
                            cEvent.inOut = !prev.inOut;
                            cEvent.inside = !sliEvent.inOut;
                        }
                    }
                    else
                    { // previous line segment in S belongs to the same polygon that "cEvent" belongs to
                        cEvent.inside = prev.inside;
                        cEvent.inOut = !prev.inOut;
                    }

                    SweepEvent nextEvent = sweepRay.Next(pos);
                    if (nextEvent != null)
                        HandlePossibleIntersection(eventQueue, cEvent, nextEvent);
                    if (prev != null)
                        HandlePossibleIntersection(eventQueue, cEvent, prev);

                }
                else
                {// the line segment must be removed from S
                    int pos = sweepRay.Find(cEvent.other);

                    connector.Add(cEvent.other.p, cEvent.p);
                    // delete line segment associated to e from S and check for intersection between the neighbors of "e" in S
                    SweepEvent next = sweepRay.Next(pos), prev = sweepRay.Previous(pos);
                    sweepRay.RemoveAt(pos);
                    if (next != null && prev != null)
                        HandlePossibleIntersection(eventQueue, prev, next);
                }
            }
            return connector.ToArray();
        }

        private static bool NoChangesMade(PointChain sp, PointChain cp, PointChain[] result)
        {
            if (result.Length != 2)
                return false;
            if (sp.chain.Count == result[0].chain.Count)
                return cp.chain.Count == result[1].chain.Count;
            else if ((sp.chain.Count == result[1].chain.Count))
                return (cp.chain.Count == result[0].chain.Count);

            return false;
        }

        private static void InsertPolygon(HeapPriorityQueue<SweepEvent> eventQueue, PointChain p, PolygonType pType)
        {
            SweepEvent se1;
            SweepEvent se2;

            LinkedListNode<Vector2> cNode = p.chain.First;
            while ((cNode = cNode.Next) != null)
            {
                if (cNode.Value.x < cNode.Previous.Value.x || (cNode.Value.x == cNode.Previous.Value.x && cNode.Value.y < cNode.Previous.Value.y))
                {
                    se1 = new SweepEvent(cNode.Value, true, pType);
                    se2 = new SweepEvent(cNode.Previous.Value, false, pType);
                }
                else
                {
                    se1 = new SweepEvent(cNode.Value, false, pType);
                    se2 = new SweepEvent(cNode.Previous.Value, true, pType);
                }
                se1.other = se2;
                se2.other = se1;
                eventQueue.Enqueue(se1);
                eventQueue.Enqueue(se2);
            }
            //Insert last connection [count - 1] . [0]
            if (p.chain.First.Value.x < p.chain.Last.Value.x || (p.chain.First.Value.x == p.chain.Last.Value.x && p.chain.First.Value.y < p.chain.Last.Value.y))
            {
                se1 = new SweepEvent(p.chain.First.Value, true, pType);
                se2 = new SweepEvent(p.chain.Last.Value, false, pType);
            }
            else
            {
                se1 = new SweepEvent(p.chain.First.Value, false, pType);
                se2 = new SweepEvent(p.chain.Last.Value, true, pType);
            }
            se1.other = se2;
            se2.other = se1;
            eventQueue.Enqueue(se1);
            eventQueue.Enqueue(se2);
        }

        private static int FindIntersection(SweepEvent se1, SweepEvent se2, out Vector2 pA, out Vector2 pB)
        {
            //Assign the resulting points some dummy values
            pA = Vector2.zero;
            pB = Vector2.zero;
            Vector2 se1_Begin = (se1.left) ? se1.p : se1.other.p;
            Vector2 se1_End = (se1.left) ? se1.other.p : se1.p;
            Vector2 se2_Begin = (se2.left) ? se2.p : se2.other.p;
            Vector2 se2_End = (se2.left) ? se2.other.p : se2.p;

            Vector2 d0 = se1_End - se1_Begin;
            Vector2 d1 = se2_End - se2_Begin;
            Vector2 e = se2_Begin - se1_Begin;

            float sqrEpsilon = 0.0000001f; // 0.001 before

            float kross = d0.x * d1.y - d0.y * d1.x;
            float sqrKross = kross * kross;
            float sqrLen0 = d0.sqrMagnitude;
            float sqrLen1 = d1.sqrMagnitude;

            if (sqrKross > sqrEpsilon * sqrLen0 * sqrLen1)
            {
                // lines of the segments are not parallel
                float s = (e.x * d1.y - e.y * d1.x) / kross;
                if ((s < 0) || (s > 1))
                {
                    return 0;
                }
                double t = (e.x * d0.y - e.y * d0.x) / kross;
                if ((t < 0) || (t > 1))
                {
                    return 0;
                }
                // intersection of lines is a point an each segment
                pA = se1_Begin + s * d0;
                if (Mathf.Approximately(Vector2.Distance(pA, se1_Begin), 0)) pA = se1_Begin;
                if (Mathf.Approximately(Vector2.Distance(pA, se1_End), 0)) pA = se1_End;
                if (Mathf.Approximately(Vector2.Distance(pA, se2_Begin), 0)) pA = se2_Begin;
                if (Mathf.Approximately(Vector2.Distance(pA, se2_End), 0)) pA = se2_End;
                return 1;
            }

            // lines of the segments are parallel
            float sqrLenE = e.sqrMagnitude;
            kross = e.x * d0.y - e.y * d0.x;
            sqrKross = kross * kross;
            if (sqrKross > sqrEpsilon * sqrLen0 * sqrLenE)
            {
                // lines of the segment are different
                return 0;
            }

            // Lines of the segments are the same. Need to test for overlap of segments.
            float s0 = (d0.x * e.x + d0.y * e.y) / sqrLen0;  // so = Dot (D0, E) * sqrLen0
            float s1 = s0 + (d0.x * d1.x + d0.y * d1.y) / sqrLen0;  // s1 = s0 + Dot (D0, D1) * sqrLen0
            float smin = Mathf.Min(s0, s1);
            float smax = Mathf.Max(s0, s1);
            float[] w = new float[2];
            int imax = FindIntersection(0.0f, 1.0f, smin, smax, w);

            if (imax > 0)
            {
                pA = se1_Begin + w[0] * d0;
                if (Mathf.Approximately(Vector2.Distance(pA, se1_Begin), 0)) pA = se1_Begin;
                if (Mathf.Approximately(Vector2.Distance(pA, se1_End), 0)) pA = se1_End;
                if (Mathf.Approximately(Vector2.Distance(pA, se2_Begin), 0)) pA = se2_Begin;
                if (Mathf.Approximately(Vector2.Distance(pA, se2_End), 0)) pA = se2_End;
                if (imax > 1)
                {
                    pB = se1_Begin + w[1] * d0;
                }
            }
            return imax;
        }

        private static int FindIntersection(float u0, float u1, float v0, float v1, float[] w)
        {
            if ((u1 < v0) || (u0 > v1))
                return 0;
            if (u1 > v0)
            {
                if (u0 < v1)
                {
                    w[0] = (u0 < v0) ? v0 : u0;
                    w[1] = (u1 > v1) ? v1 : u1;
                    return 2;
                }
                else
                {
                    // u0 == v1
                    w[0] = u0;
                    return 1;
                }
            }
            else
            {
                // u1 == v0
                w[0] = u1;
                return 1;
            }
        }

        private static void HandlePossibleIntersection(HeapPriorityQueue<SweepEvent> eventQueue, SweepEvent e1, SweepEvent e2)
        {

            Vector2 ip1, ip2;  // intersection points
            int nintersections;

            if ((nintersections = FindIntersection(e1, e2, out ip1, out ip2)) == 0)
                return;

            if ((nintersections == 1) && ((e1.p == e2.p) || (e1.other.p == e2.other.p)))
                return; // the line segments intersect at an endpoint of both line segments

            if (nintersections == 2 && e1.pl == e2.pl)
                return; // the line segments overlap, but they belong to the same polygon

            // The line segments associated to e1 and e2 intersect
            if (nintersections == 1)
            {
                if (e1.p != ip1 && e1.other.p != ip1)  // if ip1 is not an endpoint of the line segment associated to e1 then divide "e1"
                    DivideEdge(eventQueue, e1, ip1);
                if (e2.p != ip1 && e2.other.p != ip1)  // if ip1 is not an endpoint of the line segment associated to e2 then divide "e2"
                    DivideEdge(eventQueue, e2, ip1);
                return;
            }

            // The line segments overlap
            List<SweepEvent> sortedEvents = new List<SweepEvent>(2);
            if (e1.p == e2.p)
            {
                sortedEvents.Add(null);
            }
            else if (e1.CompareTo(e2) > 0)
            {
                sortedEvents.Add(e2);
                sortedEvents.Add(e1);
            }
            else
            {
                sortedEvents.Add(e1);
                sortedEvents.Add(e2);
            }

            if (e1.other.p == e2.other.p)
            {
                sortedEvents.Add(null);
            }
            else if (e1.other.CompareTo(e2.other) > 0)
            {
                sortedEvents.Add(e2.other);
                sortedEvents.Add(e1.other);
            }
            else
            {
                sortedEvents.Add(e1.other);
                sortedEvents.Add(e2.other);
            }

            if (sortedEvents.Count == 2)
            { // are both line segments equal?
                e1.type = e1.other.type = EdgeType.NON_CONTRIBUTING;
                e2.type = e2.other.type = (e1.inOut == e2.inOut) ? EdgeType.SAME_TRANSITION : EdgeType.DIFFERENT_TRANSITION;
                return;
            }
            if (sortedEvents.Count == 3)
            { // the line segments share an endpoint
                sortedEvents[1].type = sortedEvents[1].other.type = EdgeType.NON_CONTRIBUTING;
                if (sortedEvents[0] != null)         // is the right endpoint the shared point?
                    sortedEvents[0].other.type = (e1.inOut == e2.inOut) ? EdgeType.SAME_TRANSITION : EdgeType.DIFFERENT_TRANSITION;
                else                                // the shared point is the left endpoint
                    sortedEvents[2].other.type = (e1.inOut == e2.inOut) ? EdgeType.SAME_TRANSITION : EdgeType.DIFFERENT_TRANSITION;
                DivideEdge(eventQueue, sortedEvents[0] != null ? sortedEvents[0] : sortedEvents[2].other, sortedEvents[1].p);
                return;
            }
            if (sortedEvents[0] != sortedEvents[3].other)
            { // no line segment includes totally the other one
                sortedEvents[1].type = EdgeType.NON_CONTRIBUTING;
                sortedEvents[2].type = (e1.inOut == e2.inOut) ? EdgeType.SAME_TRANSITION : EdgeType.DIFFERENT_TRANSITION;
                DivideEdge(eventQueue, sortedEvents[0], sortedEvents[1].p);
                DivideEdge(eventQueue, sortedEvents[1], sortedEvents[2].p);
                return;
            }
            // one line segment includes the other one
            sortedEvents[1].type = sortedEvents[1].other.type = EdgeType.NON_CONTRIBUTING;
            DivideEdge(eventQueue, sortedEvents[0], sortedEvents[1].p);
            sortedEvents[3].other.type = (e1.inOut == e2.inOut) ? EdgeType.SAME_TRANSITION : EdgeType.DIFFERENT_TRANSITION;
            DivideEdge(eventQueue, sortedEvents[3].other, sortedEvents[2].p);
        }

        private static void DivideEdge(HeapPriorityQueue<SweepEvent> eventQueue, SweepEvent e, Vector2 p)
        {
            // "Right event" of the "left line segment" resulting from dividing e (the line segment associated to e)
            SweepEvent r = new SweepEvent(p, false, e.pl, e.type);
            r.other = e;

            // "Left event" of the "right line segment" resulting from dividing e (the line segment associated to e)
            SweepEvent l = new SweepEvent(p, true, e.pl, e.other.type);
            l.other = e.other;

            if (l.CompareTo(e.other) > 0)
            { // avoid a rounding error. The left event would be processed after the right event
                Debug.LogWarning("Oops");
                e.other.left = true;
                l.left = false;
            }
            if (e.CompareTo(r) > 0)
            { // avoid a rounding error. The left event would be processed after the right event
                Debug.LogWarning("Oops2");
            }
            e.other.other = l;
            e.other = r;
            eventQueue.Enqueue(r);
            eventQueue.Enqueue(l);
        }

        class SweepEvent : PriorityQueueNode
        {
            public Vector2 p;           // point associated with the event
            public bool left;         // is the point the left endpoint of the segment (p, other.p)?
            public PolygonType pl;    // Polygon to which the associated segment belongs to
            public SweepEvent other; // Event associated to the other endpoint of the segment
                                     /**  Does the segment (p, other.p) represent an inside-outside transition in the polygon for a vertical ray from (p.x, -infinite) that crosses the segment? */
            public bool inOut;
            public EdgeType type;
            public bool inside; // Only used in "left" events. Is the segment (p, other.p) inside the other polygon?
            //public int posS; // Only used in "left" events. Position of the event (line segment) in S READD LATER!!!!!!

            /** Class constructor */
            public SweepEvent(Vector2 point, bool left, PolygonType pTyp, EdgeType t = EdgeType.NORMAL)
            {
                p = point;
                this.left = left;
                pl = pTyp;
                type = t;
            }

            /** Is the line segment (p, other.p) below point x */
            public bool IsBelow(Vector2 o) { return (left) ? ExMathf.SignedArea(p, other.p, o) > 0 : ExMathf.SignedArea(other.p, p, o) > 0; }
            /** Is the line segment (p, other.p) above point x */
            public bool IsAbove(Vector2 o) { return !IsBelow(o); }

            // Return true(1) means that e1 is placed at the event queue after e2, i.e,, e1 is processed by the algorithm after e2
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
                    if (IsAbove(so.other.p))
                        return 1;
                    return -1;
                }
                return base.CompareTo(other);

            }

            public static bool operator ==(SweepEvent se1, SweepEvent se2)
            {
                if (object.ReferenceEquals(se1, null))
                    return object.ReferenceEquals(se2, null);
                if (object.ReferenceEquals(se2, null))
                    return object.ReferenceEquals(se1, null);
                return (se1.p == se2.p && se1.left == se2.left && se1.other.p == se2.other.p);
            }

            public static bool operator !=(SweepEvent se1, SweepEvent se2)
            {
                return !(se1 == se2);
            }

            public override string ToString()
            {
                return "SE (p = " + p + ", l = " + left + ", pl = " + pl + ", inOut = " + ((left) ? inOut : other.inOut) + ", inside = " + ((left) ? inside : other.inside) + ", other.p = " + other.p + ")";
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
                    s.Insert(i, e);
                    return i;
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
                if (ExMathf.SignedArea(se1.p, se1.other.p, se2.p) != 0 || ExMathf.SignedArea(se1.p, se1.other.p, se2.other.p) != 0)
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

#if false
//Version with debug statments
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

        public static PointChain[] Compute(PointChain sp, PointChain cp, BoolOpType op)
        {
            Debug.Log("Start Compute");
            //Bad data: At least one PointChain isnt closed
            if (!sp.IsClosed || !cp.IsClosed)
            {
                Debug.Log("One, or both PointChains arent closed. sp.IsClosed = " + sp.IsClosed + ", cp.IsClosed = " + cp.IsClosed);
                return null;
            }

            //Trivial case: At least one polygon is empty
            if (sp.IsEmpty || cp.IsEmpty)
            {
                Debug.Log("One, or both are empty. sp.IsEmpty = " + sp.IsEmpty + ", cp.IsEmpty = " + cp.IsEmpty);
                if (op == BoolOpType.DIFFERENCE)
                    return new PointChain[] { sp };
                if (op == BoolOpType.UNION)
                    return new PointChain[] { (sp.IsEmpty) ? cp : sp };
                //Return null for INTERSECTION and XOR operations;
                return null;
            }

            //Trivial case: The polygons cannot intersect each other.
            if (!sp.Bounds.Intersects(cp.Bounds))
            {
                Debug.Log("They dont intersect!");
                if (op == BoolOpType.DIFFERENCE)
                    return new PointChain[] { sp };
                if (op == BoolOpType.UNION)
                {
                    return new PointChain[] { sp, cp };
                }
                //Return null for INTERSECTION and XOR operations;
                return null;
            }

            //Init the event queue with the polygon edges
            Debug.Log("sp.TotalVertexCount = " + sp.chain.Count + ", cp.TotalVertexCount = " + cp.chain.Count);
            HeapPriorityQueue<SweepEvent> eventQueue = new HeapPriorityQueue<SweepEvent>((sp.chain.Count + cp.chain.Count) * 2);
            InsertPolygon(eventQueue, sp, PolygonType.SUBJECT);
            InsertPolygon(eventQueue, cp, PolygonType.CLIPPING);

            SweepRay sweepRay = new SweepRay(20);
            SweepEvent cEvent;
            float minRightBounds = Mathf.Min(sp.Bounds.max.x, cp.Bounds.max.x);
            Connector connector = new Connector(10);
            Debug.Log("Start with a eventQueue.Count = " + eventQueue.Count);
            while (eventQueue.Count != 0)
            {
                cEvent = eventQueue.Dequeue();
                Debug.Log("<b>CEvent = " + cEvent + "</b>");
                if ((op == BoolOpType.INTERSECTION && (cEvent.p.x > minRightBounds)) || (op == BoolOpType.DIFFERENCE && cEvent.p.x > sp.Bounds.max.x))
                {
                    //Exit the loop. No more intersections are to be found.
                    // Create a polygon out of the pointchain
                    Debug.Log("<color=green>Ended early. Op = 2, " + (eventQueue.Count + 1) + " events skiped</color>");
                    return connector.ToArray();
                }
                if ((op == BoolOpType.UNION && (cEvent.p.x > minRightBounds)))
                {
                    if (!cEvent.left)
                    {
                        connector.Add(cEvent.other.p, cEvent.p);
                    }
                    while (eventQueue.Count != 0)
                    {
                        cEvent = eventQueue.Dequeue();
                        if (!cEvent.left)
                        {
                            connector.Add(cEvent.other.p, cEvent.p);
                        }
                    }
                    Debug.Log("<color=green>Ended early. Op = 3, " + (eventQueue.Count + 1) + " events skiped </color>");
                    return connector.ToArray();
                }

                if (cEvent.left)
                {// the line segment must be inserted into S
                    int pos = sweepRay.Add(cEvent);
                    SweepEvent prev = sweepRay.Previous(pos);
                    if (prev == null)
                        cEvent.inside = cEvent.inOut = false;
                    else if (prev.type != EdgeType.NORMAL)
                    {
                        if (pos - 1 == 0)
                        {
                            cEvent.inside = true;
                            cEvent.inOut = false;
                        }
                        else
                        {
                            SweepEvent sliEvent = sweepRay.Previous(pos - 1);
                            if (prev.pl == cEvent.pl)
                            {
                                cEvent.inOut = !prev.inOut;
                                cEvent.inside = !sliEvent.inOut;
                            }
                            else
                            {
                                cEvent.inOut = !sliEvent.inOut;
                                cEvent.inside = !prev.inOut;
                            }
                        }
                    }
                    else if (cEvent.pl == prev.pl)
                    { // previous line segment in S belongs to the same polygon that "cEvent" belongs to
                        cEvent.inside = prev.inside;
                        cEvent.inOut = !prev.inOut;
                        Debug.Log("     Set flags to: inside = " + cEvent.inside + ", inOut = " + cEvent.inOut + ", because prev event is same polygon");
                    }
                    else
                    {                          // previous line segment in S belongs to a different polygon that "cEvent" belongs to
                        cEvent.inside = !prev.inOut;
                        cEvent.inOut = prev.inside;
                        Debug.Log("     Set flags to: inside = " + cEvent.inside + ", inOut = " + cEvent.inOut + ", because of " + prev);
                    }

                    SweepEvent nextEvent = sweepRay.Next(pos);
                    if (nextEvent != null)
                        HandlePossibleIntersection(eventQueue, cEvent, nextEvent);
                    if (prev != null)
                        HandlePossibleIntersection(eventQueue, cEvent, prev);

                }
                else
                {// the line segment must be removed from S
                    int pos = sweepRay.Find(cEvent.other);
                    switch (cEvent.type)
                    {
                        case (EdgeType.NORMAL):
                            switch (op)
                            {
                                case (BoolOpType.INTERSECTION):
                                    if (cEvent.other.inside)
                                        connector.Add(cEvent.other.p, cEvent.p);
                                    break;
                                case (BoolOpType.UNION):
                                    if (!cEvent.other.inside)
                                        connector.Add(cEvent.other.p, cEvent.p);
                                    break;
                                case (BoolOpType.DIFFERENCE):
                                    if (((cEvent.pl == PolygonType.SUBJECT) && (!cEvent.other.inside)) || (cEvent.pl == PolygonType.CLIPPING && cEvent.other.inside))
                                        connector.Add(cEvent.other.p, cEvent.p);
                                    break;
                                case (BoolOpType.XOR):
                                    connector.Add(cEvent.other.p, cEvent.p);
                                    break;
                            }
                            break;
                        case (EdgeType.SAME_TRANSITION):
                            if (op == BoolOpType.INTERSECTION || op == BoolOpType.UNION)
                                connector.Add(cEvent.other.p, cEvent.p);
                            break;
                        case (EdgeType.DIFFERENT_TRANSITION):
                            if (op == BoolOpType.DIFFERENCE)
                                connector.Add(cEvent.other.p, cEvent.p);
                            break;
                    }
                    // delete line segment associated to e from S and check for intersection between the neighbors of "e" in S
                    SweepEvent next = sweepRay.Next(pos), prev = sweepRay.Previous(pos);
                    sweepRay.RemoveAt(pos);
                    if (next != null && prev != null)
                        HandlePossibleIntersection(eventQueue, prev, next);
                }
            }
            return connector.ToArray();
        }

        private static void InsertPolygon(HeapPriorityQueue<SweepEvent> eventQueue, PointChain p, PolygonType pType)
        {
            SweepEvent se1;
            SweepEvent se2;

            LinkedListNode<Vector2> cNode = p.chain.First;
            while((cNode = cNode.Next) != null)
            { 
                if (cNode.Value.x < cNode.Previous.Value.x || (cNode.Value.x == cNode.Previous.Value.x && cNode.Value.y < cNode.Previous.Value.y))
                {
                    se1 = new SweepEvent(cNode.Value, true, pType);
                    se2 = new SweepEvent(cNode.Previous.Value, false, pType);
                }
                else
                {
                    se1 = new SweepEvent(cNode.Value, false, pType);
                    se2 = new SweepEvent(cNode.Previous.Value, true, pType);
                }
                se1.other = se2;
                se2.other = se1;
                eventQueue.Enqueue(se1);
                eventQueue.Enqueue(se2);
            }
            //Insert last connection [count - 1] . [0]
            if (p.chain.First.Value.x < p.chain.Last.Value.x || (p.chain.First.Value.x == p.chain.Last.Value.x && p.chain.First.Value.y < p.chain.Last.Value.y))
            {
                se1 = new SweepEvent(p.chain.First.Value, true, pType);
                se2 = new SweepEvent(p.chain.Last.Value, false, pType);
            }
            else
            {
                se1 = new SweepEvent(p.chain.First.Value, false, pType);
                se2 = new SweepEvent(p.chain.Last.Value, true, pType);
            }
            se1.other = se2;
            se2.other = se1;
            eventQueue.Enqueue(se1);
            eventQueue.Enqueue(se2);
        }

        private static int FindIntersection(SweepEvent se1, SweepEvent se2, out Vector2 pA, out Vector2 pB)
        {
            //Assign the resulting points some dummy values
            pA = Vector2.zero;
            pB = Vector2.zero;
            Vector2 se1_Begin = (se1.left) ? se1.p : se1.other.p;
            Vector2 se1_End = (se1.left) ? se1.other.p : se1.p;
            Vector2 se2_Begin = (se2.left) ? se2.p : se2.other.p;
            Vector2 se2_End = (se2.left) ? se2.other.p : se2.p;

            Vector2 d0 = se1_End - se1_Begin;
            Vector2 d1 = se2_End - se2_Begin;
            Vector2 e = se2_Begin - se1_Begin;

            float sqrEpsilon = 0.0000001f; // 0.001 before

            float kross = d0.x * d1.y - d0.y * d1.x;
            float sqrKross = kross * kross;
            float sqrLen0 = d0.sqrMagnitude;
            float sqrLen1 = d1.sqrMagnitude;

            if (sqrKross > sqrEpsilon * sqrLen0 * sqrLen1)
            {
                // lines of the segments are not parallel
                float s = (e.x * d1.y - e.y * d1.x) / kross;
                if ((s < 0) || (s > 1))
                {
                    Debug.Log("         Not in segment " + d1);
                    return 0;
                }
                double t = (e.x * d0.y - e.y * d0.x) / kross;
                if ((t < 0) || (t > 1))
                {
                    Debug.Log("         Not in segment " + d0);
                    return 0;
                }
                // intersection of lines is a point an each segment
                pA = se1_Begin + s * d0;
                if (Mathf.Approximately(Vector2.Distance(pA, se1_Begin), 0)) pA = se1_Begin;
                if (Mathf.Approximately(Vector2.Distance(pA, se1_End), 0)) pA = se1_End;
                if (Mathf.Approximately(Vector2.Distance(pA, se2_Begin), 0)) pA = se2_Begin;
                if (Mathf.Approximately(Vector2.Distance(pA, se2_End), 0)) pA = se2_End;
                return 1;
            }

            // lines of the segments are parallel
            float sqrLenE = e.sqrMagnitude;
            kross = e.x * d0.y - e.y * d0.x;
            sqrKross = kross * kross;
            if (sqrKross > sqrEpsilon * sqrLen0 * sqrLenE)
            {
                // lines of the segment are different
                Debug.Log("         Segements dont overlap");
                return 0;
            }

            // Lines of the segments are the same. Need to test for overlap of segments.
            float s0 = (d0.x * e.x + d0.y * e.y) / sqrLen0;  // so = Dot (D0, E) * sqrLen0
            float s1 = s0 + (d0.x * d1.x + d0.y * d1.y) / sqrLen0;  // s1 = s0 + Dot (D0, D1) * sqrLen0
            float smin = Mathf.Min(s0, s1);
            float smax = Mathf.Max(s0, s1);
            float[] w = new float[2];
            int imax = FindIntersection(0.0f, 1.0f, smin, smax, w);

            if (imax > 0)
            {
                pA = se1_Begin + w[0] * d0;
                if (Mathf.Approximately(Vector2.Distance(pA, se1_Begin), 0)) pA = se1_Begin;
                if (Mathf.Approximately(Vector2.Distance(pA, se1_End), 0)) pA = se1_End;
                if (Mathf.Approximately(Vector2.Distance(pA, se2_Begin), 0)) pA = se2_Begin;
                if (Mathf.Approximately(Vector2.Distance(pA, se2_End), 0)) pA = se2_End;
                if (imax > 1)
                {
                    pB = se1_Begin + w[1] * d0;
                }
            }
            Debug.Log("         Overlap test returned " + imax);
            return imax;
        }

        private static int FindIntersection(float u0, float u1, float v0, float v1, float[] w)
        {
            if ((u1 < v0) || (u0 > v1))
                return 0;
            if (u1 > v0)
            {
                if (u0 < v1)
                {
                    w[0] = (u0 < v0) ? v0 : u0;
                    w[1] = (u1 > v1) ? v1 : u1;
                    return 2;
                }
                else
                {
                    // u0 == v1
                    w[0] = u0;
                    return 1;
                }
            }
            else
            {
                // u1 == v0
                w[0] = u1;
                return 1;
            }
        }

        private static void HandlePossibleIntersection(HeapPriorityQueue<SweepEvent> eventQueue, SweepEvent e1, SweepEvent e2)
        {
            //	if ((e1.pl == e2.pl) ) // you can uncomment these two lines if self-intersecting polygons are not allowed
            //		return false;

            Debug.Log("     Check for intersection between " + e1 + " and " + e2);
            Vector2 ip1, ip2;  // intersection points
            int nintersections;

            if ((nintersections = FindIntersection(e1, e2, out ip1, out ip2)) == 0)
                return;

            if ((nintersections == 1) && ((e1.p == e2.p) || (e1.other.p == e2.other.p)))
                return; // the line segments intersect at an endpoint of both line segments

            if (nintersections == 2 && e1.pl == e2.pl)
                return; // the line segments overlap, but they belong to the same polygon

            // The line segments associated to e1 and e2 intersect
            if (nintersections == 1)
            {
                Debug.Log("         The lines intersect!");
                if (e1.p != ip1 && e1.other.p != ip1)  // if ip1 is not an endpoint of the line segment associated to e1 then divide "e1"
                    DivideEdge(eventQueue, e1, ip1);
                if (e2.p != ip1 && e2.other.p != ip1)  // if ip1 is not an endpoint of the line segment associated to e2 then divide "e2"
                    DivideEdge(eventQueue, e2, ip1);
                return;
            }

            // The line segments overlap
            List<SweepEvent> sortedEvents = new List<SweepEvent>(2);
            if (e1.p == e2.p)
            {
                sortedEvents.Add(null);
            }
            else if (e1.CompareTo(e2) > 0)
            {
                sortedEvents.Add(e2);
                sortedEvents.Add(e1);
            }
            else
            {
                sortedEvents.Add(e1);
                sortedEvents.Add(e2);
            }

            if (e1.other.p == e2.other.p)
            {
                sortedEvents.Add(null);
            }
            else if (e1.other.CompareTo(e2.other) > 0)
            {
                sortedEvents.Add(e2.other);
                sortedEvents.Add(e1.other);
            }
            else
            {
                sortedEvents.Add(e1.other);
                sortedEvents.Add(e2.other);
            }

            if (sortedEvents.Count == 2)
            { // are both line segments equal?
                Debug.Log("         Both lines are equal!");
                e1.type = e1.other.type = EdgeType.NON_CONTRIBUTING;
                e2.type = e2.other.type = (e1.inOut == e2.inOut) ? EdgeType.SAME_TRANSITION : EdgeType.DIFFERENT_TRANSITION;
                return;
            }
            if (sortedEvents.Count == 3)
            { // the line segments share an endpoint
                Debug.Log("         The lines share an endpoint!");
                sortedEvents[1].type = sortedEvents[1].other.type = EdgeType.NON_CONTRIBUTING;
                if (sortedEvents[0] == null)         // is the right endpoint the shared point?
                    sortedEvents[0].other.type = (e1.inOut == e2.inOut) ? EdgeType.SAME_TRANSITION : EdgeType.DIFFERENT_TRANSITION;
                else                                // the shared point is the left endpoint
                    sortedEvents[2].other.type = (e1.inOut == e2.inOut) ? EdgeType.SAME_TRANSITION : EdgeType.DIFFERENT_TRANSITION;
                DivideEdge(eventQueue, sortedEvents[0] == null ? sortedEvents[0] : sortedEvents[2].other, sortedEvents[1].p);
                return;
            }
            if (sortedEvents[0] != sortedEvents[3].other)
            { // no line segment includes totally the other one
                Debug.Log("         No line segment includes the other one!");
                sortedEvents[1].type = EdgeType.NON_CONTRIBUTING;
                sortedEvents[2].type = (e1.inOut == e2.inOut) ? EdgeType.SAME_TRANSITION : EdgeType.DIFFERENT_TRANSITION;
                DivideEdge(eventQueue, sortedEvents[0], sortedEvents[1].p);
                DivideEdge(eventQueue, sortedEvents[1], sortedEvents[2].p);
                return;
            }
            Debug.Log("         One line segment includes the other one!");
            // one line segment includes the other one
            sortedEvents[1].type = sortedEvents[1].other.type = EdgeType.NON_CONTRIBUTING;
            DivideEdge(eventQueue, sortedEvents[0], sortedEvents[1].p);
            sortedEvents[3].other.type = (e1.inOut == e2.inOut) ? EdgeType.SAME_TRANSITION : EdgeType.DIFFERENT_TRANSITION;
            DivideEdge(eventQueue, sortedEvents[3].other, sortedEvents[2].p);
        }

        private static void DivideEdge(HeapPriorityQueue<SweepEvent> eventQueue, SweepEvent e, Vector2 p)
        {
            Debug.Log("         Divided Edge between " + e + " and " + p);
            // "Right event" of the "left line segment" resulting from dividing e (the line segment associated to e)
            SweepEvent r = new SweepEvent(p, false, e.pl, e.type);
            r.other = e;

            // "Left event" of the "right line segment" resulting from dividing e (the line segment associated to e)
            SweepEvent l = new SweepEvent(p, true, e.pl, e.other.type);
            l.other = e.other;

            if (l.CompareTo(e.other) > 0)
            { // avoid a rounding error. The left event would be processed after the right event
                Debug.LogWarning("Oops");
                e.other.left = true;
                l.left = false;
            }
            if (e.CompareTo(r) > 0)
            { // avoid a rounding error. The left event would be processed after the right event
                Debug.LogWarning("Oops2");
            }
            e.other.other = l;
            e.other = r;
            eventQueue.Enqueue(r);
            eventQueue.Enqueue(l);
        }

        class SweepEvent : PriorityQueueNode
        {
            public Vector2 p;           // point associated with the event
            public bool left;         // is the point the left endpoint of the segment (p, other.p)?
            public PolygonType pl;    // Polygon to which the associated segment belongs to
            public SweepEvent other; // Event associated to the other endpoint of the segment
                                     /**  Does the segment (p, other.p) represent an inside-outside transition in the polygon for a vertical ray from (p.x, -infinite) that crosses the segment?*/
public bool inOut;
public EdgeType type;
public bool inside; // Only used in "left" events. Is the segment (p, other.p) inside the other polygon?
                    //public int posS; // Only used in "left" events. Position of the event (line segment) in S READD LATER!!!!!!

/** Class constructor */
public SweepEvent(Vector2 point, bool left, PolygonType pTyp, EdgeType t = EdgeType.NORMAL)
{
    p = point;
    this.left = left;
    pl = pTyp;
    type = t;
}

/** Is the line segment (p, other.p) below point x */
public bool IsBelow(Vector2 o) { return (left) ? ExMathf.SignedArea(p, other.p, o) > 0 : ExMathf.SignedArea(other.p, p, o) > 0; }
/** Is the line segment (p, other.p) above point x */
public bool IsAbove(Vector2 o) { return !IsBelow(o); }

// Return true(1) means that e1 is placed at the event queue after e2, i.e,, e1 is processed by the algorithm after e2
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
        if (IsAbove(so.other.p))
            return 1;
        return -1;
    }
    return base.CompareTo(other);

}

public static bool operator ==(SweepEvent se1, SweepEvent se2)
{
    if (object.ReferenceEquals(se1, null))
        return object.ReferenceEquals(se2, null);
    if (object.ReferenceEquals(se2, null))
        return object.ReferenceEquals(se1, null);
    return (se1.p == se2.p && se1.left == se2.left && se1.other.p == se2.other.p);
}

public static bool operator !=(SweepEvent se1, SweepEvent se2)
{
    return !(se1 == se2);
}

public override string ToString()
{
    return "SE (p = " + p + ", l = " + left + ", pl = " + pl + ", inOut = " + ((left) ? inOut : other.inOut) + ", inside = " + ((left) ? inside : other.inside) + ", other.p = " + other.p + ")";
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
            s.Insert(i, e);
            return i;
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
        if (ExMathf.SignedArea(se1.p, se1.other.p, se2.p) != 0 || ExMathf.SignedArea(se1.p, se1.other.p, se2.other.p) != 0)
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
#endif