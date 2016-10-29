//#define DEBUG_JUMP_LINKS

using UnityEngine;
using System.Collections.Generic;
using System;

namespace NavMesh2D.Core
{
    public class RawNavigationData2DBuilder
    {
        const float fudgeFactor = 0.00001f;
        NavAgentGroundWalkerSettings agentSettings;

        public RawNavigationData2DBuilder(NavAgentGroundWalkerSettings agentSettings)
        {
            this.agentSettings = agentSettings;
        }

        public void Build(ExpandedTree expandedTree, RawNavigationData2D dst)
        {
            List<RawNavNode> navNodes = new List<RawNavNode>(10); //-> make it not arbitrary!!
            foreach (ExpandedNode n in expandedTree.headNode.children)
                HandleMarkableContour(n, navNodes, 1);
            RawNavNode[] allNN = navNodes.ToArray();
            navNodes = null;
            dst.nodes = allNN;
        }

        private void HandleMarkableContour(ExpandedNode exNode, List<RawNavNode> nodes, int hierachyIndex)
        {
            ConvertMarkableContour(nodes, exNode.contour, hierachyIndex);
            foreach (ExpandedNode n in exNode.children)
                HandleMarkableContour(n, nodes, hierachyIndex + 1);
        }

        private void ConvertMarkableContour(List<RawNavNode> inOutNodes, MarkableContour mc, int hierachyIndex)
        {
            //Some debuging
            DebugExtension.DebugArrow(mc.firstPoint.pointB, mc.firstPoint.pointC - mc.firstPoint.pointB);

            //Find start point
            PointNode startPointNode = mc.firstPoint;
            bool isClosed = true;

            foreach (PointNode pn in mc)
            {
                if (/*!pn.IsPointWalkable() ||*/ pn.FirstObstructedSegment != null || !IsEdgeAcceptable(pn))
                {
                    startPointNode = pn;
                    isClosed = false;
                    break;
                }
            }
            DebugExtension.DebugCircle(startPointNode.pointB, Vector3.forward, Color.magenta, 0.5f);
            //startPointNode points now at the first obstruction
            List<RawNavVert> vertBuffer = new List<RawNavVert>(mc.pointNodeCount);
            Bounds inoutBounds = new Bounds(startPointNode.pointB, Vector3.zero);

            PointNode cPN = startPointNode;
            do
            {
                HandleEdge(cPN, inOutNodes, vertBuffer, ref inoutBounds, ref isClosed);
            } while ((cPN = cPN.Next) != startPointNode);


            HandleEdge(cPN, inOutNodes, vertBuffer, ref inoutBounds, ref isClosed);
            if (vertBuffer.Count > 1)
            {
                inOutNodes.Add(new RawNavNode(vertBuffer.ToArray(), inoutBounds, isClosed, hierachyIndex));
            }
        }

        private void HandleEdge(PointNode edge, List<RawNavNode> inoutNodes, List<RawNavVert> inoutVerts, ref Bounds inoutBounds, ref bool isClosed)
        {
            PointNode.ObstructedSegment obstrSeg = edge.FirstObstructedSegment;
            Vector2 startPoint = edge.pointB;

            if (!IsEdgeAcceptable(edge))
            {
                inoutVerts.Add(new RawNavVert(startPoint));
                inoutBounds.Encapsulate(startPoint);
                if (edge.Next != null)
                    EndNavNode(inoutNodes, inoutVerts, ref inoutBounds, edge.Next.pointB);
                else
                    EndNavNode(inoutNodes, inoutVerts, ref inoutBounds, Vector3.zero);
                isClosed = false;
                return;
            }

            if (obstrSeg == null)
            {
                inoutVerts.Add(new RawNavVert(startPoint));
                inoutBounds.Encapsulate(startPoint);
                return;
            }
            else if (obstrSeg.start == 0)
            {
                inoutVerts.Add(new RawNavVert(startPoint));
                inoutBounds.Encapsulate(startPoint);
                EndNavNode(inoutNodes, inoutVerts, ref inoutBounds, startPoint + edge.tangentBC * obstrSeg.end);
                if (obstrSeg.next == null)
                {
                    if (edge.distanceBC - obstrSeg.end > fudgeFactor)
                    {
                        startPoint = edge.pointB + edge.tangentBC * obstrSeg.end;
                        inoutVerts.Add(new RawNavVert(startPoint));
                        inoutBounds.Encapsulate(startPoint);
                    }
                    return;
                }
                obstrSeg = obstrSeg.next;
            }

            while (true)
            {
                inoutVerts.Add(new RawNavVert(startPoint));
                inoutBounds.Encapsulate(startPoint);
                inoutVerts.Add(new RawNavVert(edge.pointB + edge.tangentBC * obstrSeg.start));
                inoutBounds.Encapsulate(edge.pointB + edge.tangentBC * obstrSeg.start);
                startPoint = edge.pointB + edge.tangentBC * obstrSeg.end;
                EndNavNode(inoutNodes, inoutVerts, ref inoutBounds, startPoint);

                if (obstrSeg.next == null)
                    break;

                obstrSeg = obstrSeg.next;
            }

            if (edge.distanceBC - obstrSeg.end > fudgeFactor)
            {
                inoutVerts.Add(new RawNavVert(startPoint));
                inoutBounds.Encapsulate(startPoint);
            }
        }

        private void EndNavNode(List<RawNavNode> inoutNodes, List<RawNavVert> inoutVerts, ref Bounds inoutBounds, Vector2 nextPoint)
        {
            if (inoutVerts.Count > 1)
            {
                inoutNodes.Add(new RawNavNode(inoutVerts.ToArray(), inoutBounds, false, 0));
            }
            inoutBounds = new Bounds(nextPoint, Vector3.zero);
            inoutVerts.Clear();
        }

        private bool IsEdgeAcceptable(PointNode edge)
        {
            if (true)
            {
                float angle = Vector2.Angle(Vector2.left, edge.tangentBC);
                /*if (Vector3.Cross(Vector2.left, edge.tangentBC).z < 0)
                    angle = 360 - angle;
                if (angle > 90)
                    angle = 180 - angle;*/
                if (angle > agentSettings.slopeLimit)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                /*
                if (pointNodeCount < 3 && !isClosed)
                    return;

                PointNode current = isClosed ? firstPoint : firstPoint.Next.Next;
                PointNode prevNode = current.Previous;
                PointNode prevPrevNode = prevNode.Previous;
                int edgeCount = isClosed ? pointNodeCount : pointNodeCount - 2;
                for (int iNode = 0; iNode < edgeCount; iNode++)
                {
                    if (prevNode.angle * Mathf.Rad2Deg > maxAngle)
                    {
                        //stage for remove
                    }

                    prevPrevNode = prevNode;
                    prevNode = current;
                    current = current.Next;
                }*/
            }
        }

        public class Segment
        {
            Vector2 start;
            Vector2 end;
            bool isStartXSmaller;
            bool isStartYSmaller;
            public readonly float m;
            public readonly float n;

            public float MaxX { get { return (isStartXSmaller) ? end.x : start.x; } }
            public float MinX { get { return (isStartXSmaller) ? start.x : end.x; } }
            public float MaxY { get { return (isStartYSmaller) ? end.y : start.y; } }
            public float MinY { get { return (isStartYSmaller) ? start.y : end.y; } }

            public Segment(Vector2 start, Vector2 end)
            {
                this.start = start;
                this.end = end;
                isStartXSmaller = start.x < end.x;
                isStartYSmaller = start.y < end.y;
                m = (end.y - start.y) / (end.x - start.x);
                n = start.y - m * start.x;
            }

            public bool IsPointOnSegment(float x)
            {
                return x >= MinX && x <= MaxX;
            }

            public float Calc(float x)
            {
                return m * x + n;
            }

            public void VisualDebug()
            {
                Debug.DrawLine(start, end);
            }
        }
    }

    [Serializable]
    public class JumpArcSegment
    {
        [SerializeField]
        public float j, halfG, v, doubleG;
        [SerializeField]
        public float minX, maxX;
        [SerializeField]
        public float startX, endX;
        [SerializeField]
        public float minY, maxY;

        public JumpArcSegment(float j, float g, float v, float startX, float endX)
        {
            this.j = j;
            this.halfG = g / 2;
            this.v = v;
            this.startX = startX;
            this.endX = endX;
            if (startX < endX)
            {
                minX = startX;
                maxX = endX;
            }
            else
            {
                minX = endX;
                maxX = startX;
            }
            minY = Mathf.Min(Calc(0), Calc(maxX - minX));
            doubleG = g * 2;
            maxY = (j * j) / (4 * doubleG);
        }

        public void UpdateArc(float j, float g, float v, float startX, float endX)
        {
            this.j = j;
            this.halfG = g / 2;
            this.v = v;
            this.startX = startX;
            this.endX = endX;
            if (startX < endX)
            {
                minX = startX;
                maxX = endX;
            }
            else
            {
                minX = endX;
                maxX = startX;
            }
            minY = Mathf.Min(Calc(0), Calc(maxX - minX));
            doubleG = g * 2;
            maxY = (j * j) / (4 * doubleG);
        }

        public float Calc(float x)
        {
            x /= v;
            return (j - halfG * x) * x;
        }

        public bool IntersectsWithSegment(RawNavigationData2DBuilder.Segment seg, Vector2 arcOrigin)
        {
            /*
            if (seg.MinX > maxX || seg.MaxX < minX || seg.MinY > maxY || seg.MaxY < minY)
                return false;

            float b = j - (seg.m / v);
            float det = (b * b) - (2 * doubleG * seg.n);
            if (det < 0)
                return false;

            b = -b / doubleG;
            det = Mathf.Sqrt(det) / doubleG;
            float x1 = b + det;
            float x2 = b - det;

            if (seg.IsPointOnSegment(x1))
            {
                if (x1 >= minX && x1 <= maxX)
                    return true;
            }
            else if (seg.IsPointOnSegment(x2))
            {
                if (x2 >= minX && x2 <= maxX)
                    return true;
            }*/
            return false;
        }

        public void VisualDebug(Vector2 origin, Color color)
        {
            Vector2 swapPos;
            Vector2 prevPos = new Vector2(minX, Calc(minX)) + origin;
            for (float x = minX; x + 0.1f < maxX; x += 0.1f)
            {
                swapPos = new Vector2(x, Calc(x)) + origin;
                Debug.DrawLine(prevPos, swapPos, color);
                prevPos = swapPos;
            }
            Debug.DrawLine(prevPos, new Vector2(maxX, Calc(maxX)) + origin, color);

        }
    }
}
