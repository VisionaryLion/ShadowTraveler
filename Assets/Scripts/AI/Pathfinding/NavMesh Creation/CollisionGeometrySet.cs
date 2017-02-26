using UnityEngine;
using System.Collections.Generic;
using System;

namespace NavMesh2D.Core
{
    public class CollisionGeometrySet
    {
        public List<Vector2d[]> colliderVerts;
        public List<Vector2d[]> edgeVerts;

        public CollisionGeometrySet()
        {
            colliderVerts = new List<Vector2d[]>(20);
            edgeVerts = new List<Vector2d[]>(5);
        }

        public void AddEdge(List<Vector2> verts)
        {
            edgeVerts.Add(Array.ConvertAll(verts.ToArray(), (item) => (Vector2d)item));
        }

        public void AddCollider(List<Vector2> verts)
        {
            colliderVerts.Add(Array.ConvertAll(verts.ToArray(), (item) => (Vector2d)item));
        }

        public void AddEdge(List<Vector2d> verts)
        {
            edgeVerts.Add(verts.ToArray());
        }

        public void AddCollider(List<Vector2d> verts)
        {
            colliderVerts.Add(verts.ToArray());
        }

        public void VisualDebug()
        {
            for (int iVertList = 0; iVertList < colliderVerts.Count; iVertList++)
            {
                for (int iVert = 0; iVert < colliderVerts[iVertList].Length - 1; iVert++)
                {
                    DebugExtension.DebugArrow((Vector2)colliderVerts[iVertList][iVert], (Vector2)(colliderVerts[iVertList][iVert + 1] - colliderVerts[iVertList][iVert]));
                    DebugExtension.DebugCircle((Vector2)colliderVerts[iVertList][iVert], Vector3.forward, 0.1f);
                }

                DebugExtension.DebugArrow((Vector2)colliderVerts[iVertList][colliderVerts[iVertList].Length - 1], (Vector2)(colliderVerts[iVertList][0] - colliderVerts[iVertList][colliderVerts[iVertList].Length - 1]));
                DebugExtension.DebugCircle((Vector2)colliderVerts[iVertList][colliderVerts[iVertList].Length - 1], Vector3.forward, 0.1f);
            }

            for (int iVertList = 0; iVertList < edgeVerts.Count; iVertList++)
            {
                for (int iVert = 0; iVert < edgeVerts[iVertList].Length - 1; iVert++)
                {
                    Debug.DrawLine((Vector2)edgeVerts[iVertList][iVert], (Vector2)edgeVerts[iVertList][iVert + 1]);
                    DebugExtension.DebugCircle((Vector2)edgeVerts[iVertList][iVert], Vector3.forward, 0.1f);
                }
            }
        }
    }
}
