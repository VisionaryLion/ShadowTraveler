using UnityEngine;
using System.Collections.Generic;
using Utility;

namespace NavMesh2D
{
    public class NavigationData2D : ScriptableObject
    {

        //identifiers
        public new string name;
        public int version;

        public NavNode[] nodes; // sorted by x Value. Min -> Max

        /*public NavRayCastHit RayCast(Vector2 pos, Vector2 dir, float length)
                {
                    Vector2 endPos = dir * length + pos;
                    float minX = Mathf.Min(pos.x, endPos.x);
                    float maxX = Mathf.Max(pos.x, endPos.x);

                    float minY = Mathf.Min(pos.y, endPos.y);
                    float maxY = Mathf.Min(pos.y, endPos.y);

                    float m = (dir.x == 0) ? 0 : dir.x / dir.y;

                    foreach (NavNode nn in nodes)
                    {
                        if (nn.max.x < minX)
                            continue;
                        if (nn.min.x > maxX)
                            break;
                        if (nn.min.y > maxY || nn.max.y < minY)
                            continue;

                        if (dir.x != 0) // m != 0
                        {
                            float startY = m * nn.min.x + pos.y;
                            float endY = m * nn.max.x + pos.y;

                            if (startY > endY)
                            {
                                if (startY > maxY)
                                    continue;
                                if (endY < minY)
                                    continue;
                            }
                            else
                            {
                                if (startY < maxY)
                                    continue;
                                if (endY > minY)
                                    continue;
                            }
                        }

                        //Bounds passed all tests. A intersection is possible

                    }
                }*/

        public void DrawForDebug()
        {
            int counter = 1;
            foreach (NavNode n in nodes)
            {
                n.DrawForDebug(counter++);
            }
        }
    }

    public class NavNode
    {
        const float maxDeviationInside = 0.1f;
        const float maxDeviationOutside = 0.001f;

        public readonly Vector2 min;
        public readonly Vector2 max;
        public readonly bool isClosed;
        public readonly int hierachyIndex; // 0 = hole, 1 = solid, 2 = hole, 3 = solid, ...

        public bool IsSolid { get { return hierachyIndex % 2 == 0; } }

        NavNodeLink[] links;
        public NavVert[] verts;
       

        public NavNode(NavVert[] verts, Bounds bounds, bool isClosed, int hierachyIndex)
        {
            this.verts = verts;
            min = bounds.min;
            max = bounds.max;
            this.isClosed = isClosed;
            this.hierachyIndex = hierachyIndex;
        }

        public void DrawForDebug(int colorId)
        {
            if (verts.Length == 0)
                Debug.Log("Node [" + colorId + "] is empty.");
            if (verts.Length == 1)
                Debug.Log("Node [" + colorId + "] is too small. " + verts[0].PointB);

            for (int iVert = 0; iVert < verts.Length - 1; iVert++)
            {
                if (verts[iVert].PointB == verts[iVert + 1].PointB)
                {
                    Debug.Log("Node [" + colorId + "] has edge with zero length. " + verts[iVert].PointB + ", totalVertCount = " + verts.Length);
                    DebugExtension.DebugPoint(verts[iVert].PointB, Color.magenta);
                }
                //float angleGrad = (1 / Mathf.PI) * verts[iVert].slopeAngleBC;
                //DebugExtension.DrawArrow(verts[iVert].PointB, verts[iVert + 1].PointB - verts[iVert].PointB, DifferentColors.GetColor(colorId));
                //Debug.DrawLine(verts[iVert].PointB, verts[iVert + 1].PointB, DifferentColors.GetColor(colorId));
                DebugExtension.DrawCircle(verts[iVert].PointB, Vector3.forward, DifferentColors.GetColor(colorId), 0.2f);
            }
            if (isClosed)
            {
                //Debug.DrawLine(verts[verts.Length -1].PointB, verts[0].PointB, DifferentColors.GetColor(colorId));
                //float angleGrad = (1 / Mathf.PI) * verts[verts.Length - 1].slopeAngleBC;
                //DebugExtension.DrawArrow(verts[verts.Length - 1].PointB, verts[0].PointB - verts[verts.Length - 1].PointB, DifferentColors.GetColor(colorId));
            }
            DebugExtension.DrawCircle(verts[verts.Length - 1].PointB, Vector3.forward, DifferentColors.GetColor(colorId), 0.2f);
        }
    }

    public class NavNodeLink
    {
        public enum LinkType { NotAccessible, Jump, Ladder, };
        NavNode target;
        PathRequirements[] isReachableFrom;
        Vector2 startPoint;
        Vector2 endPoint;
        LinkType linkType;
    }

    public class NavVert
    {
        public Vector2 PointB { get { return pointB; } }

        public DynamicObstruction firstObstruction;

        public readonly float angleABC;
        public readonly float slopeAngleBC;
        public readonly float distanceBC;

        Vector2 pointB; // a -> b -> c
        NavNodeLink[] links;

        public NavVert(Vector2 point, float angleABC, float slopeAngleBC, float distanceBC)
        {
            this.pointB = point;
            this.angleABC = angleABC;
            this.slopeAngleBC = slopeAngleBC;
            this.distanceBC = distanceBC;
        }

        public NavVert(Vector2 point)
        {
            this.pointB = point;
        }
    }

    public class DynamicObstruction
    {

    }

    public class PathRequirements
    {
        float minSlope;
        bool useRelativeSlopeAngle;
        bool isPathUnblocked;

    }

    class NavAgent
    {
        float maxWalkableSlope;
        bool useRelativeSlopeAngle;
        float height;
    }

    class NavRayCastHit
    {

    }
}
