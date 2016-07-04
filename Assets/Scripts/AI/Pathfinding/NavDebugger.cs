using UnityEngine;
using System.Collections;
using Pathfinding2D;
using Utility;
using Polygon2D;
using System;
using System.Collections.Generic;
using NavMesh2D.Core;

public class NavDebugger : MonoBehaviour
{
    public enum DebugWhichSet { CollisionGeometrySet, OutlineTree }

    public int circleVertCount;
    public LayerMask collisionMask;
    public DebugWhichSet debugConfig;
    public bool debugText = true;

    CollisionGeometrySet cgs;
    OutlineTree outlineTree;
    // Use this for initialization
    void Start()
    {
        System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();

        CollisionGeometrySetBuilder cgBuilder = new CollisionGeometrySetBuilder(circleVertCount, collisionMask);
        Collider2D[] allCollider = GameObject.FindObjectsOfType<Collider2D>();
        cgs = cgBuilder.Build(allCollider);
        Debug.Log("CollisionGeometrySetBuilder finished in " + (watch.ElapsedMilliseconds / 1000f) + " sec.");

        watch.Reset();
        watch.Start();

        OutlineTreeBuilder otBuilder = new OutlineTreeBuilder();
        outlineTree = otBuilder.Build(cgs);
        Debug.Log("OutlineTreeBuilder finished in " + (watch.ElapsedMilliseconds / 1000f) + " sec.");
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDrawGizmos()
    {
        switch (debugConfig)
        {
            case DebugWhichSet.CollisionGeometrySet:
                if (cgs != null)
                {
                    cgs.DrawDebugInfo();
                }
                break;
            case DebugWhichSet.OutlineTree:
                if (outlineTree != null)
                    outlineTree.DrawDebugInfo();
                break;
        }

    }
}
