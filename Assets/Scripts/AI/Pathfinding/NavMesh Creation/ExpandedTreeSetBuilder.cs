using UnityEngine;
using System.Collections;

namespace NavMesh2D.Core
{
    public static class ExpandedTreeSetBuilder
    {
        
        public static ExpandedTree[] Build(ContourTree contourTree, float[] heighLevel)
        {
            //Create initial tree
            ExpandedTree initialTree = new ExpandedTree(contourTree, 1);

            //Mark all Segments, that don't pass the minimum height
            initialTree.headNode.Mark(heighLevel[0],0);

            //initialTree.headNode.Scale(-heighLevel[0]);

            return new ExpandedTree[] { initialTree };
        }
    }
}
