using UnityEngine;
using System.Collections;
using System;
using CC2D;

namespace Pathfinding2D
{
    [Serializable]
    public class JumpLink : IOffNodeLink
    {
        public float xVel;
        public float jumpForce;
        public float xMin;
        public float xMax;
        public float yMin;
        public float yMax;

        public JumpLink(JumpLinkPlacer.JumpLink link)
        {
            xVel = link.jumpArc.v;
            jumpForce = link.jumpArc.j;
            targetNodeIndex = link.nodeIndexB;
            targetVertIndex = link.nodeVertIndexB;
            startPoint = link.navPointA;
            endPoint = link.navPointB;
            xMin = link.jumpArc.minX;
            xMax = link.jumpArc.maxX;
            yMin = link.jumpArc.minY;
            yMax = link.jumpArc.maxY;
            traversCosts = xMax - xMin;
        }
    }
}
