using UnityEngine;
using System.Collections;
using Pathfinding2D;
using Utility;
using Polygon2D;
using System;
using System.Collections.Generic;

public class NavDebugger : MonoBehaviour
{
    public int circleVertCount;

    public GameObject gA;
    public GameObject gB;
    public PolygonClipper.BoolOpType op;
    public LayerMask collisionMask;
    PointChain[] result;
    PolygonClipper.BoolOpType old_op;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDrawGizmos()
    {
        if ((Event.current.isMouse && Event.current.button == 0) || result == null || old_op != op)
        {
            NavMesh2DBuilder builder = new NavMesh2DBuilder(collisionMask, circleVertCount);
            LinkedList<PointChain> tL = builder.Build();
            result = new PointChain[tL.Count];
            tL.CopyTo(result, 0);

            //DEBUG
            Debug.Log("Contour count = " + result.Length);
            for (int iCount = 0; iCount < result.Length; iCount++)
            {
                Debug.Log("     Contour[" + iCount + "] vertex count = " + result[iCount].chain.Count);
                foreach (Vector2 vert in result[iCount].chain)
                {
                    Debug.Log("          Contour[" + iCount + "] vertex = " + vert);
                }
            }
        }

        for (int iCount = 0; iCount < result.Length; iCount++)
        {
            Gizmos.color = DifferentColors.GetColor(iCount);
            LinkedListNode<Vector2> cNode = result[iCount].chain.First;
            while ((cNode = cNode.Next) != null)
            {
                DrawLine(cNode.Value, cNode.Previous.Value, 2);
                Gizmos.DrawWireSphere(cNode.Value, 0.5f);
                Gizmos.DrawWireSphere(cNode.Previous.Value, 0.1f);
            }
            DrawLine(result[iCount].chain.Last.Value, result[iCount].chain.First.Value, 2);
            Gizmos.DrawWireSphere(result[iCount].chain.Last.Value, 0.5f);
            Gizmos.DrawWireSphere(result[iCount].chain.First.Value, 0.1f);
        }
    }

    public static void DrawLine(Vector2 p1, Vector2 p2, float width)
    {
        int count = Mathf.CeilToInt(width); // how many lines are needed.
        if (count == 1)
            Gizmos.DrawLine(p1, p2);
        else
        {
            Camera c = Camera.current;
            if (c == null)
            {
                Debug.LogError("Camera.current is null");
                return;
            }
            Vector2 v1 = (p2 - p1).normalized; // line direction
            Vector2 v2 = ((Vector2)c.transform.position - p1).normalized; // direction to camera
            Vector2 n = Vector3.Cross(v1, v2); // normal vector
            for (int i = 0; i < count; i++)
            {
                Vector2 o = n * (0.1f / width) * i;
                Gizmos.DrawLine(p1 + o, p2 + o);
            }
        }
    }

    private void LoadColliderVerts(Collider2D collider, List<Vector2> verts)
    {
        verts.Clear();
        Type cTyp = collider.GetType();

        if (cTyp == typeof(BoxCollider2D))
            GetBoxColliderVerts((BoxCollider2D)collider, verts);
        else if (cTyp == typeof(CircleCollider2D))
            GetCircleColliderVerts((CircleCollider2D)collider, verts, circleVertCount);
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
            verts.Add(collider.transform.TransformPoint(new Vector2(collider.radius * Mathf.Sin(anglePerCircleVert * i), collider.radius * Mathf.Cos(anglePerCircleVert * i))));
        }
    }
}
