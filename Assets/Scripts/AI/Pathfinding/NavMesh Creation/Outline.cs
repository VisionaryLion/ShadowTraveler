using UnityEngine;
using System.Collections.Generic;
using Polygon2D;

namespace NavMesh2D.Core
{
    public class Outline
    {

        public PointChain pointChain;
        public List<Outline> children;
        public Bounds Bounds { get { return pointChain.Bounds; } }
        public bool IsSolid { get { return isSolid; } }

        private bool isSolid;

        public Outline(PointChain chain, bool isSolid)
        {
            pointChain = chain;
            children = new List<Outline>(1);
            this.isSolid = isSolid;
        }

        public bool TryAddOutline(Outline outline, OutlineTree tree)
        {
            if (!DoBoundsIntersect(outline))
                return false;
            for (int iChild = 0; iChild < children.Count; iChild++)
            {
                if (children[iChild].TryAddOutline(outline, tree))
                    return true;
            }
            /*if (DoesFullyEncompass(outline))
            {
                if (!isSolid)
                {
                    children.Add(outline);
                    return true;
                }
                else
                {
                    for (int iChild = 0; iChild < outline.children.Count; iChild++)
                        children.Add(outline.children[iChild]);
                    return true;
                }
            }*/
            if (isSolid && outline.IsSolid)
            {
                PointChain[] result;
                PolygonClipper.ResultType resultType = PolygonClipper.Compute(pointChain, outline.pointChain, PolygonClipper.BoolOpType.UNION, out result);
                if (resultType != PolygonClipper.ResultType.SuccesfullyCliped || result.Length > 1)
                    return false;
                this.pointChain = result[0];
                //Changed our appearance, so add again.
                tree.ReAddOutline(outline);
                Debug.Log(result.Length);
                return true;
            }
            //Do some merge stuff
            return false;
        }

        public bool TryMergeOutline(Outline outline)
        {
            if (!DoBoundsIntersect(outline))
                return false;
            for (int iChild = 0; iChild < children.Count; iChild++)
            {
                if (children[iChild].TryMergeOutline(outline))
                    return true;
            }
            /*if (DoesFullyEncompass(outline))
            {
                if (!isSolid)
                {
                    children.Add(outline);
                    return true;
                }
                else
                {
                    for (int iChild = 0; iChild < outline.children.Count; iChild++)
                        children.Add(outline.children[iChild]);
                    return true;
                }
            }*/
            if (isSolid && outline.IsSolid)
            {
                PointChain[] result;
                PolygonClipper.ResultType resultType = PolygonClipper.Compute(pointChain, outline.pointChain, PolygonClipper.BoolOpType.UNION, out result);
                if (resultType != PolygonClipper.ResultType.SuccesfullyCliped)
                    return false;
                this.pointChain = result[0];
                //Changed our appearance, so add again.
                Debug.Log(result.Length);
                return true;
            }
            //Do some merge stuff
            return false;
        }

        public bool DoBoundsIntersect(Outline outline)
        {
            return Bounds.Intersects(outline.Bounds);
        }

        public bool DoesFullyEncompass(Outline outline)
        {
            return Bounds.Contains(outline.Bounds.min) && Bounds.Contains(outline.Bounds.max);
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
