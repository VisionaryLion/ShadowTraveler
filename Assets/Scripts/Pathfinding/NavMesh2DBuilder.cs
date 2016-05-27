using UnityEngine;
using System.Collections.Generic;
using System;
using Polygon2D;

namespace Pathfinding2D
{/*
    public class NavMesh2DBuilder
    {
        public volatile float progress;
        int _circleVertCount;
        int _nextNodeId;



        public NavMesh2D Build(int colliderLayer, float slopeLimit, int circleVertCount)
        {
            _circleVertCount = circleVertCount;
            
            List<Vector2> colliderVerts = new List<Vector2>(20);

            //Find all collider to process
            List<Collider2D> allCollider = LoadCollider(colliderLayer);
            Debug.Log("NavmeshBuilder.Build ["+progress+"%]: Found "+allCollider.Count+" collider to process.");

            //Init the navMesh
            NavMesh2D navMesh2D = ScriptableObject.CreateInstance<NavMesh2D>();
            navMesh2D.dynamicNodes = new List<DynamicPathNode>((int)Mathf.Min(5, allCollider.Count / 100));
            navMesh2D.staticNodes = new List<PathNode>(allCollider.Count);

            float progressStep = 200 / allCollider.Count;
            Polygon2DBooleanFunc polyUnion = new Polygon2DBooleanFunc();
            //Process each collider
            foreach (Collider2D c in allCollider)
            {
                colliderVerts.Clear();
                //First load the collider verts into the colliderVerts array
                LoadColliderVerts(c, colliderVerts);
                Debug.Log("NavmeshBuilder.Build [" + progress + "%]: Loaded " + colliderVerts.Count + " verts to process.");
                if (colliderVerts.Count <= 1)
                    continue;
                HandleCollider(c, colliderVerts, slopeLimit, navMesh2D);
                Debug.Log("NavmeshBuilder.Build [" + progress + "%]: Transformed " + colliderVerts.Count + " to nodes.");

            }
            return navMesh2D;
        }

        private void HandleCollider(Collider2D collider, List<Vector2> colliderVerts, float slopeLimit, NavMesh2D inOutMesh)
        {
            //Add the first vert at the end
            colliderVerts.Add(colliderVerts[0]);
            float xMin = colliderVerts[0].x;
            float xMax = colliderVerts[0].x;
            float angle;
            bool wasLastEdgeWalkable = false;
            int notWalkableEdges = 0;

            for(int i = 0; i < colliderVerts)

            for (int i = 1; i < colliderVerts.Count; i++)
            {
                angle = Vector2.Angle(Vector2.left, colliderVerts[i] - colliderVerts[i - 1]);
                Debug.Log("Angle = "+angle+", "+ (colliderVerts[i] - colliderVerts[i - 1]));
                if (angle > slopeLimit && angle < 180 - slopeLimit)
                {
                    if (lastWalkableSegment != -1)
                    {
                        Vector2[] vertsOfNode = new Vector2[(i - lastWalkableSegment) + 1];
                        colliderVerts.CopyTo(lastWalkableSegment, vertsOfNode, 0, vertsOfNode.Length);
                        if (collider.GetComponent<Rigidbody2D>() != null)
                            inOutMesh.dynamicNodes.Add(new DynamicPathNode(NextNodeId, xMin, xMax, vertsOfNode, collider));
                        else
                            inOutMesh.staticNodes.Add(new PathNode(NextNodeId, xMin, xMax, vertsOfNode));
                    }
                    xMin = colliderVerts[i].x;
                    xMax = colliderVerts[i].x;
                    lastWalkableSegment = -1;
                }
                else
                {
                    xMin = Mathf.Max(colliderVerts[i].x);
                    xMax = Mathf.Min(colliderVerts[i].x);
                    if (lastWalkableSegment == -1)
                        lastWalkableSegment = i - 1;
                }
            }
            if (lastWalkableSegment != -1)
            {
                Vector2[] vertsOfNode = new Vector2[colliderVerts.Count - lastWalkableSegment];
                colliderVerts.CopyTo(lastWalkableSegment, vertsOfNode, 0, vertsOfNode.Length);
                if (collider.GetComponent<Rigidbody2D>() != null)
                    inOutMesh.dynamicNodes.Add(new DynamicPathNode(NextNodeId, xMin, xMax, vertsOfNode, collider));
                else
                    inOutMesh.staticNodes.Add(new PathNode(NextNodeId, xMin, xMax, vertsOfNode));
            }
        }

        private int NextNodeId
        {
            get { return _nextNodeId++; }
        }

        private List<Collider2D> LoadCollider(int colliderLayer)
        {
            Collider2D[] allCollider = GameObject.FindObjectsOfType<Collider2D>();
            List<Collider2D> selectedCollider = new List<Collider2D>(allCollider.Length);
            foreach (Collider2D c in allCollider)
            {
                if (c.isTrigger)
                    continue;
                if (((1 << c.gameObject.layer) | colliderLayer) == colliderLayer)
                    selectedCollider.Add(c);
            }
            return selectedCollider;
        }

        private void LoadColliderVerts(Collider2D collider, List<Vector2> verts)
        {
            verts.Clear();
            Type cTyp = collider.GetType();

            if (cTyp == typeof(BoxCollider2D))
                GetBoxColliderVerts((BoxCollider2D)collider, verts);
            else if (cTyp == typeof(CircleCollider2D))
                GetCircleColliderVerts((CircleCollider2D)collider, verts, _circleVertCount);
            else if (cTyp == typeof(EdgeCollider2D))
                verts.AddRange(((EdgeCollider2D)collider).points);
            else if (cTyp == typeof(PolygonCollider2D))
                verts.AddRange(((PolygonCollider2D)collider).points);
        }

        private static void GetBoxColliderVerts(BoxCollider2D collider, List<Vector2> verts)
        {
            Vector2 halfSize = collider.size / 2;
            verts.Add(collider.transform.TransformPoint(halfSize + collider.offset));
            verts.Add(collider.transform.TransformPoint(new Vector2(halfSize.x, -halfSize.y) + collider.offset));
            verts.Add(collider.transform.TransformPoint(-halfSize + collider.offset));
            verts.Add(collider.transform.TransformPoint(new Vector2(-halfSize.x, halfSize.y) + collider.offset));
        }

        private static void GetCircleColliderVerts(CircleCollider2D collider, List<Vector2> verts, int circleVertCount)
        {
            float anglePerCircleVert = (Mathf.PI * 2) / circleVertCount;
            for (int i = 0; i < circleVertCount; i++)
            {
                verts.Add(collider.transform.TransformPoint(new Vector2(collider.radius * Mathf.Sin(anglePerCircleVert * i), collider.radius * Mathf.Sin(anglePerCircleVert * i))));
            }
        }
    }*/
}
