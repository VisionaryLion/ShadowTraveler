using UnityEngine;
using System.Collections.Generic;
using Utility;

namespace NavMesh2D.Core
{
    public class CollisionGeometrySet
    {
        public List<Vector2[]> colliderVerts;
        public List<string> colliderNames;
        public List<Vector2[]> edgeVerts;
        public List<Vector2[]> dynamicColliderVerts;
        public List<Vector2[]> dynamicEdgeVerts;

        public CollisionGeometrySet()
        {
            colliderVerts = new List<Vector2[]>(20);
            colliderNames = new List<string>();
            edgeVerts = new List<Vector2[]>(5);
            dynamicColliderVerts = new List<Vector2[]>(5);
            dynamicEdgeVerts = new List<Vector2[]>(5);
        }

        public void AddEdge(List<Vector2> verts)
        {
            edgeVerts.Add(verts.ToArray());
        }

        public void AddCollider(List<Vector2> verts, string name)
        {
            colliderVerts.Add(verts.ToArray());
            colliderNames.Add(name);
        }

        public void AddDynamicCollider(List<Vector2> verts)
        {
            dynamicColliderVerts.Add(verts.ToArray());
        }

        public void AddDynamicEdge(List<Vector2> verts)
        {
            dynamicEdgeVerts.Add(verts.ToArray());
        }

        public void DrawDebugInfo()
        {
            Gizmos.color = Color.blue;
            for (int iVertList = 0; iVertList < colliderVerts.Count; iVertList++)
            {
                for (int iVert = 0; iVert < colliderVerts[iVertList].Length - 1; iVert++)
                {
                    OribowsUtilitys.DrawLine(colliderVerts[iVertList][iVert], colliderVerts[iVertList][iVert + 1], 2);
                    Gizmos.DrawWireSphere(colliderVerts[iVertList][iVert], 0.1f);
                }
                OribowsUtilitys.DrawLine(colliderVerts[iVertList][0], colliderVerts[iVertList][colliderVerts[iVertList].Length - 1], 2);
                Gizmos.DrawWireSphere(colliderVerts[iVertList][colliderVerts[iVertList].Length - 1], 0.1f);
            }

            Gizmos.color = Color.green;
            for (int iVertList = 0; iVertList < edgeVerts.Count; iVertList++)
            {
                for (int iVert = 0; iVert < edgeVerts[iVertList].Length - 1; iVert++)
                {
                    OribowsUtilitys.DrawLine(edgeVerts[iVertList][iVert], edgeVerts[iVertList][iVert + 1], 2);
                    Gizmos.DrawWireSphere(edgeVerts[iVertList][iVert], 0.1f);
                }
            }

            Gizmos.color = Color.red;
            for (int iVertList = 0; iVertList < dynamicColliderVerts.Count; iVertList++)
            {
                for (int iVert = 0; iVert < dynamicColliderVerts[iVertList].Length - 1; iVert++)
                {
                    OribowsUtilitys.DrawLine(dynamicColliderVerts[iVertList][iVert], dynamicColliderVerts[iVertList][iVert + 1], 2);
                    Gizmos.DrawWireSphere(dynamicColliderVerts[iVertList][iVert], 0.1f);
                }
                OribowsUtilitys.DrawLine(dynamicColliderVerts[iVertList][0], dynamicColliderVerts[iVertList][colliderVerts[iVertList].Length - 1], 2);
                Gizmos.DrawWireSphere(dynamicColliderVerts[iVertList][colliderVerts[iVertList].Length - 1], 0.1f);
            }

            for (int iVertList = 0; iVertList < dynamicEdgeVerts.Count; iVertList++)
            {
                for (int iVert = 0; iVert < dynamicEdgeVerts[iVertList].Length - 1; iVert++)
                {
                    OribowsUtilitys.DrawLine(dynamicEdgeVerts[iVertList][iVert], dynamicEdgeVerts[iVertList][iVert + 1], 2);
                    Gizmos.DrawWireSphere(dynamicEdgeVerts[iVertList][iVert], 0.1f);
                }
            }
        }
    }
}
