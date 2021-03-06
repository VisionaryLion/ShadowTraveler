﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace NavMesh2D.Core
{
    public class ExpandedTree : ScriptableObject, ISerializationCallbackReceiver
    {
        [NonSerialized]
        public ExpandedNode headNode;
        [SerializeField]
        ExpandedNode[] headNodeChildren;

        public float mapPointMaxDeviation = 3;

        public static ExpandedTree Build(ContourTree contourTree, float height)
        {
            //Create initial tree
            ExpandedTree initialTree = ScriptableObject.CreateInstance<ExpandedTree>();
            initialTree.Init(contourTree);

            //Mark all Segments, that don't pass the minimum height
            initialTree.headNode.Mark(height);

            //initialTree.headNode.Scale(-heighLevel[0]);

            return initialTree;
        }

        public void Init (ContourTree cTree)
        {
            headNode = new ExpandedNode();

            foreach (ContourNode cN in cTree.FirstNode.children)
            {
                headNode.children.Add(new ExpandedNode(cN));
            }
        }

        public bool TryMapPointToContour(Vector2 point, out Vector2 mappedPos, out Vector2 normal)
        {
            Stack<ExpandedNode> nodesToProcess = new Stack<ExpandedNode>(10);
            Stack<ExpandedNode> nodesToCheckDistance = new Stack<ExpandedNode>(10);
            for (int iChild = 0; iChild < headNode.children.Count; iChild++)
            {
                nodesToProcess.Push(headNode.children[iChild]);
            }

            while (nodesToProcess.Count != 0)
            {
                ExpandedNode cNode = nodesToProcess.Pop();

                //Extended bounds test
                if (cNode.contour.bounds.min.x - mapPointMaxDeviation > point.x || cNode.contour.bounds.max.x + mapPointMaxDeviation < point.x
                || cNode.contour.bounds.min.y - mapPointMaxDeviation > point.y || cNode.contour.bounds.max.y + mapPointMaxDeviation < point.y)
                {
                    //Failed test
                    continue;
                }

                if (cNode.contour.Contains(point))
                {
                    nodesToProcess.Clear();
                    nodesToCheckDistance.Clear();
                    for (int iChild = 0; iChild < cNode.children.Count; iChild++)
                    {
                        nodesToProcess.Push(cNode.children[iChild]);
                    }
                    if (!cNode.contour.isSolid)
                        nodesToCheckDistance.Push(cNode);
                }
                else if(cNode.contour.isSolid)
                {
                    nodesToCheckDistance.Push(cNode);
                }
            }

            bool foundOne = false;
            //Assign some dummy values
            mappedPos = Vector2.zero;
            float minDistance = float.MaxValue;
            float dist;
            Vector2 cPoint;
            Vector2 tangent;
            Vector2 minTangent = Vector2.zero;

            while (nodesToCheckDistance.Count != 0)
            {
                ExpandedNode cNode = nodesToCheckDistance.Pop();
                cPoint = cNode.contour.ClosestPointOnContour(point, out dist, out tangent);
                if (dist < minDistance)
                {
                    mappedPos = cPoint;
                    minDistance = dist;
                    minTangent = tangent;
                    foundOne = true;
                }
            }
            normal = new Vector2(minTangent.y, -minTangent.x).normalized;
            return foundOne;
        }

        public void OnBeforeSerialize()
        {
            headNodeChildren = headNode.children.ToArray();
        }

        public void OnAfterDeserialize()
        {
            headNode = new ExpandedNode();
            headNode.children = new List<ExpandedNode>(headNodeChildren);
            headNodeChildren = null;
        }
    }
}
