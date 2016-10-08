using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NavMesh2D;

public class PathPlaner : MonoBehaviour {

    public static PathPlaner Instance { get { return instance; } }
    private static PathPlaner instance;

    [SerializeField]
    NavigationData2D navData;

    void Awake()
    {
        instance = this;
    }

    Stack<PathRequest> requestBuffer;

    public PathPlaner()
    {
        requestBuffer = new Stack<PathRequest>(5);
    }

    public void RequestPath (PathRequest request)
    {
        requestBuffer.Push(request);
    }

    void FindRequestedPath (PathRequest request)
    {
        NavNode start_node;
        int start_vertIndex;
        Vector2 start_point;
        if (!navData.TryMapPoint(request.start, out start_point, out start_vertIndex, out start_node))
        {
            //No path can be found!
            request.callback(null);
        }

        NavNode goal_node;
        int goal_vertIndex;
        Vector2 goal_point;
        if (!navData.TryMapPoint(request.goal, out goal_point, out goal_vertIndex, out goal_node))
        {
            //No path can be found!
            request.callback(null);
        }

        //Trivial case: Start and Goal are on the same line
        if (goal_vertIndex == start_vertIndex)
        {

        }

        Utility.HeapPriorityQueue<PathNode> openList = new Utility.HeapPriorityQueue<PathNode>();
    }

    class PathNode : Utility.PriorityQueueNode
    {
        int navVertIndex;
        NavNode navNode;

        public PathNode(int navVertIndex, NavNode navNode)
        {
            this.navVertIndex = navVertIndex;
            this.navNode = navNode;
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
