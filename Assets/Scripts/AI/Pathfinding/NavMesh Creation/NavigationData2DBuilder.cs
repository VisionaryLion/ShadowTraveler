//#define DEBUG_JUMP_LINKS

using UnityEngine;
using System.Collections.Generic;
using System;

namespace NavMesh2D.Core
{
    public class NavigationData2DBuilder
    {
        const float fudgeFactor = 0.00001f;
        NavAgentGroundWalkerSettings agentSettings;

        public NavigationData2DBuilder(NavAgentGroundWalkerSettings agentSettings)
        {
            this.agentSettings = agentSettings;
        }

        public NavigationData2D Build(ExpandedTree expandedTree)
        {
            List<NavNode> navNodes = new List<NavNode>(10); //-> make it not arbitrary!!
            foreach (ExpandedNode n in expandedTree.headNode.children)
                HandleMarkableContour(n, navNodes, 1);
            NavNode[] allNN = navNodes.ToArray();
            navNodes = null;
            GenerateFloorWalkerJumpLinks(allNN, 20, 20, 5, 5, 2, 40);
            NavigationData2D result =  ScriptableObject.CreateInstance<NavigationData2D>();
            result.nodes = allNN;
            return result;
        }

        private void HandleMarkableContour(ExpandedNode exNode, List<NavNode> nodes, int hierachyIndex)
        {
            ConvertMarkableContour(nodes, exNode.contour, hierachyIndex);
            foreach (ExpandedNode n in exNode.children)
                HandleMarkableContour(n, nodes, hierachyIndex + 1);
        }

        private void ConvertMarkableContour(List<NavNode> inOutNodes, MarkableContour mc, int hierachyIndex)
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
            List<NavVert> vertBuffer = new List<NavVert>(mc.pointNodeCount);
            Bounds inoutBounds = new Bounds(startPointNode.pointB, Vector3.zero);

            PointNode cPN = startPointNode;
            do
            {
                HandleEdge(cPN, inOutNodes, vertBuffer, ref inoutBounds, ref isClosed);
            } while ((cPN = cPN.Next) != startPointNode);


            HandleEdge(cPN, inOutNodes, vertBuffer, ref inoutBounds, ref isClosed);
            if (vertBuffer.Count > 1)
            {
                inOutNodes.Add(new NavNode(vertBuffer.ToArray(), inoutBounds, isClosed, hierachyIndex));
            }
        }

        private void HandleEdge(PointNode edge, List<NavNode> inoutNodes, List<NavVert> inoutVerts, ref Bounds inoutBounds, ref bool isClosed)
        {
            PointNode.ObstructedSegment obstrSeg = edge.FirstObstructedSegment;
            Vector2 startPoint = edge.pointB;

            if (!IsEdgeAcceptable(edge))
            {
                inoutVerts.Add(new NavVert(startPoint));
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
                inoutVerts.Add(new NavVert(startPoint));
                inoutBounds.Encapsulate(startPoint);
                return;
            }
            else if (obstrSeg.start == 0)
            {
                inoutVerts.Add(new NavVert(startPoint));
                inoutBounds.Encapsulate(startPoint);
                EndNavNode(inoutNodes, inoutVerts, ref inoutBounds, startPoint + edge.tangentBC * obstrSeg.end);
                if (obstrSeg.next == null)
                {
                    if (edge.distanceBC - obstrSeg.end > fudgeFactor)
                    {
                        startPoint = edge.pointB + edge.tangentBC * obstrSeg.end;
                        inoutVerts.Add(new NavVert(startPoint));
                        inoutBounds.Encapsulate(startPoint);
                    }
                    return;
                }
                obstrSeg = obstrSeg.next;
            }

            while (true)
            {
                inoutVerts.Add(new NavVert(startPoint));
                inoutBounds.Encapsulate(startPoint);
                inoutVerts.Add(new NavVert(edge.pointB + edge.tangentBC * obstrSeg.start));
                inoutBounds.Encapsulate(edge.pointB + edge.tangentBC * obstrSeg.start);
                startPoint = edge.pointB + edge.tangentBC * obstrSeg.end;
                EndNavNode(inoutNodes, inoutVerts, ref inoutBounds, startPoint);

                if (obstrSeg.next == null)
                    break;

                obstrSeg = obstrSeg.next;
            }

            if (edge.distanceBC - obstrSeg.end > fudgeFactor)
            {
                inoutVerts.Add(new NavVert(startPoint));
                inoutBounds.Encapsulate(startPoint);
            }
        }

        private void EndNavNode(List<NavNode> inoutNodes, List<NavVert> inoutVerts, ref Bounds inoutBounds, Vector2 nextPoint)
        {
            if (inoutVerts.Count > 1)
            {
                inoutNodes.Add(new NavNode(inoutVerts.ToArray(), inoutBounds, false, 0));
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

        private void GenerateFloorWalkerJumpLinks(NavNode[] nodes, float maxJumpVel, float gravity, float xVel, float minJumpableHeight, float agentWidth, float upperXBound)
        {
            float maxY = ((maxJumpVel * maxJumpVel) / (2 * gravity));
            float maxX = upperXBound * xVel * 2;
            float minY = (maxJumpVel - gravity * upperXBound) * upperXBound;

            //Go through all nodes
            foreach (NavNode srcNN in nodes)
            {
                NavVert prevSrcNV = (srcNN.isClosed) ? srcNN.verts[srcNN.verts.Length - 1] : srcNN.verts[0];
                for (int iVert = 0; iVert < srcNN.verts.Length; iVert++)
                {
                    NavVert srcNV = srcNN.verts[iVert];

                    //Test if segment is A: ground angle of less then pi 
                    if (srcNV.slopeAngleBC <= Mathf.PI && prevSrcNV.slopeAngleBC <= Mathf.PI)
                    {
                        Debug.DrawLine(prevSrcNV.PointB, srcNV.PointB, Color.red);
                        goto SrcNVLoopEnd;
                    }

                    //point found, now find all possible connection points!
                    Bounds maxPossibleArcBound = new Bounds(srcNV.PointB, new Vector2(maxX, maxY));
                    maxPossibleArcBound.min = new Vector2(maxPossibleArcBound.min.x, srcNV.PointB.y + minY);
                    DebugExtension.DebugBounds(maxPossibleArcBound, Color.blue);

                    for (int iTargetNode = 0; iTargetNode < nodes.Length; iTargetNode++)
                    {
                        NavNode targetNN = nodes[iTargetNode];

                        //Do the Bounds overlap?
                        if (targetNN.min.x < maxPossibleArcBound.max.x && targetNN.max.x > maxPossibleArcBound.min.x && targetNN.min.y < maxPossibleArcBound.max.y && targetNN.max.y > maxPossibleArcBound.min.y)
                        {
                            NavVert prevTargetNV = (targetNN.isClosed) ? targetNN.verts[targetNN.verts.Length - 1] : targetNN.verts[0];

                            for (int iTargetNV = 0; iTargetNV < targetNN.verts.Length; iTargetNV++)
                            {
                                //Try to construct a jump arc
                                NavVert targetNV = targetNN.verts[iTargetNV];
                                if (targetNV == srcNV)
                                {
                                    iTargetNode++;
                                    if (iTargetNode + 1 >= targetNN.verts.Length)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        prevTargetNV = targetNN.verts[iTargetNode];
                                        continue;
                                    }
                                }
                                else if (targetNV == prevSrcNV)
                                {
                                    iTargetNode += 2;
                                    if (iTargetNode + 1 >= targetNN.verts.Length)
                                        break;
                                    else
                                        prevTargetNV = targetNN.verts[iTargetNode];
                                    continue;
                                }

                                if (targetNV.slopeAngleBC <= Mathf.PI && prevTargetNV.slopeAngleBC <= Mathf.PI)
                                    goto TargetNVLoopEnd; //Not walkable surface

                                if (!maxPossibleArcBound.Contains(targetNV.PointB))
                                    goto TargetNVLoopEnd; //targetNV out of bounds

                                float targetT = (targetNV.PointB.x - srcNV.PointB.x) / xVel;
                                float targetJ = ((targetNV.PointB.y - srcNV.PointB.y) / targetT) + gravity * targetT;
                                if (Mathf.Abs(targetJ) > maxJumpVel)
                                    goto TargetNVLoopEnd; //the required jumpforce exceeds limit

#if DEBUG_JUMP_LINKS
                                //debug this arc:
                                float lowerBound = (targetNV.PointB.x < srcNV.PointB.x) ? targetNV.PointB.x : srcNV.PointB.x;
                                float uppperBound = (targetNV.PointB.x < srcNV.PointB.x) ? srcNV.PointB.x : targetNV.PointB.x;

                                new JumpArcSegment(targetJ, gravity, xVel, lowerBound - srcNV.PointB.x, uppperBound - srcNV.PointB.x).VisualDebug(srcNV.PointB, Color.green);
#endif

                                TargetNVLoopEnd:
                                prevTargetNV = targetNV;
                            }

                        }
                    }
                    SrcNVLoopEnd:
                    prevSrcNV = srcNV;
                }
            }
        }

        private bool IsJumpArcValid(float targetT, float targetJ, NavVert src, NavVert src2, int srcNodeIndex, NavVert dst, NavVert dst2, int dstNodeIndex, bool[] boundsIntersectionMap, NavNode[] nodes)
        {
            //first check the two aligning sides for both src and dst
            return true;
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

        public float Calc(float x)
        {
            x /= v;
            return (j - halfG * x) * x;
        }

        public bool IntersectsWithSegment(NavigationData2DBuilder.Segment seg, Vector2 arcOrigin)
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
