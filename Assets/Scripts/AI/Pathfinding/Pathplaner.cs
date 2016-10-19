using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NavMesh2D;
using Priority_Queue;
using Pathfinding2D;
using System;

public class PathPlaner : MonoBehaviour
{

    public static PathPlaner Instance { get { return instance; } }
    private static PathPlaner instance;

    [SerializeField]
    NavigationData2D navData;

    void Awake()
    {
        Debug.Assert(instance == null);
        instance = this;
        openList = new StablePriorityQueue<PathNode>(100);
        closedList = new List<PathNode>(100);
        requestBuffer = new Stack<PathRequest>(5);
    }

    Stack<PathRequest> requestBuffer;

    public void RequestPath(PathRequest request)
    {
        requestBuffer.Push(request);
    }

    StablePriorityQueue<PathNode> openList;
    List<PathNode> closedList;

    public void FindRequestedPath(PathRequest request)
    {
        NavNode start_node;
        int start_vertIndex;
        Vector2 start_point;
        if (!navData.TryMapPoint(request.start, out start_point, out start_vertIndex, out start_node))
        {
            //No path can be found!
            Debug.Log("Couldn't map points. -> no path");
            request.callback(null);
            return;
        }

        NavNode goal_node;
        int goal_vertIndex;
        Vector2 goal_point;
        if (!navData.TryMapPoint(request.goal, out goal_point, out goal_vertIndex, out goal_node))
        {
            //No path can be found!
            Debug.Log("Couldn't map points. -> no path");
            request.callback(null);
            return;
        }

        //Trivial case: Start and Goal are on the same line
        if (start_node == goal_node && goal_vertIndex == start_vertIndex)
        {
            request.callback(new NavPath() { pathSegments = new IPathSegment[] { new PathSegment(start_point, goal_point, Vector2.Distance(start_point, goal_point) / navData.navAgentSettings.maxXVel) } });
            return;
        }
        closedList.Clear();
        openList.Clear();

        PathNode cNode = new PathNode(null, start_node, start_vertIndex, 0, Vector2.Distance(start_node.verts[start_vertIndex].PointB, goal_point));
        openList.Enqueue(cNode, cNode.totalCost);
        int newVertIndex;
        float costSoFar;

        while (openList.Count > 0)
        {
            cNode = openList.Dequeue();
            if (cNode.navNode == goal_node && cNode.navVertIndex == goal_vertIndex)
            {
                goto FoundPath;
            }

            closedList.Add(cNode);

            if (cNode.navVertIndex - 1 >= 0)
            {
                newVertIndex = cNode.navVertIndex - 1;
                if (!IsNodeClosed(cNode.navNode, newVertIndex))
                {
                    costSoFar = cNode.costSoFar + cNode.navNode.verts[newVertIndex].distanceBC;
                    PathNode newNode = new PathNode(cNode, cNode.navNode, newVertIndex, costSoFar, Vector2.Distance(goal_point, cNode.navNode.verts[newVertIndex].PointB));
                    openList.Enqueue(newNode, newNode.costSoFar);
                }
            }
            else if (cNode.navNode.isClosed)
            {
                newVertIndex = cNode.navNode.verts.Length - 1;
                if (!IsNodeClosed(cNode.navNode, newVertIndex))
                {
                    costSoFar = cNode.costSoFar + cNode.navNode.verts[newVertIndex].distanceBC;
                    PathNode newNode = new PathNode(cNode, cNode.navNode, newVertIndex, costSoFar, Vector2.Distance(goal_point, cNode.navNode.verts[newVertIndex].PointB));
                    openList.Enqueue(newNode, newNode.costSoFar);
                }
            }

            if (cNode.navVertIndex + 1 < cNode.navNode.verts.Length)
            {
                newVertIndex = cNode.navVertIndex + 1;
                if (!IsNodeClosed(cNode.navNode, newVertIndex))
                {
                    costSoFar = cNode.costSoFar + cNode.NVert.distanceBC;
                    PathNode newNode = new PathNode(cNode, cNode.navNode, newVertIndex, costSoFar, Vector2.Distance(goal_point, cNode.navNode.verts[newVertIndex].PointB));
                    openList.Enqueue(newNode, newNode.costSoFar);
                }
            }
            else if (cNode.navNode.isClosed)
            {
                newVertIndex = 0;
                if (!IsNodeClosed(cNode.navNode, newVertIndex))
                {
                    costSoFar = cNode.costSoFar + cNode.NVert.distanceBC;
                    PathNode newNode = new PathNode(cNode, cNode.navNode, newVertIndex, costSoFar, Vector2.Distance(goal_point, cNode.navNode.verts[newVertIndex].PointB));
                    openList.Enqueue(newNode, newNode.costSoFar);
                }
            }

            if (cNode.navNode.verts[cNode.navVertIndex].linkIndex.Length > 0)
            {
                int[] linkIndecies = cNode.navNode.verts[cNode.navVertIndex].linkIndex;
                for (int iLink = 0; iLink < linkIndecies.Length; iLink++)
                {
                    IOffNodeLink cLink = cNode.navNode.links[linkIndecies[iLink]];
                    if (!IsNodeClosed(navData.nodes[cLink.targetNodeIndex], cLink.targetVertIndex))
                    {
                        costSoFar = cNode.costSoFar + cLink.traversCosts;
                        PathNode newNode = new PathNode(cNode, navData.nodes[cLink.targetNodeIndex], cLink.targetVertIndex, costSoFar, Vector2.Distance(goal_point, cLink.endPoint), linkIndecies[iLink]);
                        openList.Enqueue(newNode, newNode.costSoFar);
                    }
                }
            }
        }

        request.callback(null);
        Debug.Log("No Path found");
        return;

        FoundPath:
        List<IPathSegment> pathSegments = new List<IPathSegment>(50);
        Vector2 prevPoint = goal_point;
        float distance = 0;
        while (cNode.parent != null)
        {
            /*if (cNode.linkIndex == -1)
            {
                distance += (cNode.NVert.PointB - prevPoint).magnitude;
            }
            else
            {*/

            pathSegments.Add(new PathSegment(cNode.Link.endPoint, prevPoint, (distance + (cNode.Link.endPoint - prevPoint).magnitude) / navData.navAgentSettings.maxXVel));
            distance = 0;
            pathSegments.Add(new JumpSegment((JumpLink)cNode.Link));
            prevPoint = cNode.Link.startPoint;
            // }
            cNode = cNode.parent;
        }
        pathSegments.Add(new PathSegment(start_point, prevPoint, distance + (start_point - prevPoint).magnitude));

        IPathSegment[] inversedSeg = new IPathSegment[pathSegments.Count];
        for (int iSeg = 0, iInv = pathSegments.Count - 1; iSeg < pathSegments.Count; iSeg++, iInv--)
        {
            inversedSeg[iSeg] = pathSegments[iInv];
        }
        request.callback(new NavPath() { pathSegments = inversedSeg });
    }

    bool IsNodeClosed(NavNode nn, int vertIndex)
    {
        for (int i = 0; i < closedList.Count; i++)
        {
            if (closedList[i].navNode == nn && closedList[i].navVertIndex == vertIndex)
                return true;
        }
        return false;
    }

    class PathNode : StablePriorityQueueNode
    {
        public int navVertIndex;
        public NavNode navNode;
        public float costSoFar;
        public float totalCost;
        public int linkIndex;
        public PathNode parent;

        public NavVert NVert { get { return navNode.verts[navVertIndex]; } }
        public IOffNodeLink Link { get { return parent.navNode.links[linkIndex]; } }


        public PathNode(PathNode parent, NavNode navNode, int navVertIndex, float costSoFar, float estimatedCost)
        {
            this.navNode = navNode;
            this.navVertIndex = navVertIndex;
            this.costSoFar = costSoFar;
            totalCost = estimatedCost = costSoFar;
            this.parent = parent;
            linkIndex = -1;
        }

        public PathNode(PathNode parent, NavNode navNode, int navVertIndex, float costSoFar, float estimatedCost, int linkIndex)
        {
            this.navNode = navNode;
            this.navVertIndex = navVertIndex;
            this.costSoFar = costSoFar;
            totalCost = estimatedCost = costSoFar;
            this.parent = parent;
            this.linkIndex = linkIndex;
        }
    }
}

public class PathRequest
{
    public delegate void PathCompleted(NavPath path);

    public readonly Vector2 start;
    public readonly Vector2 goal;
    public readonly PathCompleted callback;

    public PathRequest(Vector2 start, Vector2 goal, PathCompleted callback)
    {
        this.start = start;
        this.goal = goal;
        this.callback = callback;
    }
}

[Serializable]
public class NavPath
{
    public IPathSegment[] pathSegments;

    public void Visualize()
    {
        foreach (IPathSegment seg in pathSegments)
        {
            seg.Visualize();
        }
    }
}

