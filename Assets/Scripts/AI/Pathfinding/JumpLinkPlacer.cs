using UnityEngine;
using NavMesh2D.Core;
using System.Collections.Generic;
using System;
using NavMesh2D;

namespace Pathfinding2D
{
    public class JumpLinkPlacer : ScriptableObject
    {
        [SerializeField]
        public List<JumpLink> jumpLinks;

        public void Init()
        {
            jumpLinks = new List<JumpLink>(5);
        }

        [Serializable]
        public class JumpLink
        {
            [SerializeField]
            public Vector2 worldPointA;
            [SerializeField]
            public Vector2 worldPointB;

            [SerializeField, Range(0, 1)]
            public float xSpeedScale = 1;

            [SerializeField, ReadOnly]
            public Vector2 navPointA;
            [SerializeField, ReadOnly]
            public Vector2 navPointB;

            [SerializeField, ReadOnly]
            public int nodeIndexA;
            [SerializeField, ReadOnly]
            public int nodeIndexB;

            [SerializeField, ReadOnly]
            public int nodeVertIndexA;
            [SerializeField, ReadOnly]
            public int nodeVertIndexB;

            [SerializeField]
            public JumpArcSegment jumpArc;

            [SerializeField]
            public bool isJumpLinkValid;

            public JumpLink()
            {

            }

            public JumpLink(Vector2 worldPointA, Vector2 worldPointB)
            {
                this.worldPointA = worldPointA;
                this.worldPointB = worldPointB;
            }

            public bool TryRemapPoints(NavigationData2D navData)
            {
                Vector2 navPoint;
                int mappedVertIndex;
                int mappedNodeIndex;
                if (navData.TryMapPoint(worldPointA, out navPoint, out mappedVertIndex, out mappedNodeIndex))
                {
                    this.navPointA = navPoint;
                    this.nodeIndexA = mappedNodeIndex;
                    this.nodeVertIndexA = mappedVertIndex;
                }
                else
                {
                    return false;
                }

                if (navData.TryMapPoint(worldPointB, out navPoint, out mappedVertIndex, out mappedNodeIndex))
                {
                    this.navPointB = navPoint;
                    this.nodeIndexB = mappedNodeIndex;
                    this.nodeVertIndexB = mappedVertIndex;
                }
                else
                {
                    return false;
                }
                return true;
            }
        }
    }
}
