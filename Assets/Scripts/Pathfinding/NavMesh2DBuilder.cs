using UnityEngine;
using System.Collections.Generic;
using System;
using Polygon2D;
using Utility;
using System.Diagnostics;

namespace Pathfinding2D
{
    public class NavMesh2DBuilder
    {
        public int BuildProgress { get { return _progress; } }
        public int CircleVertCount { get { return _vertCountCircle; } set { _radianPerVert = (Mathf.PI * 2) / value; _vertCountCircle = value; } }
        public int CollisionLayerMask { get; set; }

        float _radianPerVert; // Which angle should lie between two verts in a circle?
        int _vertCountCircle; // How many verts should represent a circle?
        volatile int _progress;

        public NavMesh2DBuilder(int colliderLayer, int circleVertCount = 10)
        {
            CircleVertCount = circleVertCount;
            CollisionLayerMask = colliderLayer;
        }

        public LinkedList<PointChain> Build()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            LinkedList<PointChain> inOutList = ReadCollisionGeometry();
            int itemCount = inOutList.Count;
            inOutList = UnifyCollisionGeometry(inOutList);
            stopwatch.Stop();

            UnityEngine.Debug.Log("Finished baking NavMesh for "+ itemCount+" items in "+stopwatch.ElapsedMilliseconds+" ms");
            return inOutList;
        }

        private LinkedList<PointChain> UnifyCollisionGeometry(LinkedList<PointChain> chains)
        {
            HeapPriorityQueue<PointChainNode> queue = new HeapPriorityQueue<PointChainNode>(chains.Count * 2);

            //Enqueue pointchains
            foreach (PointChain pChain in chains)
            {
                EnquePointChain(queue, pChain);
            }

            LinkedList<PointChain> sweepRay = new LinkedList<PointChain>();
            LinkedList<PointChain> result = new LinkedList<PointChain>();

            while (queue.Count != 0)
            {
                PointChainNode pNode = queue.Dequeue();
                if (pNode.left)
                {
                    LinkedListNode<PointChain> pNodeNode = sweepRay.First;
                    while (pNodeNode != null)
                    {
                        PointChain[] unifiedResult = PolygonClipper.Compute(pNodeNode.Value, pNode.chain, PolygonClipper.BoolOpType.UNION);
                        // pNodeNode.Value = 
                        pNodeNode = pNodeNode.Next;
                    } 
                    sweepRay.AddLast(pNode.chain);
                }
                else
                {
                    sweepRay.Remove(pNode.chain);
                    result.AddLast(pNode.chain);
                }
            }
            return result;
        }

        private LinkedList<PointChain> ResolveSelfIntersections()
        {
            return null;
        }

        private void EnquePointChain(HeapPriorityQueue<PointChainNode> queue, PointChain chain)
        {
            PointChainNode n1 = new PointChainNode(chain, true);
            PointChainNode n2 = new PointChainNode(chain, false);
            n1.other = n2;
            n2.other = n1;
            queue.Enqueue(n1);
            queue.Enqueue(n2);
        }

        private LinkedList<PointChain> ReadCollisionGeometry()
        {
            //Find all collider that fullfill the requirements.
            // 1. Is within the collision layer
            // 2. Is activated
            // 3. Isnt a trigger
            //Then convert them to a pointchain.

            LinkedList<PointChain> result = new LinkedList<PointChain>();
            Collider2D[] allCollider = GameObject.FindObjectsOfType<Collider2D>(); //Load all colliders
            Collider2D pCol;

            for (int iCol = 0; iCol < allCollider.Length; iCol++)
            {
                pCol = allCollider[iCol];

                if (((1 << pCol.gameObject.layer) | CollisionLayerMask) != CollisionLayerMask)
                    continue;

                if (pCol.isTrigger)
                    continue;

                if (!pCol.enabled)
                    continue;
                //Collider fullfills the requirements. Now process it!
                    result.AddLast(ConvertColliderToPointChain(pCol));
            }
            return result;
        }

        private PointChain ConvertColliderToPointChain (Collider2D collider)
        {
            Type cTyp = collider.GetType();
            Vector2[] verts;
            Bounds bounds = collider.bounds;
            if (cTyp == typeof(BoxCollider2D))
            {
                BoxCollider2D pCol = collider as BoxCollider2D;
                verts = new Vector2[4];

                Vector2 halfSize = pCol.size / 2;
                verts[0] = pCol.transform.TransformPoint(halfSize + pCol.offset);
                verts[1] = pCol.transform.TransformPoint(new Vector2(halfSize.x, -halfSize.y) + pCol.offset);
                verts[2] = pCol.transform.TransformPoint(-halfSize + pCol.offset);
                verts[3] = pCol.transform.TransformPoint(new Vector2(-halfSize.x, halfSize.y) + pCol.offset);
            }
            else if (cTyp == typeof(CircleCollider2D))
            {
                CircleCollider2D pCol = collider as CircleCollider2D;
                verts = new Vector2[_vertCountCircle];

                for (int i = 0; i < _vertCountCircle; i++)
                {
                    verts[i] = collider.transform.TransformPoint(new Vector2(pCol.radius * Mathf.Sin(_radianPerVert * i), pCol.radius * Mathf.Sin(_radianPerVert * i)));
                }
            }
            else if (cTyp == typeof(EdgeCollider2D))
                verts = ((EdgeCollider2D)collider).points;
            else
                verts = ((PolygonCollider2D)collider).points;

            PointChain result = new PointChain(verts, true);
            result.Bounds = bounds;
            return result;
        }

        class PointChainNode : PriorityQueueNode
        {
            public PointChain chain;
            public bool left;
            public PointChainNode other;
            public Vector2 point;

            public PointChainNode(PointChain chain, bool left)
            {
                this.chain = chain;
                this.left = left;
                point = (left) ? chain.Bounds.min : chain.Bounds.max;
            }

            public override int CompareTo(PriorityQueueNode other)
            {
                if (other.GetType() == typeof(PointChainNode))
                {
                    PointChainNode otherChain = other as PointChainNode;
                    if (point.x < otherChain.point.x)
                        return -1;
                    if (point.x > otherChain.point.x)
                        return 1;
                    if (left)
                        return 1;
                    return -1;
                }
                return base.CompareTo(other);
            }
        }
    }
}
