using UnityEngine;
using System.Collections.Generic;
using Polygon2D;

namespace NavMesh2D.Core
{
    public class Contour
    {

        public PointChain pointChain;
        public List<Contour> children;
        public Bounds Bounds { get { return pointChain.Bounds; } }
        public bool IsSolid { get { return isSolid; } }

        private string name;
        private bool isSolid;

        public Contour(PointChain chain, bool isSolid, string name)
        {
            pointChain = chain;
            children = new List<Contour>(1);
            this.isSolid = isSolid;
            this.name = name;
        }

        public bool TryAddContour(Contour outline)
        {
            PointChain[] result;
            PolygonClipper.ResultType resultType = PolygonClipper.Compute(pointChain, outline.pointChain, (isSolid) ? PolygonClipper.BoolOpType.UNION : PolygonClipper.BoolOpType.DIFFERENCE, out result);
            if (resultType != PolygonClipper.ResultType.SuccesfullyCliped)
            {
                Debug.Log(name + " does not clip with " + outline.name + ". Fehlercode: " + resultType);
                return false;
            }
            else if (result.Length < 1)
            {
                Debug.LogError(name + " does not clip with " + outline.name +". Fehlercode: " + resultType);
                return false;
            }
            name = name + "+" + outline.name;
            pointChain = result[0]; //Update this contour
            for (int iChild = 0; iChild < children.Count; iChild++)
            {
                children[iChild].TryAddContour(outline);
            }

            children.Clear();
            for (int iChain = 1; iChain > result.Length; iChain++)
            {
                children.Add(new Contour(result[iChain], false, name + "/"));
            }
            return true;
        }

        public void DrawForDebug()
        {
            Gizmos.color = Color.green;
            pointChain.DrawForDebug();
            for (int iChildren = 0; iChildren < children.Count; iChildren++)
            {
                children[iChildren].DrawForDebug();
            }
        }
    }
}
