using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NavMesh2D.Core;

namespace NavData2d.Editor
{
    public class NavData2dBuildContainer : ScriptableObject
    {
        [SerializeField]
        internal NavAgentGroundWalkerSettings navAgentSettings;
        [SerializeField]
        internal ColliderSet colliderSet;
        [SerializeField]
        internal ContourTree contourTree;
        [SerializeField]
        internal ContourTree strippedContourTree;
        [SerializeField]
        internal NavigationData2D prebuildNavData;
        [SerializeField]
        internal NavigationData2D filteredNavData;
        [SerializeField]
        internal List<MetaJumpLink> jumpLinks;
        [SerializeField]
        internal float nodeMergeDistance;
        [SerializeField]
        internal float maxEdgeDeviation;
        //DebugVertSet 
    }
}
