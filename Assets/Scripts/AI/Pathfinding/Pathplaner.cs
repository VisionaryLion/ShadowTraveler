using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NavMesh2D;
using Priority_Queue;
using Pathfinding2D;
using System;
using LightSensing;

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
        openList = new StablePriorityQueue<IPathNode>(100);
        closedNodes = new List<IPathNode>(100);
        closedLinks = new List<IOffNodeLink>(100);
        requestBuffer = new Stack<PathRequest>(5);
    }

    Stack<PathRequest> requestBuffer;

    public void RequestPath(PathRequest request)
    {
        requestBuffer.Push(request);
    }

    StablePriorityQueue<IPathNode> openList;
    List<IPathNode> closedNodes;
    List<IOffNodeLink> closedLinks;

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

        //trivial case: Start and Goal are on the same line
        float mult;
        if (start_node == goal_node && goal_vertIndex == start_vertIndex && IsLineTraversable(request, start_point, goal_point, out mult))
        {
            request.callback(new NavPath(new IPathSegment[] { new PathSegment(start_point, goal_point, Vector2.Distance(start_point, goal_point) / navData.navAgentSettings.maxXVel) }, goal_point));
            return;
        }
        closedNodes.Clear();
        closedLinks.Clear();
        openList.Clear();

        IPathNode cNode = new PathNode_Start(start_node, start_vertIndex, Vector2.Distance(start_node.verts[start_vertIndex].PointB, goal_point), start_point);
        openList.Enqueue(cNode, cNode.totalCost);
        float costSoFar;
        float costMultiplier = 1;
        Vector2 previousPoint = start_point;

        while (openList.Count > 0)
        {
            cNode = openList.Dequeue();
            if (cNode.navNode == goal_node && cNode.navVertIndex == goal_vertIndex && IsLineTraversable(request, cNode.StartPoint, goal_point, out costMultiplier))
            {
                goto FoundPath;
            }

            closedNodes.Add(cNode);

            int neighbourLeft, neighbourRight;
            cNode.GetNeighbours(out neighbourLeft, out neighbourRight);


            if (neighbourLeft != -1 && cNode.navNode.verts[neighbourLeft].distanceBC != 0)
            {
                if (!IsNodeClosed(cNode.navNode, neighbourLeft) && IsLineTraversable(request, cNode.StartPoint, cNode.NavVert.PointB, out costMultiplier))
                {
                    costSoFar = cNode.costSoFar + cNode.navNode.verts[neighbourLeft].distanceBC;
                    IPathNode newNode = new PathNode_Edge(cNode, cNode.navNode, neighbourLeft, false, costSoFar, Vector2.Distance(goal_point, cNode.navNode.verts[neighbourLeft].PointB));
                    openList.Enqueue(newNode, newNode.costSoFar);
                }
            }

            if (neighbourRight != -1 && cNode.navNode.verts[neighbourLeft].distanceBC != 0)
            {
                if (!IsNodeClosed(cNode.navNode, neighbourRight) && IsLineTraversable(request, cNode.StartPoint, cNode.navNode.verts[neighbourRight].PointB, out costMultiplier))
                {
                    costSoFar = cNode.costSoFar + cNode.NavVert.distanceBC * costMultiplier;
                    IPathNode newNode = new PathNode_Edge(cNode, cNode.navNode, neighbourRight, true, costSoFar, Vector2.Distance(goal_point, cNode.navNode.verts[neighbourRight].PointB));
                    openList.Enqueue(newNode, newNode.costSoFar);
                }
            }


            if (cNode.NavVert.linkIndex.Length > 0)
            {
                int[] linkIndecies = cNode.NavVert.linkIndex;
                for (int iLink = 0; iLink < linkIndecies.Length; iLink++)
                {
                    IOffNodeLink cLink = cNode.navNode.links[linkIndecies[iLink]];
                    if (!IsLinkClosed(cLink) && IsLineTraversable(request, cNode.StartPoint, cLink.startPoint, out costMultiplier))
                    {
                        costSoFar = cNode.costSoFar + cLink.traversCosts;
                        IPathNode newNode = new PathNode_Link(cNode, navData.nodes[cLink.targetNodeIndex], cLink.targetVertIndex, costSoFar, Vector2.Distance(goal_point, cLink.endPoint), linkIndecies[iLink]);
                        openList.Enqueue(newNode, newNode.costSoFar);
                        closedLinks.Add(cLink);
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

        while (cNode.parent != null)
        {
            if (cNode.GetType() == typeof(PathNode_Edge))
            {
                pathSegments.Add(new PathSegment(cNode.EndPoint, prevPoint, float.MaxValue));
                prevPoint = cNode.EndPoint;
            }
            else//link
            {
                var linkNode = (PathNode_Link)cNode;
                pathSegments.Add(new PathSegment(linkNode.Link.endPoint, prevPoint, float.MaxValue));
                prevPoint = cNode.EndPoint;
                pathSegments.Add(new JumpSegment((JumpLink)linkNode.Link));
                prevPoint = linkNode.Link.startPoint;
            }
            cNode = cNode.parent;
        }
        pathSegments.Add(new PathSegment(start_point, prevPoint, float.MaxValue));

        IPathSegment[] inversedSeg = new IPathSegment[pathSegments.Count];
        for (int iSeg = 0, iInv = pathSegments.Count - 1; iSeg < pathSegments.Count; iSeg++, iInv--)
        {
            inversedSeg[iSeg] = pathSegments[iInv];
        }
        request.callback(new NavPath(inversedSeg, request.goal));
    }

    bool IsLineTraversable(PathRequest requestData, Vector2 pointA, Vector2 pointB, out float costMultiplier)
    {
        costMultiplier = 1;
        foreach (var marker in requestData.lightMarker)
        {
            float result;
            Vector2 dir = ((pointB - pointA).magnitude / 2) * (pointB - pointA).normalized;
            if (!marker.IsTraversable(requestData.skin, pointA, pointB, out result))
            {
                Vector2 upLift = Vector2.up * UnityEngine.Random.Range(0.1f, 0.4f);
                Vector3 forwardLift = Vector3.forward * UnityEngine.Random.Range(0f, 10f);
                DebugExtension.DebugArrow((Vector3)(pointA + upLift) + forwardLift, dir, Color.red);
                DebugExtension.DebugArrow((Vector3)(pointB + upLift) + forwardLift, -dir, Color.red);
                Debug.DrawLine((Vector3)marker.Center + forwardLift, (Vector3)(pointB + upLift) + forwardLift, Color.red);
                Debug.DrawLine((Vector3)marker.Center + forwardLift, (Vector3)(pointA + upLift) + forwardLift, Color.red);
                Debug.Log("Path found to be not traversable!");
                return false;
            }
            if (result != 1)
            {
                DebugExtension.DebugArrow(marker.Center, dir, Color.green);
                costMultiplier += result;
            }
        }
        if(costMultiplier > 1)
            Debug.Log("Additional transition costs applied: "+costMultiplier);
        return true;
    }

    bool IsNodeClosed(NavNode nn, int vertIndex)
    {
        for (int i = 0; i < closedNodes.Count; i++)
        {
            if (closedNodes[i].navNode == nn && closedNodes[i].navVertIndex == vertIndex)
                return true;
        }
        return false;
    }

    bool IsLinkClosed(IOffNodeLink link)
    {
        return closedLinks.Contains(link);
    }

    abstract class IPathNode : StablePriorityQueueNode
    {
        public IPathNode parent;

        public NavNode navNode;
        public int navVertIndex;

        public float costSoFar;
        public float totalCost;

        public abstract void GetNeighbours(out int neighbourLeft, out int neighbourRight);
        public abstract Vector2 StartPoint { get; }
        public abstract Vector2 EndPoint { get; }
        public NavNode NavNode { get { return navNode; } }
        public NavVert NavVert { get { return navNode.verts[navVertIndex]; } }

        public IPathNode(IPathNode parent, NavNode navNode, int navVertIndex, float costSoFar, float estimatedCost)
        {
            this.navNode = navNode;
            this.navVertIndex = navVertIndex;
            this.costSoFar = costSoFar;
            totalCost = estimatedCost = costSoFar;
            this.parent = parent;
        }
    }

    class PathNode_Edge : IPathNode
    {
        public bool vertIndexIsIncrementing;

        public PathNode_Edge(IPathNode parent, NavNode navNode, int navVertIndex, bool vertIndexIsIncrementing, float costSoFar, float estimatedCost) : base(parent, navNode, navVertIndex, costSoFar, estimatedCost)
        {
            this.vertIndexIsIncrementing = vertIndexIsIncrementing;
        }

        public override Vector2 EndPoint
        {
            get
            {
                if (vertIndexIsIncrementing)
                {
                    if (navVertIndex + 1 >= navNode.verts.Length)
                    {
                        Debug.Assert(navNode.isClosed);
                        return navNode.verts[0].PointB;
                    }
                    else
                    {
                        return navNode.verts[navVertIndex + 1].PointB;
                    }
                }
                else
                {
                    return NavVert.PointB;
                }
            }
        }

        public override Vector2 StartPoint
        {
            get
            {
                if (vertIndexIsIncrementing)
                {
                    return NavVert.PointB;
                }
                else
                {
                    if (navVertIndex - 1 < 0)
                    {
                        return navNode.verts[navNode.verts.Length - 1].PointB;
                    }
                    else
                    {
                        return navNode.verts[navVertIndex - 1].PointB;
                    }
                }
            }
        }

        public override void GetNeighbours(out int neighbourLeft, out int neighbourRight)
        {
            neighbourLeft = -1;
            neighbourRight = -1;
            if (vertIndexIsIncrementing)
            {
                if (navVertIndex + 1 >= navNode.verts.Length)
                {
                    if (navNode.isClosed)
                    {
                        neighbourRight = 0;
                    }
                }
                else
                {
                    neighbourRight = navVertIndex + 1;
                }
            }
            else
            {
                if (navVertIndex - 1 < 0)
                {
                    if (navNode.isClosed)
                    {
                        neighbourLeft = navNode.verts.Length - 1;
                    }
                }
                else
                {
                    neighbourLeft = navVertIndex - 1;
                }
            }
        }
    }

    class PathNode_Start : IPathNode
    {
        Vector2 start_point;

        public PathNode_Start(NavNode navNode, int navVertIndex, float estimatedCost, Vector2 startPoint) : base(null, navNode, navVertIndex, 0, estimatedCost)
        {
            this.start_point = startPoint;
        }

        public override Vector2 EndPoint
        {
            get
            {
                return start_point;
            }
        }

        public override Vector2 StartPoint
        {
            get
            {
                return start_point;
            }
        }

        public override void GetNeighbours(out int neighbourLeft, out int neighbourRight)
        {
            if (navVertIndex + 1 >= navNode.verts.Length)
            {
                if (navNode.isClosed)
                {
                    neighbourRight = 0;
                }
                else
                    neighbourRight = -1;
            }
            else
            {
                neighbourRight = navVertIndex + 1;
            }
            if (navVertIndex - 1 < 0)
            {
                if (navNode.isClosed)
                {
                    neighbourLeft = navNode.verts.Length - 1;
                }
                else
                    neighbourLeft = -1;
            }
            else
            {
                neighbourLeft = navVertIndex - 1;
            }
        }
    }

    class PathNode_Link : IPathNode
    {
        int linkIndex;

        public PathNode_Link(IPathNode parent, NavNode navNode, int navVertIndex, float costSoFar, float estimatedCost, int linkIndex) : base(parent, navNode, navVertIndex, costSoFar, estimatedCost)
        {
            this.linkIndex = linkIndex;
        }

        public override Vector2 StartPoint
        {
            get
            {
                return Link.endPoint;
            }
        }

        public IOffNodeLink Link { get { return parent.navNode.links[linkIndex]; } }

        public override Vector2 EndPoint
        {
            get
            {
                return Link.endPoint;
            }
        }

        public override void GetNeighbours(out int neighbourLeft, out int neighbourRight)
        {
            var endNode = NavNode;
            if (Link.targetVertIndex + 1 >= endNode.verts.Length)
            {
                if (endNode.isClosed)
                {
                    neighbourRight = 0;
                }
                else
                    neighbourRight = -1;
            }
            else
            {
                neighbourRight = Link.targetVertIndex + 1;
            }
            if (Link.targetVertIndex - 1 < 0)
            {
                if (endNode.isClosed)
                {
                    neighbourLeft = endNode.verts.Length - 1;
                }
                else
                    neighbourLeft = -1;
            }
            else
            {
                neighbourLeft = Link.targetVertIndex - 1;
            }
        }
    }
}

public class PathRequest
{
    public delegate void PathCompleted(NavPath path);

    public readonly Vector2 start;
    public readonly Vector2 goal;
    public readonly LightSkin skin;
    public readonly LightMarker[] lightMarker;
    public readonly PathCompleted callback;

    public PathRequest(Vector2 start, Vector2 goal, PathCompleted callback, LightSkin skin, params LightMarker[] lightMarker)
    {
        this.skin = skin;
        this.start = start;
        this.goal = goal;
        this.callback = callback;
        this.lightMarker = lightMarker;
    }
}

[Serializable]
public class NavPath
{
    public IPathSegment[] pathSegments;
    public Vector2 goal;

    public NavPath(IPathSegment[] pathSegments, Vector2 goal)
    {
        this.pathSegments = pathSegments;
        this.goal = goal;
    }

    public void Visualize()
    {
        foreach (IPathSegment seg in pathSegments)
        {
            seg.Visualize();
        }
    }
}

