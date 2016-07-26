using UnityEngine;
using System.Collections.Generic;

namespace Pathfinding2D
{
    public class NavMesh2D : ScriptableObject {

        //identifiers
        public new string name;
        public int version;

        public List<PathNode> staticNodes;
        public List<DynamicPathNode> dynamicNodes;
        public List<JumpLink> staticJumpLinks;
        public List<DynamicJumpLink> dynamicJumpLink;
    }
}
