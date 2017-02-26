using UnityEngine;
using System.Collections;
using System;
using NavMesh2D.Core;

[Serializable]
public class JumpArcSegment
{
    [SerializeField]
    public float j, halfG, v, doubleG;
    [SerializeField]
    public float minX, maxX;
    [SerializeField]
    public float startX, endX;
    [SerializeField]
    public float minY, maxY;

    public JumpArcSegment(float j, float g, float v, float startX, float endX)
    {
        this.j = j;
        this.halfG = g / 2;
        this.v = v;
        this.startX = startX;
        this.endX = endX;
        if (startX < endX)
        {
            minX = startX;
            maxX = endX;
        }
        else
        {
            minX = endX;
            maxX = startX;
        }
        minY = Mathf.Min(Calc(0), Calc(maxX - minX));
        doubleG = g * 2;
        maxY = (j * j) / (4 * doubleG);
    }

    public void UpdateArc(float j, float g, float v, float startX, float endX)
    {
        this.j = j;
        this.halfG = g / 2;
        this.v = v;
        this.startX = startX;
        this.endX = endX;
        if (startX < endX)
        {
            minX = startX;
            maxX = endX;
        }
        else
        {
            minX = endX;
            maxX = startX;
        }
        minY = Mathf.Min(Calc(0), Calc(maxX - minX));
        doubleG = g * 2;
        maxY = (j * j) / (4 * doubleG);
    }

    public float Calc(float x)
    {
        x /= v;
        return (j - halfG * x) * x;
    }

    public bool IntersectsWithSegment(NavigationData2DBuilder.Segment seg, Vector2 arcOrigin)
    {
        /*
        if (seg.MinX > maxX || seg.MaxX < minX || seg.MinY > maxY || seg.MaxY < minY)
            return false;

        float b = j - (seg.m / v);
        float det = (b * b) - (2 * doubleG * seg.n);
        if (det < 0)
            return false;

        b = -b / doubleG;
        det = Mathf.Sqrt(det) / doubleG;
        float x1 = b + det;
        float x2 = b - det;

        if (seg.IsPointOnSegment(x1))
        {
            if (x1 >= minX && x1 <= maxX)
                return true;
        }
        else if (seg.IsPointOnSegment(x2))
        {
            if (x2 >= minX && x2 <= maxX)
                return true;
        }*/
        return false;
    }

    public void VisualDebug(Vector2 origin, Color color)
    {
        Vector2 swapPos;
        Vector2 prevPos = new Vector2(minX, Calc(minX)) + origin;
        for (float x = minX; x + 0.1f < maxX; x += 0.1f)
        {
            swapPos = new Vector2(x, Calc(x)) + origin;
            Debug.DrawLine(prevPos, swapPos, color);
            prevPos = swapPos;
        }
        Debug.DrawLine(prevPos, new Vector2(maxX, Calc(maxX)) + origin, color);

    }
}
