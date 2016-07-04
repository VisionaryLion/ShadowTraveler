using UnityEngine;
using System.Collections.Generic;

namespace NavMesh2D.Core
{
    public class OutlineTree
    {

        List<Contour> outlines; // root

        public OutlineTree()
        {
            outlines = new List<Contour>();
        }

        public void AddOutline(Contour outline)
        {
            bool isConsumed = false;
            for (int iOut = 0; iOut < outlines.Count; iOut++)
            {
                if (outlines[iOut].TryAddContour(outline))
                {
                    outline = outlines[iOut];
                    isConsumed = true;
                }
            }
            if (!isConsumed)
                outlines.Add(outline);
        }

        public void DrawDebugInfo()
        {
            for (int iOutline = 0; iOutline < outlines.Count; iOutline++)
            {
                outlines[iOutline].DrawForDebug();
            }
        }
    }
}
