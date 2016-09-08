using UnityEngine;
using System.Collections.Generic;
using Utility.Polygon2D;
using Utility.ExtensionMethods;

namespace NavMesh2D.Core
{
    public class ContourTree
    {
        ContourNode headNode; // root

        public ContourNode FirstNode { get { return headNode; } }

        public ContourTree()
        {
            //create biggest contour possible
            headNode = new ContourNode(null, false);
        }

        public static ContourTree Build(CollisionGeometrySet cgSet)
        {
            ContourTree result = new ContourTree();
            for (int iCol = 0; iCol < cgSet.colliderVerts.Count; iCol++)
            {
                Contour contour = new Contour(cgSet.colliderVerts[iCol]);
                result.AddOutline(contour);
            }
            return result;
        }

        public void AddOutline(Contour outline)
        {
            bool consumed = false;
            headNode.AddSolidContour(outline, ref consumed);
            /*PrintDebugForChild(headNode, "");
            Debug.Log("****************************");*/
        }

        private void PrintDebugForChild(ContourNode node, string prefix)
        {
            for (int iChild = 0; iChild < node.children.Count; iChild++)
            {
                if (node.children[iChild].children.Count == 0)
                    Debug.Log(prefix + "/" + node.children[iChild].contour.VertexCount + "," + node.children[iChild].IsSolid);
                else
                    PrintDebugForChild(node.children[iChild], prefix + "/" + node.children[iChild].contour.VertexCount + "," + node.children[iChild].IsSolid);
            }
        }

        public void DrawDebugInfo()
        {
            for (int iOutline = 0; iOutline < headNode.children.Count; iOutline++)
            {
                headNode.children[iOutline].DrawForDebug();
            }
        }
    }
}
