using UnityEngine;
using System.Collections.Generic;
using Utility.Polygon2D;
using Utility.ExtensionMethods;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NavMesh2D.Core
{
    public class ContourTree : ScriptableObject
    {
        [SerializeField]
        ContourNode headNode; // root

        public ContourNode FirstNode { get { return headNode; } }

        public void OnEnabled()
        {
            //create biggest contour possible
            headNode = new ContourNode(null, false);
        }

        public static ContourTree Build(CollisionGeometrySet cgSet)
        {
            ContourTree result = ScriptableObject.CreateInstance<ContourTree>();
            result.OnEnabled();
            for (int iCol = 0; iCol < cgSet.colliderVerts.Count; iCol++)
            {
                result.AddContour(cgSet.colliderVerts[iCol]);
            }
            return result;
        }

        public void AddContour(Contour outline)
        {
            headNode.AddSolidContour(outline);
        }

        public void AddContour(Vector2[] verts)
        {
            headNode.AddSolidContour(new Contour(verts));
        }

        public void VisualDebug()
        {
            int debugColorID = 0;
            for (int iOutline = 0; iOutline < headNode.children.Count; iOutline++)
            {
                headNode.children[iOutline].VisualDebug(++debugColorID);
            }
        }
    }
}
