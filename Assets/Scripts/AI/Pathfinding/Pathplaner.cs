using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NavMesh2D;
using Priority_Queue;

public class PathPlaner : MonoBehaviour
{

    public static PathPlaner Instance { get { return instance; } }
    private static PathPlaner instance;

    [SerializeField]
    NavigationData2D navData;

    void Awake()
    {
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
            //request.callback(null);
            return;
        }
        DebugExtension.DebugCircle(start_point, Vector3.forward, Color.magenta, 0.1f);
        Debug.DrawLine(start_point, request.start, Color.white);

        NavNode goal_node;
        int goal_vertIndex;
        Vector2 goal_point;
        if (!navData.TryMapPoint(request.goal, out goal_point, out goal_vertIndex, out goal_node))
        {
            //No path can be found!
            Debug.Log("Couldn't map points. -> no path");
            //request.callback(null);
            return;
        }
        DebugExtension.DebugCircle(goal_point, Vector3.forward, Color.magenta, 0.1f);
        Debug.DrawLine(goal_point, request.goal, Color.white);

        //Trivial case: Start and Goal are on the same line
        if (start_node == goal_node && goal_vertIndex == start_vertIndex)
        {
            Debug.Log("Are on same Edge");
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
                //Found path!
                goto FoundPath;
            }

            closedList.Add(cNode);
            DebugExtension.DebugPoint(cNode.NVert.PointB, Color.red, 0.5f);

            if (cNode.navVertIndex - 1 >= 0)
            {
                newVertIndex = cNode.navVertIndex - 1;
                if (!IsNodeClosed(cNode.navNode, newVertIndex))
                {
                    costSoFar = cNode.costSoFar + Vector2.Distance(cNode.NVert.PointB, cNode.navNode.verts[newVertIndex].PointB);
                    PathNode newNode = new PathNode(cNode, cNode.navNode, newVertIndex, costSoFar, Vector2.Distance(goal_point, cNode.navNode.verts[newVertIndex].PointB));
                    openList.Enqueue(newNode, newNode.costSoFar);
                }
            }
            else if (cNode.navNode.isClosed)
            {
                newVertIndex = cNode.navNode.verts.Length - 1;
                if (!IsNodeClosed(cNode.navNode, newVertIndex))
                {
                    costSoFar = cNode.costSoFar + Vector2.Distance(cNode.NVert.PointB, cNode.navNode.verts[newVertIndex].PointB);
                    PathNode newNode = new PathNode(cNode, cNode.navNode, newVertIndex, costSoFar, Vector2.Distance(goal_point, cNode.navNode.verts[newVertIndex].PointB));
                    openList.Enqueue(newNode, newNode.costSoFar);
                }
            }

            if (cNode.navVertIndex + 1 < cNode.navNode.verts.Length)
            {
                newVertIndex = cNode.navVertIndex + 1;
                if (!IsNodeClosed(cNode.navNode, newVertIndex))
                {
                    costSoFar = cNode.costSoFar + Vector2.Distance(cNode.NVert.PointB, cNode.navNode.verts[newVertIndex].PointB);
                    PathNode newNode = new PathNode(cNode, cNode.navNode, newVertIndex, costSoFar, Vector2.Distance(goal_point, cNode.navNode.verts[newVertIndex].PointB));
                    openList.Enqueue(newNode, newNode.costSoFar);
                }
            }
            else if (cNode.navNode.isClosed)
            {
                newVertIndex = 0;
                if (!IsNodeClosed(cNode.navNode, newVertIndex))
                {
                    costSoFar = cNode.costSoFar + Vector2.Distance(cNode.NVert.PointB, cNode.navNode.verts[newVertIndex].PointB);
                    PathNode newNode = new PathNode(cNode, cNode.navNode, newVertIndex, costSoFar, Vector2.Distance(goal_point, cNode.navNode.verts[newVertIndex].PointB));
                    openList.Enqueue(newNode, newNode.costSoFar);
                }
            }

            if (cNode.navNode.verts[cNode.navVertIndex].linkIndex.Length > 0)
            {
                int[] linkIndecies = cNode.navNode.verts[cNode.navVertIndex].linkIndex;
                for (int iLink = 0; iLink < linkIndecies.Length; iLink++)
                {
                    NavNodeLink cLink = cNode.navNode.links[linkIndecies[iLink]];
                    if (!IsNodeClosed(navData.nodes[cLink.targetNodeIndex], cLink.targetVertIndex))
                    {
                        costSoFar = cNode.costSoFar + Vector2.Distance(cLink.startPoint, cLink.endPoint);
                        PathNode newNode = new PathNode(cNode, navData.nodes[cLink.targetNodeIndex], cLink.targetVertIndex, costSoFar, Vector2.Distance(goal_point, cLink.endPoint));
                        openList.Enqueue(newNode, newNode.costSoFar);
                    }
                }
            }
        }

        Debug.Log("No path found!");
        return;

        FoundPath:
        Debug.Log("Path found!");
        Debug.DrawLine(cNode.NVert.PointB, goal_point, Color.green);
        while (cNode.parent != null)
        {
            if (cNode.parent.parent == null)
                Debug.DrawLine(cNode.NVert.PointB, start_point, Color.green);
            else
                Debug.DrawLine(cNode.NVert.PointB, cNode.parent.NVert.PointB, Color.green);

            DebugExtension.DebugCircle(cNode.NVert.PointB, Vector3.forward, Color.green, 0.12f);
            cNode = cNode.parent;
        }
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
        public PathNode parent;

        public NavVert NVert { get { return navNode.verts[navVertIndex]; } }


        public PathNode(PathNode parent, NavNode navNode, int navVertIndex, float costSoFar, float estimatedCost)
        {
            this.navNode = navNode;
            this.navVertIndex = navVertIndex;
            this.costSoFar = costSoFar;
            totalCost = estimatedCost = costSoFar;
            this.parent = parent;
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

public class NavPath
{

}
