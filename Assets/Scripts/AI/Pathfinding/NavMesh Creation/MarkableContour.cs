using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NavMesh2D.Core
{
    public class MarkableContour : IEnumerable<PointNode>
    {

        public readonly Bounds bounds;
        public readonly bool isSolid;
        public readonly bool isClosed;
        public readonly int pointNodeCount;

        public bool IsEmpty { get { return pointNodeCount == 0; } }

        public PointNode firstPoint;

        public MarkableContour(Contour contour, bool isSolid, bool isClosed, int traversTestCount)
        {
            bounds = contour.Bounds;
            this.isSolid = isSolid;
            this.isClosed = isClosed;

            pointNodeCount = contour.VertexCount;
            PointNode cSeg = new PointNode(contour.verticies[0], traversTestCount);
            firstPoint = cSeg;
            for (int iVert = 1; iVert < contour.verticies.Count; iVert++)
            {
                cSeg.Next = new PointNode(contour.verticies[iVert], traversTestCount);
                cSeg.Next.Previous = cSeg;
                cSeg = cSeg.Next;
            }

            if (isClosed)
            {
                cSeg.Next = firstPoint;
                firstPoint.Previous = cSeg;
            }
        }

        public void Mark(List<ExpandedNode> cp, float minWalkableHeight, int testIndex)
        {
            WalkSpaceTester.MarkNotWalkableSegments(this, cp, minWalkableHeight, testIndex);
        }

        public void MarkSelfOnly(float minWalkableHeight, int testIndex)
        {
            WalkSpaceTester.MarkSelfIntersections(this, minWalkableHeight, testIndex);
        }

        public void VisualDebug(int cHeightTest)
        {
            foreach (PointNode s in this)
            {
                s.VisualDebug(cHeightTest);
            }
        }

        public IEnumerator<PointNode> GetEnumerator()
        {
            return new PointNodeEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new PointNodeEnumerator(this);
        }

        class PointNodeEnumerator : IEnumerator<PointNode>
        {
            MarkableContour target;
            int cIndex = 0;
            PointNode cSeg;

            public PointNodeEnumerator(MarkableContour target)
            {
                this.target = target;
                cSeg = target.firstPoint;
            }

            public PointNode Current
            {
                get
                {
                    return cSeg;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return cSeg;
                }
            }

            public void Dispose()
            {
                target = null;
                cSeg = null;
            }

            public bool MoveNext()
            {
                if (cIndex >= target.pointNodeCount)
                {
                    return false;
                }
                else
                {
                    cIndex++;
                    cSeg = cSeg.Next;
                    return true;
                }
            }

            public void Reset()
            {
                cIndex = 0;
                cSeg = target.firstPoint;
            }
        }
    }

    public class PointNode
    {
        public Vector2 pointB;
        public Vector2 pointC { get { return next.pointB; } }
        public Vector2 pointA { get { return prev.pointB; } }
        public PointNode Previous { get { return prev; } set { prev = value; if (next != null) CalcAngle(); } }
        public PointNode Next { get { return next; } set { next = value; PrecomputeVars(); if (prev != null) CalcAngle(); } }
        public ObstructedSegment FirstObstructedSegment { get { return obstructedSegment; } }

        //Precomputed information
        public float angle; // in rads
        public float distanceBC;
        public Vector2 tangentBC;

        PointNode prev;
        PointNode next;
        ObstructedSegment obstructedSegment;
        bool[] isPointWalkable;

        public PointNode(Vector2 point, int walkTestCount)
        {
            this.pointB = point;
            angle = -1;
            isPointWalkable = new bool[walkTestCount];
            for (int i = 0; i < walkTestCount; i++)
            {
                isPointWalkable[i] = true;
            }
        }

        public void AddObstruction(float start, float end)
        {
            if (start == end)
                return;
            if (start > end)
                Debug.Log("Violation of order!");
            ObstructedSegment cSeg = obstructedSegment;
            ObstructedSegment prevSeg = null;

            while (cSeg != null)
            {
                if (cSeg.start > end)
                {
                    ObstructedSegment newSeg = new ObstructedSegment(start, end);
                    newSeg.next = cSeg;
                    if (prevSeg != null)
                        prevSeg.next = newSeg;
                    else
                        obstructedSegment = newSeg;
                    return;
                }
                else if (cSeg.end >= start)
                {
                    start = Mathf.Min(start, cSeg.start);
                    end = Mathf.Max(end, cSeg.end);
                    if (prevSeg != null)
                        prevSeg.next = cSeg.next;
                    cSeg = cSeg.next;
                }
                else
                {
                    prevSeg = cSeg;
                    cSeg = cSeg.next;
                }
            }

            cSeg = new ObstructedSegment(start, end);
            if (prevSeg != null)
                prevSeg.next = cSeg;
            else
                obstructedSegment = cSeg;
        }

        public void MarkPointNotWalkable(int index)
        {
            isPointWalkable[index] = false;
        }

        public bool IsPointWalkable(int index)
        {
            return isPointWalkable[index];
        }

        public void VisualDebug(int walkTestIndex)
        {
            if (!isPointWalkable[walkTestIndex])
            {
                DebugExtension.DebugCircle(pointB, Vector3.forward, Color.red, 0.2f);
            }
            Debug.DrawLine(pointB, pointC, Color.green);
            ObstructedSegment pObSeg = obstructedSegment;
            while (pObSeg != null)
            {
                pObSeg.VisualDebug(this);
                pObSeg = pObSeg.next;
            }
        }


        private void CalcAngle()
        {
            Vector2 nA = pointA - pointB;
            Vector2 nB = pointC - pointB;
            angle = Vector2.Angle(nA, nB) * Mathf.Deg2Rad;
            if (Vector3.Cross(nA, nB).z < 0)
                angle = Mathf.PI * 2 - angle;
        }

        private void PrecomputeVars()
        {
            distanceBC = Vector2.Distance(pointB, pointC);
            tangentBC = (pointC - pointB).normalized;
        }

        public class ObstructedSegment
        {
            public float start;
            public float end;
            public ObstructedSegment next;

            public ObstructedSegment()
            {
                start = 0;
                end = 0;
            }

            public ObstructedSegment(float start, float end)
            {
                this.start = start;
                this.end = end;
            }

            public void VisualDebug(PointNode holder)
            {
                Debug.DrawLine(holder.pointB + holder.tangentBC * start, holder.pointB + holder.tangentBC * end, Color.red);
            }
        }
    }
}
