using UnityEngine;
using System.Collections.Generic;

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

        public void VisualDebug()
        {
            for (int iVertList = 0; iVertList < colliderVerts.Count; iVertList++)
            {
                for (int iVert = 0; iVert < colliderVerts[iVertList].Length - 1; iVert++)
                {
                    DebugExtension.DebugArrow(colliderVerts[iVertList][iVert], colliderVerts[iVertList][iVert + 1] - colliderVerts[iVertList][iVert]);
                    DebugExtension.DebugCircle(colliderVerts[iVertList][iVert], Vector3.forward, 0.1f);
                }

                DebugExtension.DebugArrow(colliderVerts[iVertList][colliderVerts[iVertList].Length - 1], colliderVerts[iVertList][0] - colliderVerts[iVertList][colliderVerts[iVertList].Length - 1]);
                DebugExtension.DebugCircle(colliderVerts[iVertList][colliderVerts[iVertList].Length - 1], Vector3.forward, 0.1f);
            }

            for (int iVertList = 0; iVertList < edgeVerts.Count; iVertList++)
            {
                for (int iVert = 0; iVert < edgeVerts[iVertList].Length - 1; iVert++)
                {
                    Debug.DrawLine(edgeVerts[iVertList][iVert], edgeVerts[iVertList][iVert + 1]);
                    DebugExtension.DebugCircle(edgeVerts[iVertList][iVert], Vector3.forward, 0.1f);
                }
            }
        }
    }
}
