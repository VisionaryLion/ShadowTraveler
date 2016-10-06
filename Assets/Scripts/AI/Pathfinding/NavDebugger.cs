using UnityEngine;
using System.Collections;
using Pathfinding2D;
using Utility;
using System;
using System.Collections.Generic;
using NavMesh2D.Core;
using NavMesh2D;
using Utility.Polygon2D;

public class NavDebugger : MonoBehaviour
{
    public enum DebugWhichSet { CollisionGeometrySet, OutlineTree, ExpandedTree, NavigationData2D }

    public int circleVertCount;
    public DebugWhichSet debugConfig;

    [Range(0.0f, 10.0f)]
    public float minHeightTest = 0;
    [Range(0.001f, .5f)]
    public float nodeMergeDist;
    [Range(0.0f, 5f)]
    public float maxEdgeDeviation;

    public Transform closestTestPoint;

    CollisionGeometrySet cgs;
    ContourTree outlineTree;
    ExpandedTree[] exTrees;
    NavigationData2D navData2D;

    // Use this for initialization
    void Start()
    {

        System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();

        Collider2D[] allCollider = GameObject.FindObjectsOfType<Collider2D>();
        long totalEllapsedTime = 0;

        cgs = CollisionGeometrySetBuilder.Build(allCollider, circleVertCount);
        Debug.Log("CollisionGeometrySetBuilder finished in " + (watch.ElapsedMilliseconds / 1000f) + " sec.");

        totalEllapsedTime += watch.ElapsedMilliseconds;
        watch.Reset();
        watch.Start();

        outlineTree = ContourTree.Build(cgs, nodeMergeDist, maxEdgeDeviation);
        Debug.Log("OutlineTreeBuilder finished in " + (watch.ElapsedMilliseconds / 1000f) + " sec.");

        totalEllapsedTime += watch.ElapsedMilliseconds;
        watch.Reset();
        watch.Start();

        exTrees = ExpandedTreeSetBuilder.Build(outlineTree, new float[] { minHeightTest });
        Debug.Log("ExpandedTreeSetBuilder finished in " + (watch.ElapsedMilliseconds / 1000f) + " sec.");

        totalEllapsedTime += watch.ElapsedMilliseconds;
        watch.Reset();
        watch.Start();

        navData2D = NavigationData2DBuilder.Build(exTrees[0], 0);
        Debug.Log("NavigationData2DBuilder finished in " + (watch.ElapsedMilliseconds / 1000f) + " sec.");

        totalEllapsedTime += watch.ElapsedMilliseconds;
        watch.Stop();
        Debug.Log("Total build time is " + (totalEllapsedTime / 1000f) + " sec.");
    }

    int counter = -1;
    // Update is called once per frame
    void Update()
    {
        switch (debugConfig)
        {
            case DebugWhichSet.OutlineTree:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    timer = 0;
                    if (counter == -1)
                    {
                        outlineTree = new ContourTree();
                        counter = 0;
                    }
                    if (counter < cgs.colliderVerts.Count)
                    {
                        outlineTree.AddContour(cgs.colliderVerts[counter]);
                        counter++;
                    }
                }
                break;
        }

        switch (debugConfig)
        {
            case DebugWhichSet.CollisionGeometrySet:
                if (cgs != null)
                {
                    cgs.VisualDebug();
                }
                break;
            case DebugWhichSet.OutlineTree:
                if (outlineTree != null)
                {
                    outlineTree.VisualDebug();
                    if (counter > 0 && timer < 1)
                    {
                        Contour cC = new Contour(cgs.colliderVerts[counter - 1]);
                        cC.VisualDebug(Utility.DifferentColors.GetColor(0));
                        timer += Time.deltaTime;
                    }
                }
                break;
            case DebugWhichSet.ExpandedTree:
                if (exTrees != null)
                {
                    foreach (ExpandedTree eT in exTrees)
                    {
                        eT.headNode.VisualDebug(0);
                        Vector2 mappedPoint;
                        Vector2 normal;
                        if (eT.TryMapPointToContour(closestTestPoint.position, out mappedPoint, out normal))
                        {
                            Debug.DrawLine(closestTestPoint.position, mappedPoint, Color.green);
                            DebugExtension.DebugPoint(mappedPoint);
                        }
                    }
                    
                }
                break;
            case DebugWhichSet.NavigationData2D:
                if (navData2D != null)
                {
                    navData2D.DrawForDebug();
                    Vector2 mappedPoint;
                    if (navData2D.TryMapPoint(closestTestPoint.position, out mappedPoint))
                    {
                        Debug.DrawLine(closestTestPoint.position, mappedPoint, Color.green);
                        DebugExtension.DebugPoint(mappedPoint);
                    }
                }
                break;
        }
    }
    float timer = 0;
}
