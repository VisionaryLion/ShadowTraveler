using UnityEngine;
using System.Collections.Generic;
using Utility;
using NavMesh2D;

namespace NavMesh2D.Core
{
    public static class NavigationData2DBuilder
    {
        const float fudgeFactor = 0.00001f;

        public static NavigationData2D Build(ExpandedTree expandedTree, int testIndex)
        {
            List<NavNode> navNodes = new List<NavNode>(10); //-> make it not arbitrary!!
            foreach (ExpandedNode n in expandedTree.headNode.children)
                HandleMarkableContour(n, navNodes, testIndex, 1);
            NavNode[] allNN = navNodes.ToArray();
            navNodes = null;
            GenerateFloorWalkerJumpLinks(allNN, 40, 9, 10, 5, 2, 40);
            return new NavigationData2D { nodes = allNN };
        }

        private static void HandleMarkableContour(ExpandedNode exNode, List<NavNode> nodes, int testIndex, int hierachyIndex)
        {
            nodes.AddRange(InsertMarkableContour(exNode.contour, testIndex, hierachyIndex));
            foreach (ExpandedNode n in exNode.children)
                HandleMarkableContour(n, nodes, testIndex, hierachyIndex + 1);
        }

        private static NavNode[] InsertMarkableContour(MarkableContour mc, int testIndex, int hierachyIndex)
        {
            //Some debuging
            DebugExtension.DebugArrow(mc.firstPoint.pointB, mc.firstPoint.pointC - mc.firstPoint.pointB);

            //Find start point
            PointNode startPointNode;
            PointNode.ObstructedSegment cObstrSe = null;
            bool lastNode;

            foreach (PointNode pn in mc)
            {
                if (!pn.IsPointWalkable(testIndex))
                {
                    startPointNode = pn;
                    lastNode = true;
                    goto ObstructionFound;
                }
                else if (pn.FirstObstructedSegment != null)
                {
                    startPointNode = pn;
                    cObstrSe = pn.FirstObstructedSegment;
                    lastNode = false;
                    goto ObstructionFound;
                }
            }

            //No obstruction found
            return HandleObstructionLessContour(mc, hierachyIndex);

        ObstructionFound:

            //startPointNode points now at the first obstruction
            List<NavVert> vertBuffer = new List<NavVert>(mc.pointNodeCount);
            List<NavNode> nodes = new List<NavNode>(2);

            PointNode cPN = startPointNode;

            Bounds bounds = new Bounds();
            int safetyCounter = 0;

            while (safetyCounter < 10000)
            {
                if (cObstrSe == null)
                {
                    if (cPN == startPointNode)
                    {
                        lastNode = !lastNode;
                    }
                    if (cPN.FirstObstructedSegment == null)
                    {
                        bounds = new Bounds(cPN.pointB, Vector3.zero);
                        float angle = Vector2.Angle(Vector2.up, cPN.tangentBC) * Mathf.Deg2Rad;
                        if (Vector3.Cross(Vector2.up, cPN.tangentBC).z < 0)
                            angle = Mathf.PI * 2 - angle;
                        vertBuffer.Add(new NavVert(cPN.pointB, cPN.angle, angle, cPN.distanceBC));
                        bounds.Encapsulate(cPN.pointB);
                        if (cPN.Next == null || lastNode)
                        {
                            if (vertBuffer.Count > 1)
                            {
                                nodes.Add(new NavNode(vertBuffer.ToArray(), bounds, false, hierachyIndex));
                                vertBuffer.Clear();
                            }
                            break;
                        }
                        cPN = cPN.Next;
                    }
                    else
                    {
                        cObstrSe = cPN.FirstObstructedSegment;
                        if (vertBuffer.Count != 0)
                        {
                            if (!cPN.IsPointWalkable(testIndex))
                            {
                                vertBuffer.Add(new NavVert(cPN.pointB));
                                bounds.Encapsulate(cPN.pointB);
                                nodes.Add(new NavNode(vertBuffer.ToArray(), bounds, false, hierachyIndex));
                                vertBuffer.Clear();
                                if (lastNode)
                                    break;
                            }
                            else if (cObstrSe.start == 0)
                            {
                                vertBuffer.Add(new NavVert(cPN.pointB));
                                bounds.Encapsulate(cPN.pointB);
                                nodes.Add(new NavNode(vertBuffer.ToArray(), bounds, false, hierachyIndex));
                                vertBuffer.Clear();
                                if (lastNode)
                                    break;
                            }
                            else
                            {
                                float angle = Vector2.Angle(Vector2.up, cPN.tangentBC) * Mathf.Deg2Rad;
                                if (Vector3.Cross(Vector2.up, cPN.tangentBC).z < 0)
                                    angle = Mathf.PI * 2 - angle;
                                vertBuffer.Add(new NavVert(cPN.pointB, cPN.angle, angle, cPN.distanceBC));
                                bounds.Encapsulate(cPN.pointB);
                                Vector2 endPoint = cPN.pointB + cObstrSe.start * cPN.tangentBC;
                                vertBuffer.Add(new NavVert(endPoint));
                                bounds.Encapsulate(endPoint);
                                nodes.Add(new NavNode(vertBuffer.ToArray(), bounds, false, hierachyIndex));
                                vertBuffer.Clear();
                                if (lastNode)
                                    break;
                            }
                        }
                        else if (cObstrSe.start > 0)
                        {
                            float angle = Vector2.Angle(Vector2.up, cPN.tangentBC) * Mathf.Deg2Rad;
                            if (Vector3.Cross(Vector2.up, cPN.tangentBC).z < 0)
                                angle = Mathf.PI * 2 - angle;
                            vertBuffer.Add(new NavVert(cPN.pointB, cPN.angle, angle, cPN.distanceBC));
                            bounds.Encapsulate(cPN.pointB);
                            Vector2 endPoint = cPN.pointB + cObstrSe.start * cPN.tangentBC;
                            vertBuffer.Add(new NavVert(endPoint));
                            bounds.Encapsulate(endPoint);
                            nodes.Add(new NavNode(vertBuffer.ToArray(), bounds, false, hierachyIndex));
                            vertBuffer.Clear();
                            if (lastNode)
                                break;
                        }
                    }
                }
                else
                {
                    if (cObstrSe.end - cPN.distanceBC < fudgeFactor)
                    {
                        //End this chain!!
                        cObstrSe = null;
                        if (lastNode)
                            break;
                        cPN = cPN.Next;
                    }
                    else
                    {

                        Vector2 endPoint = cPN.pointB + cObstrSe.end * cPN.tangentBC;
                        float angle = Vector2.Angle(Vector2.up, cPN.tangentBC) * Mathf.Deg2Rad;
                        if (Vector3.Cross(Vector2.up, cPN.tangentBC).z < 0)
                            angle = Mathf.PI * 2 - angle;
                        vertBuffer.Add(new NavVert(endPoint, cPN.angle, angle, cPN.distanceBC));
                        bounds.Encapsulate(endPoint);
                        if (cObstrSe.next == null)
                        {
                            cObstrSe = null;
                            cPN = cPN.Next;
                        }
                        else
                        {
                            cObstrSe = cObstrSe.next;
                            endPoint = cPN.pointB + cObstrSe.start * cPN.tangentBC;
                            vertBuffer.Add(new NavVert(endPoint));
                            bounds.Encapsulate(endPoint);
                            nodes.Add(new NavNode(vertBuffer.ToArray(), bounds, false, hierachyIndex));
                            vertBuffer.Clear();
                        }
                    }
                }
                safetyCounter++;
            }
            if (safetyCounter >= 10000)
                Debug.Log("Endless Loop!! Mode = " + mc.isClosed + " lastNode =" + lastNode + " vertCount =" + vertBuffer.Count + " nodes.Count = " + nodes.Count);
            return nodes.ToArray();
        }

        private static NavNode[] HandleObstructionLessContour(MarkableContour mc, int hierachyIndex)
        {
            NavVert[] allNavNodes = new NavVert[mc.pointNodeCount];
            int counter = 0;
            Bounds bounds = new Bounds(mc.firstPoint.pointB, Vector3.zero);
            DebugExtension.DebugCircle(mc.firstPoint.pointB, Vector3.forward, Color.red, 0.5f);
            foreach (PointNode pn in mc)
            {
                float angle = Vector2.Angle(Vector2.right, pn.tangentBC) * Mathf.Deg2Rad;
                if (Vector3.Cross(Vector2.up, pn.tangentBC).z < 0)
                    angle = Mathf.PI * 2 - angle;
                allNavNodes[counter] = new NavVert(pn.pointB, pn.angle, angle, pn.distanceBC);
                bounds.Encapsulate(pn.pointB);
                counter++;
            }
            NavNode resultingNode = new NavNode(allNavNodes, bounds, mc.isClosed, hierachyIndex);
            return new NavNode[] { resultingNode };
        }

        private static void GenerateFloorWalkerJumpLinks(NavNode[] nodes, float maxJumpVel, float gravity, float xVel, float minJumpableHeight, float agentWidth, float upperXBound)
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
                                    iTargetNode+=2;
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

                                //debug this arc:
                                float lowerBound = (targetNV.PointB.x < srcNV.PointB.x) ? targetNV.PointB.x : srcNV.PointB.x;
                                float uppperBound = (targetNV.PointB.x < srcNV.PointB.x) ? srcNV.PointB.x : targetNV.PointB.x;
                                //DebugExtension.DebugArrow(nv.PointB, targetNV.PointB - nv.PointB, debugColor);
                                new JumpArc(targetJ, gravity, xVel, srcNV.PointB, lowerBound, uppperBound).DrawForDebug(Color.green);
                            //end debug

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

        private static bool IsJumpArcValid(float targetT, float targetJ, NavVert src, NavVert src2, int srcNodeIndex, NavVert dst, NavVert dst2, int dstNodeIndex, bool[] boundsIntersectionMap, NavNode[] nodes)
        {
            //first check the two aligning sides for both src and dst
            return true;
        }

        class JumpArc
        {
            float j, g, v, doubleG;
            Vector2 origin;
            float minX, maxX;
            float minY, maxY;

            public JumpArc(float j, float g, float v, Vector2 origin, float lowerXBound, float upperXBound)
            {
                this.j = j;
                this.g = g;
                this.v = v;
                this.origin = origin;
                minX = lowerXBound;
                maxX = upperXBound;
                minY = Calc(minX);
                doubleG = g * 2;
                maxY = (j * j) / (4 * doubleG) + origin.y;
            }

            public float Calc(float x)
            {
                x -= origin.x;
                x /= v;
                return (j - g * x) * x + origin.y;
            }

            public bool IntersectsWithSegment(Segment seg)
            {
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
                }
                return false;
            }

            public void DrawForDebug(Color color)
            {
                Vector2 swapPos;
                Vector2 prevPos = new Vector2(minX, Calc(minX));
                for (float x = minX; x + 0.1f < maxX; x += 0.1f)
                {
                    swapPos = new Vector2(x, Calc(x));
                    Debug.DrawLine(prevPos, swapPos, color);
                    prevPos = swapPos;
                }
                Debug.DrawLine(prevPos, new Vector2(maxX, Calc(maxX)), color);

            }
        }

        class Segment
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

            public void DrawForDebug()
            {
                Debug.DrawLine(start, end);
            }
        }
    }
}
