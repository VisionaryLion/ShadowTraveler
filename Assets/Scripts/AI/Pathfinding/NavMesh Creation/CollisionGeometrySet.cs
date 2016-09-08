using UnityEngine;
using System.Collections.Generic;
using Utility;

namespace NavMesh2D.Core
{
    public class CollisionGeometrySet
    {
        public List<Vector2[]> colliderVerts;
        public List<Vector2[]> edgeVerts;

        public CollisionGeometrySet()
        {
            colliderVerts = new List<Vector2[]>(20);
            edgeVerts = new List<Vector2[]>(5);
        }

        public void AddEdge(List<Vector2> verts)
        {
            edgeVerts.Add(verts.ToArray());
        }

        public void AddCollider(List<Vector2> verts)
        {
            colliderVerts.Add(verts.ToArray());
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
        }
    }
}
