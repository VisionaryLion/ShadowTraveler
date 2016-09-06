// Super Fast Soft Lighting. Copyright 2015 Howling Moon Software, LLP

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SFPolygon : MonoBehaviour, _SFCullable
{
    private Rect _bounds;

    private Transform _t;
    public Matrix4x4 _GetMatrix()
    {
        if (!_t) _t = this.transform;
        return _t.localToWorldMatrix;
    }

    private Rect _worldBounds;
    public Rect _GetWorldBounds() { return _worldBounds; }

    public void _CacheWorldBounds()
    {
        if (!_t) _t = this.transform;

        _worldBounds = SFRenderer._TransformRect(_t.localToWorldMatrix, _bounds);
    }

    public void _UpdateBounds()
    {
        float l, b, r, t;

        var v0 = _verts[0];
        l = r = v0.x;
        b = t = v0.y;

        for (var i = 1; i < _verts.Length; i++)
        {
            var v = _verts[i];
            l = Mathf.Min(v.x, l);
            r = Mathf.Max(v.x, r);
            b = Mathf.Min(v.y, b);
            t = Mathf.Max(v.y, t);
        }

        _bounds = Rect.MinMaxRect(l, b, r, t);
    }

    [SerializeField]
    private Vector2[] _verts = new Vector2[3];

    public bool _looped;
    public int _shadowLayers = 0;
    public float _lightPenetration = 0.0f;
    public float _opacity = 1.0f;

    public Vector2[] verts
    {
        get
        {
            return _verts;
        }

        set
        {
            _verts = value;
            _UpdateBounds();
        }
    }

    public bool looped { get { return _looped; } set { _looped = value; } }
    [Range(3, 100)]
    public int circleVertCount = 10;

    public int shadowLayers { get { return _shadowLayers; } }
    public float lightPenetration { get { return _lightPenetration; } set { _lightPenetration = value; } }
    public float opacity { get { return _opacity; } set { _opacity = value; } }

    public static List<SFPolygon> _polygons = new List<SFPolygon>();
    private void OnEnable() { _polygons.Add(this); }
    private void OnDisable() { _polygons.Remove(this); }

    private void Start()
    {
        _UpdateBounds();
    }

    public void _TryCopyVerts()
    {
        if (!_CopyVertsFromCollider())
            if (!_CopyVertsFromSprite())
                _GenerateDefaultVerts();
    }

    public bool _CopyVertsFromCollider()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col)
        {
            List<Vector2> inOutVerts = new List<Vector2>(10); //Just a guess
            Type cTyp = col.GetType();

            //Sort out any edge collider, as they will be processed differently.
            if (cTyp == typeof(EdgeCollider2D))
                LoadEdgeColliderVerts((EdgeCollider2D)col, inOutVerts);
            else if (cTyp == typeof(BoxCollider2D))
                LoadBoxColliderVerts((BoxCollider2D)col, inOutVerts);
            else if (cTyp == typeof(CircleCollider2D))
                LoadCircleColliderVerts((CircleCollider2D)col, inOutVerts);
            else
                LoadPolygonColliderVerts((PolygonCollider2D)col, inOutVerts);

            verts = inOutVerts.ToArray();
            this.looped = true;
            inOutVerts.Clear();
            return true;
        }
        return false;
    }

    public bool _CopyVertsFromSprite()
    {
        SpriteRenderer sprRen = GetComponent<SpriteRenderer>();
        if (sprRen)
        {
            this.looped = true;
            verts = new Vector2[] {
                transform.worldToLocalMatrix.MultiplyPoint( sprRen.bounds.min),
                transform.worldToLocalMatrix.MultiplyPoint(new Vector2(sprRen.bounds.min.x, sprRen.bounds.max.y)),
                transform.worldToLocalMatrix.MultiplyPoint(sprRen.bounds.max),
                 transform.worldToLocalMatrix.MultiplyPoint(new Vector2(sprRen.bounds.max.x, sprRen.bounds.min.y)),

                };
            return true;
        }
        return false;
    }

    public void _GenerateDefaultVerts()
    {
        this.looped = true;
        this.verts = new Vector2[] {
                new Vector2(1,1),
                new Vector2(1,-1),
                new Vector2(-1,-1),
                new Vector2(-1,1)
            };
    }

    private void LoadBoxColliderVerts(BoxCollider2D collider, List<Vector2> inOutVerts)
    {
        Vector2 halfSize = collider.size / 2;
        inOutVerts.Add(halfSize + collider.offset);
        inOutVerts.Add(new Vector2(halfSize.x, -halfSize.y) + collider.offset);
        inOutVerts.Add(-halfSize + collider.offset);
        inOutVerts.Add(new Vector2(-halfSize.x, halfSize.y) + collider.offset);
    }

    private void LoadCircleColliderVerts(CircleCollider2D collider, List<Vector2> inOutVerts)
    {
        float anglePerCircleVert = (Mathf.PI * 2) / circleVertCount;
        for (int i = 0; i < circleVertCount; i++)
        {
            inOutVerts.Add(new Vector2(collider.radius * Mathf.Sin(anglePerCircleVert * i), collider.radius * Mathf.Cos(anglePerCircleVert * i)) + collider.offset);
        }
    }

    private void LoadPolygonColliderVerts(PolygonCollider2D collider, List<Vector2> inOutVerts)
    {
        for (int iVert = 0; iVert < collider.points.Length; iVert++)
        {
            inOutVerts.Add(collider.points[iVert] + collider.offset);
        }
    }

    private void LoadEdgeColliderVerts(EdgeCollider2D collider, List<Vector2> inOutVerts)
    {
        for (int iVert = 0; iVert < collider.points.Length; iVert++)
        {
            inOutVerts.Add(collider.points[iVert] + collider.offset);
        }
    }

    public void _FlipInsideOut()
    {
        System.Array.Reverse(verts);
    }
}
