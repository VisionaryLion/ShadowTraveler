using UnityEngine;
using System.Collections.Generic;
using Utility.Polygon2D;
using System;

namespace NavMesh2D.Core
{
    [Serializable]
    public class ExpandedNode : ISerializationCallbackReceiver
    {
        [NonSerialized]
        public MarkableContour contour;
        [SerializeField]
        SerializableMarkableContour serializableMarkableContour;
        public List<ExpandedNode> children;

        public ExpandedNode(ContourNode contourNode, int traverseTestCount)
        {
            contour = new MarkableContour(contourNode.contour, contourNode.IsSolid, true, traverseTestCount);

            children = new List<ExpandedNode>(contourNode.children.Count);
            foreach (ContourNode cNode in contourNode.children)
                children.Add(new ExpandedNode(cNode, traverseTestCount));
        }

        public ExpandedNode()
        {
            children = new List<ExpandedNode>();
        }

        public void Mark(float minWalkableHeight, int testIndex)
        {
            if (contour == null)
            {
                for (int iChild = 0; iChild < children.Count; iChild++)
                {
                    children[iChild].contour.Mark(children, minWalkableHeight, testIndex);
                }
            }
            else if (!contour.isSolid)
            {
                if (children.Count == 0)
                    contour.MarkSelfOnly(minWalkableHeight, testIndex);
                else {
                    contour.Mark(children, minWalkableHeight, testIndex);
                    List<ExpandedNode> includingThis = new List<ExpandedNode>(children.Count + 1);
                    includingThis.AddRange(children);
                    includingThis.Add(this);
                    for (int iChild = 0; iChild < children.Count; iChild++)
                    {
                        children[iChild].contour.Mark(includingThis, minWalkableHeight, testIndex);
                    }
                }
            }
            foreach (ExpandedNode eN in children)
                eN.Mark(minWalkableHeight, testIndex);
        }

        public void VisualDebug(int targetHeightTest)
        {
            if(contour != null)
            contour.VisualDebug(targetHeightTest);

            foreach (ExpandedNode eN in children)
                eN.VisualDebug(targetHeightTest);
        }

        public void OnBeforeSerialize()
        {
            serializableMarkableContour = new SerializableMarkableContour(contour);
        }

        public void OnAfterDeserialize()
        {
            contour = new MarkableContour(serializableMarkableContour);
            serializableMarkableContour = null;
        }
    }
}
