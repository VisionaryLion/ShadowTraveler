using UnityEngine;
using System.Collections;

namespace NavMesh2D.Core
{
    public class ExpandedTree
    {
        public ExpandedNode headNode;

        public ExpandedTree(ContourTree cTree, int travereTestCount)
        {
            headNode = new ExpandedNode();

            foreach (ContourNode cN in cTree.FirstNode.children)
            {
                headNode.children.Add(new ExpandedNode(cN, travereTestCount));
            }
        }
    }
}
