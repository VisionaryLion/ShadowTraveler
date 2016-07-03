using UnityEngine;
using System.Collections.Generic;

namespace NavMesh2D.Core
{
    public class OutlineTree
    {

        List<Outline> outlines; // root

        public OutlineTree()
        {
            outlines = new List<Outline>();
        }

        public void AddOutline(Outline outline)
        {
            for (int iOut = 0; iOut < outlines.Count; iOut++)
            {
                if (outlines[iOut].TryAddOutline(outline, this))
                    return;
            }

            outlines.Add(outline);
        }

        public void ReAddOutline(Outline outline)
        {
            for (int iOut = 0; iOut < outlines.Count; iOut++)
            {
                if (outlines[iOut] == outline)
                    continue;
                if (outlines[iOut].TryMergeOutline(outline))
                {
                    Outline merged = outlines[iOut];
                    outlines.Remove(outline);
                    ReAddOutline(merged);
                    return;
                }
            }
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
