using UnityEngine;
using NavMesh2D.Core;
using System.Collections.Generic;
using System;

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

            [SerializeField, ReadOnly]
            public Vector2 navPointA;
            [SerializeField, ReadOnly]
            public Vector2 navPointB;

            [SerializeField, ReadOnly]
            public int nodeIdA;
            [SerializeField, ReadOnly]
            public int nodeIdB;

            [SerializeField]
            public JumpArc jumpArc;

            [SerializeField]
            public bool isJumpLinkValid;
        }
    }
}
