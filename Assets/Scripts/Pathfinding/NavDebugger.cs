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
    //public LayerMask colliderMask;
    //public float slopeLimit;
    //NavMesh2D navMesh2D;

    public GameObject gA;
    public GameObject gB;
    public PolygonClipper.BoolOpType op;
    Polygon result;
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
            List<Vector2> verts = new List<Vector2>(10);
            LoadColliderVerts(gA.GetComponent<Collider2D>(), verts);
            Polygon polyA = new Polygon();
            polyA.AddContour(new Contour(verts));
            verts.Clear();
            LoadColliderVerts(gB.GetComponent<Collider2D>(), verts);
            Polygon polyB = new Polygon();
            polyB.AddContour(new Contour(verts));
            result = PolygonClipper.Compute(ref polyA, ref polyB, op);
            old_op = op;

            //DEBUG
            Debug.Log("Contour count = " + result.ContourCount);
            for (int iCount = 0; iCount < result.ContourCount; iCount++)
            {
                Debug.Log("     Contour[" + iCount + "] vertex count = " + result[iCount].VertexCount);
                for (int iVert = 0; iVert < result[iCount].VertexCount; iVert ++)
                {
                    Debug.Log("          Contour[" + iCount + "]["+iVert+"] vertex = " + result[iCount][iVert]);
                }
            }
        }

        for (int iCount = 0; iCount < result.ContourCount; iCount++)
        {
            Gizmos.color = DifferentColors.GetColor(iCount);
            for (int iVert = 0; iVert < result[iCount].VertexCount - 1; iVert ++)
            {
                DrawLine(result[iCount][iVert], result[iCount][iVert + 1], 2);
                Gizmos.DrawWireSphere(result[iCount][iVert], 0.5f);
                Gizmos.DrawWireSphere(result[iCount][iVert + 1], 0.1f);
            }
            DrawLine(result[iCount][0], result[iCount][result[iCount].VertexCount - 1], 2);
            Gizmos.DrawWireSphere(result[iCount][0], 0.5f);
            Gizmos.DrawWireSphere(result[iCount][result[iCount].VertexCount - 1], 0.1f);
        }
        /*
        Gizmos.color = DifferentColors.GetColor(100);
        List<Vector2> verx = new List<Vector2>(10);
        LoadColliderVerts(gA.GetComponent<Collider2D>(), verx);
        for (int iVert = 0; iVert < verx.Count -1; iVert++)
        {
            DrawLine(verx[iVert], verx[iVert + 1], 2);
        }
        Gizmos.color = DifferentColors.GetColor(101);
        
        LoadColliderVerts(gB.GetComponent<Collider2D>(), verx);
        for (int iVert = 0; iVert < verx.Count - 1; iVert++)
        {
            DrawLine(verx[iVert], verx[iVert + 1], 2);
        }*/
    }

    public static void DrawLine(Vector3 p1, Vector3 p2, float width)
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
            Vector3 v1 = (p2 - p1).normalized; // line direction
            Vector3 v2 = (c.transform.position - p1).normalized; // direction to camera
            Vector3 n = Vector3.Cross(v1, v2); // normal vector
            for (int i = 0; i < count; i++)
            {
                Vector3 o = n * (0.1f / width) * i;
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
            verts.Add(collider.transform.TransformPoint(new Vector2(collider.radius * Mathf.Sin(anglePerCircleVert * i), collider.radius * Mathf.Sin(anglePerCircleVert * i))));
        }
    }
}
