﻿using UnityEngine;
using System.Collections.Generic;
using Utility.ExtensionMethods;
using NavMesh2D.Core;

namespace Utility.Polygon2D
{
    internal class Connector
    {
        List<PointChain> openChains;
        List<PointChain> closedChains;

        public Connector(int capacity)
        {
            openChains = new List<PointChain>(capacity);
            closedChains = new List<PointChain>(capacity);
        }

        public void Add(Vector2d p0, Vector2d p1)
        {
            VectorHistoryDrawer.EnqueueNewLines(0, p0, p1);
            for (int i = 0; i < openChains.Count; i++)
            {
                PointChain c = openChains[i];
                if (c.LinkSegment(ref p0, ref p1))
                {
                    if (c.IsClosed)
                    {
                        //Debug.Log("Closed a chain. "+VectorHistoryDrawer.instance.time);
                        closedChains.Add(c);
                        openChains.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        for (int k = i + 1; k < openChains.Count; k++)
                        {
                            if (openChains[k].LinkPointChain(c))
                            {
                                openChains.RemoveAt(i);
                                i--;
                                break;
                            }
                        }
                    }
                    return;
                }
            }

            openChains.Add(new PointChain(ref p0, ref p1));
        }

        public Contour[] ToArray()
        {
            Contour[] result = new Contour[closedChains.Count];
            for (int iChain = 0; iChain < result.Length; iChain++)
            {
                result[iChain] = new Contour(closedChains[iChain].chain.ToArray());
            }
            return result;
        }
    }
}
