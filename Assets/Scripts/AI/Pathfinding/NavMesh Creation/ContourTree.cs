using UnityEngine;
using System.Collections.Generic;
using Utility.Polygon2D;
using Utility.ExtensionMethods;
#if UNITY_EDITOR
using UnityEditor;
<<<<<<< HEAD
#endif
=======
using System;
using System.Collections;
>>>>>>> origin/CloudsLevel-01

namespace NavMesh2D.Core
{
    [Serializable]
    public class ContourTree : IEnumerable<ContourNode>
    {
        [SerializeField]
        ContourNode headNode; // root

        public ContourNode FirstNode { get { return headNode; } }

        public ContourTree()
        {
            //create biggest contour possible
            headNode = new ContourNode(null, false);
        }

        public static ContourTree Build(CollisionGeometrySet cgSet, float nodeMergeDist, float maxEdgeDeviation)
        {
            ContourTree result = Build(cgSet);
            result.Optimize(nodeMergeDist, maxEdgeDeviation);
            return result;
        }

        public static ContourTree Build(CollisionGeometrySet cgSet)
        {
            ContourTree result = new ContourTree();
            for (int iCol = 0; iCol < cgSet.colliderVerts.Count; iCol++)
            {
                result.AddContour(cgSet.colliderVerts[iCol]);
            }
            return result;
        }

        public void Optimize(float nodeMergeDist, float maxEdgeDeviation)
        {
            Stack<ContourNode> nodesToProcess = new Stack<ContourNode>(headNode.children);

            while (nodesToProcess.Count > 0)
            {
                ContourNode cn = nodesToProcess.Pop();
                cn.contour.Optimize(nodeMergeDist, maxEdgeDeviation);
                for (int iOutline = 0; iOutline < cn.children.Count; iOutline++)
                {
                    nodesToProcess.Push(cn.children[iOutline]);
                }
            }
        }

        public void AddContour(Contour outline)
        {
            headNode.AddSolidContour(outline);
        }

        public void AddContour(Vector2d[] verts)
        {
            headNode.AddSolidContour(new Contour(verts));
        }

        public void VisualDebug()
        {
            int debugColorID = 0;
            for (int iOutline = 0; iOutline < headNode.children.Count; iOutline++)
            {
                headNode.children[iOutline].VisualDebug(++debugColorID);
            }
        }

        public IEnumerator<ContourNode> GetEnumerator()
        {
            return new ContourNodeIEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ContourNodeIEnumerator(this);
        }

        public class ContourNodeIEnumerator : IEnumerator<ContourNode>
        {
            public ContourNode Current
            {
                get
                {
                    ContourNode result = tree.FirstNode;
                    foreach (var i in nodePointer)
                    {
                        result = result.children[i];
                    }
                    return result;
                }
            }            

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            List<int> nodePointer;
            ContourTree tree;

            public ContourNodeIEnumerator(ContourTree tree)
            {
                this.tree = tree;
                nodePointer = new List<int>(4);
                nodePointer.Add(-1);
            }

            public void Dispose()
            {
                
            }

            public bool MoveNext()
            {
                if (nodePointer[0] == -1)
                {
                    nodePointer[0] = 0;
                    return true;
                }

                ContourNode result = tree.FirstNode;
                for (int iNode = 0; iNode < nodePointer.Count - 1; iNode++)
                {
                    result = result.children[nodePointer[iNode]];
                }

                if (result.children[nodePointer[nodePointer.Count - 1]].children.Count > 0) //Current node has children, do them next
                {
                    nodePointer.Add(0);
                    return true;
                }

                if (nodePointer[nodePointer.Count - 1] + 1 < result.children.Count) //There is another node at the parent level
                {
                    nodePointer[nodePointer.Count - 1]++; //increase the index on the parent level
                    return true;
                }

                nodePointer.RemoveAt(nodePointer.Count - 1); //remove current node, as it is done!
                if (nodePointer.Count == 0)
                    return false;
                //Get the next node on a higher level
                int parentIndex = nodePointer.Count - 2;
                do {
                    result = tree.FirstNode;
                    for (int iNode = 0; iNode < nodePointer.Count - 1; iNode++)
                    {
                        result = result.children[nodePointer[iNode]];
                    }

                    if (nodePointer[nodePointer.Count - 1] + 1 < result.children.Count) //There is another node at this level
                    {
                        nodePointer[nodePointer.Count - 1]++;
                        return true;
                    }
                    nodePointer.RemoveAt(nodePointer.Count - 1); //remove current node, as it is done!
                } while ((--parentIndex) >= 0);

                //finished enumerating
                return false;
            }

            public void Reset()
            {
                nodePointer = new List<int>(4);
                nodePointer.Add(-1);
            }
        }
    }
}
