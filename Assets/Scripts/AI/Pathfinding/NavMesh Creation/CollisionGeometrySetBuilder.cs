using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace NavMesh2D.Core
{
    public class CollisionGeometrySetBuilder
    {
        #region public

        public int CircleVertCount { get { return circleVertCount; } set { circleVertCount = Mathf.Max(3, value); } }
        public int WalkableColliderMask { get; set; }

        #endregion

        #region private

        private int circleVertCount;

        #endregion

        public CollisionGeometrySetBuilder(int circleVertCount, int walkableColliderMask)
        {
            CircleVertCount = circleVertCount;
            WalkableColliderMask = walkableColliderMask;
        }

        public CollisionGeometrySet Build (Collider2D[] collider)
        {
            CollisionGeometrySet result = new CollisionGeometrySet();
            List<Vector2> inOutVerts = new List<Vector2>(10); //Just a guess

            for (int iCol = 0; iCol < collider.Length; iCol++)
            {
                if (!PreScreenCollider(collider[iCol]))
                    continue;

                Type cTyp = collider[iCol].GetType();

                //Sort out any edge collider, as they will be processed differently.
                if (cTyp == typeof(EdgeCollider2D))
                {
                    LoadEdgeColliderVerts((EdgeCollider2D)collider[iCol], inOutVerts);
                    result.AddEdge(inOutVerts);
                } else
                {
                    if (cTyp == typeof(BoxCollider2D))
                        LoadBoxColliderVerts((BoxCollider2D)collider[iCol], inOutVerts);
                    else if (cTyp == typeof(CircleCollider2D))
                        LoadCircleColliderVerts((CircleCollider2D)collider[iCol], inOutVerts);
                    else
                        LoadPolygonColliderVerts((PolygonCollider2D)collider[iCol], inOutVerts);

                    result.AddCollider(inOutVerts);
                }
                inOutVerts.Clear();
            }
            return result;
        }

        private bool PreScreenCollider(Collider2D col)
        {
            if (WalkableColliderMask == -1 || col.gameObject.layer == (col.gameObject.layer | (1 << WalkableColliderMask)))
                return true;
            return false;
        }

        private void LoadBoxColliderVerts(BoxCollider2D collider, List<Vector2> inOutVerts)
        {
            Vector2 halfSize = collider.size / 2;
            inOutVerts.Add(collider.transform.TransformPoint(halfSize + collider.offset));
            inOutVerts.Add(collider.transform.TransformPoint(new Vector2(halfSize.x, -halfSize.y) + collider.offset));
            inOutVerts.Add(collider.transform.TransformPoint(-halfSize + collider.offset));
            inOutVerts.Add(collider.transform.TransformPoint(new Vector2(-halfSize.x, halfSize.y) + collider.offset));
        }

        private void LoadCircleColliderVerts(CircleCollider2D collider, List<Vector2> inOutVerts)
        {
            float anglePerCircleVert = (Mathf.PI * 2) / circleVertCount;
            for (int i = 0; i < circleVertCount; i++)
            {
                inOutVerts.Add(collider.transform.TransformPoint(new Vector2(collider.radius * Mathf.Sin(anglePerCircleVert * i), collider.radius * Mathf.Cos(anglePerCircleVert * i))));
            }
        }

        private void LoadPolygonColliderVerts(PolygonCollider2D collider, List<Vector2> inOutVerts)
        {
            Matrix4x4 localToWorld = collider.transform.localToWorldMatrix;
            for (int iVert = 0; iVert < collider.points.Length; iVert++)
            {
                inOutVerts.Add(localToWorld.MultiplyPoint(collider.points[iVert]));
            }
        }

        private void LoadEdgeColliderVerts(EdgeCollider2D collider, List<Vector2> inOutVerts)
        {
            Matrix4x4 localToWorld = collider.transform.localToWorldMatrix;
            for (int iVert = 0; iVert < collider.points.Length; iVert++)
            {
                inOutVerts.Add(localToWorld.MultiplyPoint(collider.points[iVert]));
            }
        }
    }
}
