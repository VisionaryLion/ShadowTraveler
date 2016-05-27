using UnityEngine;
using System.Collections;
using Pathfinding2D;
using Utility;
using Polygon2D;
using System;
using System.Collections.Generic;

public class NavDebugger : MonoBehaviour {
    public int circleVertCount;
    //public LayerMask colliderMask;
    //public float slopeLimit;
    //NavMesh2D navMesh2D;

    public GameObject gA;
    public GameObject gB;
    Vector2[] vertBuffer;
	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
       
    }

    void OnDrawGizmos()
    {
        if (Event.current.isMouse && Event.current.button == 0)
        {
            List<Vector2> verts = new List<Vector2>(10);
            LoadColliderVerts(gA.GetComponent<Collider2D>(), verts);
            Polygon polyA = new Polygon(verts.ToArray());
            verts.Clear();
            LoadColliderVerts(gB.GetComponent<Collider2D>(), verts);
            Polygon polyB = new Polygon(verts.ToArray());
            Polygon2DBooleanFunc pFunc = new Polygon2DBooleanFunc();
            vertBuffer = pFunc.Union(polyA, polyB);
            Debug.Log(vertBuffer.Length);
        }
        for (int iVert = 0; iVert < vertBuffer.Length; iVert += 2)
        {
            Gizmos.color = DifferentColors.GetColor(iVert);
            DrawLine(vertBuffer[iVert], vertBuffer[iVert + 1], 2);
            Gizmos.DrawWireSphere(vertBuffer[iVert], 0.5f);
            Gizmos.DrawWireSphere(vertBuffer[iVert+1], 0.1f);
            Debug.Log(vertBuffer[iVert] +" -> "+ vertBuffer[iVert + 1]);
        }
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
                Vector3 o = n * (0.1f/width)*i;
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
