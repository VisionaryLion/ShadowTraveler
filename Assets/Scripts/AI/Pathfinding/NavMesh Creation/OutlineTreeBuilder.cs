using UnityEngine;
using System.Collections;

namespace NavMesh2D.Core
{
    public class OutlineTreeBuilder
    {

        public OutlineTree Build(CollisionGeometrySet cgSet)
        {
            OutlineTree result = new OutlineTree();

            for (int iCol = 0; iCol < cgSet.colliderVerts.Count; iCol++)
            {
                PointChain chain = new PointChain(cgSet.colliderVerts[iCol], true);
                Contour outline = new Contour(chain, true, cgSet.colliderNames[iCol]);
                result.AddOutline(outline);
            }

            return result;
        } 
    }
}
