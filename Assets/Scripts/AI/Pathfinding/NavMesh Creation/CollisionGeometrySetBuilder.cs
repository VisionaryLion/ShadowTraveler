using UnityEngine;
using System.Collections.Generic;
using System;

namespace NavMesh2D.Core
{
    public class CollisionGeometrySetBuilder
    {
        public static CollisionGeometrySet Build(IEnumerable<Collider2D> collider, int circleVertCount)
        {
            CollisionGeometrySet result = new CollisionGeometrySet();
            List<Vector2> inOutVerts = new List<Vector2>(10); //Just a guess
            float anglePerCircleVert = (Mathf.PI * 2) / circleVertCount;

            foreach (Collider2D col in collider)
            {
                if (col == null)
                    continue;

                Type cTyp = col.GetType();

                //Sort out any edge collider, as they will be processed differently.
                if (cTyp == typeof(EdgeCollider2D))
                {
                    //LoadEdgeColliderVerts((EdgeCollider2D)collider[iCol], inOutVerts);
                    //result.AddEdge(inOutVerts);
                    //Ignore it for the time being!!
                }
                else
                {
                    if (cTyp == typeof(BoxCollider2D))
                    {
                        LoadBoxColliderVerts((BoxCollider2D)col, inOutVerts);
                        RoundVerts(inOutVerts);
                        result.AddCollider(inOutVerts);
                       
                        inOutVerts.Clear();
                    }
                    else if (cTyp == typeof(CircleCollider2D))
                    {
                        LoadCircleColliderVerts((CircleCollider2D)col, inOutVerts, circleVertCount, anglePerCircleVert);
                        RoundVerts(inOutVerts);
                        result.AddCollider(inOutVerts);
                        inOutVerts.Clear();
                    }
                    else
                    {
                        PolygonCollider2D polyCol = (PolygonCollider2D)col;
                        for (int iPath = 0; iPath < polyCol.pathCount; iPath++)
                        { 
                            LoadVerts(polyCol.GetPath(iPath), polyCol.transform.localToWorldMatrix, polyCol.offset, inOutVerts);
                            if (inOutVerts.Count == 0)
                            {
                                Debug.LogError("Polygonecollider has chain with zero verts.");
                            }
                            else
                            {
                                RoundVerts(inOutVerts);
                                result.AddCollider(inOutVerts);
                                inOutVerts.Clear();
                            }
                        }
                    }

                   
                }
               
            }
            return result;
        }

        private static void RoundVerts(List<Vector2> inOutVerts)
        {
            for (int iVert = 0; iVert < inOutVerts.Count; iVert++)
            {
                inOutVerts[iVert] =new Vector2(
                    (float)Math.Round(inOutVerts[iVert].x, 3),
                    (float)Math.Round(inOutVerts[iVert].y, 3)
                    );
            }
        }

        private static void LoadBoxColliderVerts(BoxCollider2D collider, List<Vector2> inOutVerts)
        {
            Vector2 halfSize = collider.size / 2;
            int startIndex = inOutVerts.Count;
            inOutVerts.Add(collider.transform.TransformPoint(halfSize + collider.offset));
            inOutVerts.Add(collider.transform.TransformPoint(new Vector2(-halfSize.x, halfSize.y) + collider.offset));
            inOutVerts.Add(collider.transform.TransformPoint(-halfSize + collider.offset));
            inOutVerts.Add(collider.transform.TransformPoint(new Vector2(halfSize.x, -halfSize.y) + collider.offset));
        }

        private static void LoadCircleColliderVerts(CircleCollider2D collider, List<Vector2> inOutVerts, int circleVertCount, float anglePerCircleVert)
        {
            for (int i = 0; i < circleVertCount; i++)
            {
                inOutVerts.Add(collider.transform.TransformPoint(new Vector2(collider.radius * Mathf.Cos(anglePerCircleVert * i) + collider.offset.x, collider.radius * Mathf.Sin(anglePerCircleVert * i) + collider.offset.y)));
            }
        }

        private static void LoadPolygonColliderVerts(PolygonCollider2D collider, List<Vector2> inOutVerts)
        {
            Matrix4x4 localToWorld = collider.transform.localToWorldMatrix;
            for (int iVert = 0; iVert < collider.points.Length; iVert++)
            {
                inOutVerts.Add(localToWorld.MultiplyPoint(collider.points[iVert] + collider.offset));
            }
        }

        private static void LoadVerts(Vector2[] verts, Matrix4x4 localToWorld, Vector2 offset, List<Vector2> inOutVerts)
        {
            for (int iVert = 0; iVert < verts.Length; iVert++)
            {
                inOutVerts.Add(localToWorld.MultiplyPoint(verts[iVert] + offset));
            }
        }

        private static void LoadEdgeColliderVerts(EdgeCollider2D collider, List<Vector2> inOutVerts)
        {
            Matrix4x4 localToWorld = collider.transform.localToWorldMatrix;
            for (int iVert = 0; iVert < collider.points.Length; iVert++)
            {
                inOutVerts.Add(localToWorld.MultiplyPoint(collider.points[iVert] + collider.offset));
            }
        }
    }
}
