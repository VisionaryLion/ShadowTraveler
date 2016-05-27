using UnityEngine;
using System.Collections;
using System;
using ValueSetManager;
using System.Collections.Generic;

namespace Polygon2D
{
    public class Polygon2DBooleanFunc
    {
        private HeapPriorityQueue<SweepEvent> queue;
        private SweepRay sRay;
        private List<Vector2> union;
        private List<Vector2> intersection;

        public Vector2[] Union(Polygon polyA, Polygon polyB)
        {
            int totalVertCount = polyA.verticies.Length + polyB.verticies.Length;

            //Init the priorityQueue with the verts of both polygons
            queue = new HeapPriorityQueue<SweepEvent>(totalVertCount * 2); //Accounting for subdivided edges. TODO: Find a save way.
            //Load edges for polygone A
            for (int iVert = 0; iVert < polyA.verticies.Length - 1; iVert++)
                InsertNewEdge(polyA.verticies[iVert], polyA.verticies[iVert + 1], true);
            InsertNewEdge(polyA.verticies[0], polyA.verticies[polyA.verticies.Length - 1], true);
            //Load edges for polygone B
            for (int iVert = 0; iVert < polyB.verticies.Length - 1; iVert++)
                InsertNewEdge(polyB.verticies[iVert], polyB.verticies[iVert + 1], false);
            InsertNewEdge(polyB.verticies[0], polyB.verticies[polyB.verticies.Length - 1], false);

            //do some sweeping
            sRay = new SweepRay(10);
            union = new List<Vector2>(totalVertCount);
            intersection = new List<Vector2>(totalVertCount);
            List<SweepEvent> rightEdgesToProcess = new List<SweepEvent>(1);
            List<SweepEvent> leftEdgesToProcess = new List<SweepEvent>(1);
            while (queue.Count > 0)
            {
                Debug.Log(sRay);
                SweepEvent currentEvent;
                do
                {
                    currentEvent = queue.Dequeue();
                    if (currentEvent.left)
                    {
                        Debug.Log("left: " + currentEvent.ToString());
                        leftEdgesToProcess.Add(currentEvent);
                    }
                    else
                    {
                        Debug.Log("right: " + currentEvent.ToString());
                        rightEdgesToProcess.Add(currentEvent);
                    }
                } while (queue.Count > 0 && currentEvent.Priority == queue.First.Priority && currentEvent.left);

                for (int i = 0; i < rightEdgesToProcess.Count; i++)
                    ProcessRightEdge(rightEdgesToProcess[i]);
                for (int i = 0; i < leftEdgesToProcess.Count; i++)
                    ProcessLeftEdge(leftEdgesToProcess[i]);

                rightEdgesToProcess.Clear();
                leftEdgesToProcess.Clear();
            }
            return union.ToArray();
        }

        private void ProcessRightEdge(SweepEvent sEvent)
        {

            int pos = sRay.Find(sEvent.other);
            Debug.Log("Proccesed as right: " + pos);
            SweepEvent prev = sRay.Previous(pos);
            SweepEvent next = sRay.Next(pos);
            if (sEvent.other.inside)
            {
                intersection.Add(sEvent.other.point);
                intersection.Add(sEvent.point);
            }
            else
            {
                
            }
            union.Add(sEvent.other.point);
            union.Add(sEvent.point);
            sRay.RemoveAt(pos);
            HandlePossibleIntersection(prev, next);
        }

        private void ProcessLeftEdge(SweepEvent sEvent)
        {
            int pos = sRay.Add(sEvent);
            Debug.Log("Proccesed as left: " + pos);
            SweepEvent prev = sRay.Previous(pos);
            sEvent.SetInsideFlag(prev);
            HandlePossibleIntersection(sEvent, prev);
            HandlePossibleIntersection(sEvent, sRay.Next(pos));
            queue.Enqueue(sEvent.other);
        }

        private void InsertNewEdge(Vector2 vertA, Vector2 vertB, bool isInPolyA)
        {
            SweepEvent sweepA = new SweepEvent(vertA, isInPolyA);
            SweepEvent sweepB = new SweepEvent(vertB, isInPolyA);
            sweepA.other = sweepB;
            sweepB.other = sweepA;
            if (vertA.x < vertB.x)
                sweepA.left = true;
            else if (vertA.x > vertB.x)
                sweepB.left = true;
            else
            {
                if (vertA.y < vertB.y)
                    sweepA.left = true;
                else
                    sweepB.left = true;
                sweepA.isVertical = true;
                sweepB.isVertical = true;
            }
            if (sweepA.left)
                queue.Enqueue(sweepA);
            else
                queue.Enqueue(sweepB);
        }

        private void SubDivideEdge(SweepEvent sEvent, Vector2 point, bool endAllreadyInQueue)
        {
            if (endAllreadyInQueue)
                queue.Remove(sEvent.other);
            SweepEvent newEdge = new SweepEvent(point, sEvent.polyA);
            newEdge.left = true;
            newEdge.isVertical = sEvent.isVertical;
            newEdge.other = sEvent.other;
            newEdge.other.other = newEdge;
            queue.Enqueue(newEdge);

            SweepEvent newEdge2 = new SweepEvent(point, sEvent.polyA);
            newEdge2.left = false;
            newEdge2.other = sEvent;
            sEvent.other = newEdge2;
            if (endAllreadyInQueue)
                queue.Enqueue(newEdge2);
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
            Debug.Log("Try possible intersection againts "+ sRay.Find(eventB)+", "+eventB.ToString());
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

        private class SweepEvent : PriorityQueueNode
        {
            public Vector2 point;
            public SweepEvent other;
            public bool left; //is this the left endpoint of the edge?
            public bool inOut; //inside - outside transition into the polygon
            public bool inside; //is the edge inside the other polygon?
            public bool polyA; //does this edge belongs to polygon A?
            public bool isVertical;

            public SweepEvent(Vector2 point, bool isInPolyA)
            {
                this.point = point;
                this.polyA = isInPolyA;
                Priority = point.x;
            }

            public void SetInsideFlag(SweepEvent prevEvent)
            {
                if (prevEvent == null)
                {
                    inside = false;
                    inOut = false;
                }
                else if (prevEvent.polyA == this.polyA)
                {
                    inside = prevEvent.inside;
                    inOut = !prevEvent.inOut;
                }
                else
                {
                    inside = !prevEvent.inOut;
                    inOut = prevEvent.inside;
                }
            }

            public override string ToString()
            {
                if (left)
                    return "[A " + point + ", B " + other.point + "]";
                return "[A " + other.point + ", B " + point + "]";
            }
        }

        private class SweepRay
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
                    if (s[i].point.y > e.point.y)
                    {
                        s.Insert(i, e);
                        Debug.Log("Inserted " + e.point + " at " + i);
                        return i;
                    }
                }
                s.Add(e);
                Debug.Log("Added " + e);
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
                    result += ((se.left)?"l":"r")+se.ToString() + ", ";
                return result;
            }
        }
    }
}
