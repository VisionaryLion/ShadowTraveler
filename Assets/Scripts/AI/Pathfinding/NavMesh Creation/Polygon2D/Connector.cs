using UnityEngine;
using System.Collections.Generic;

namespace Polygon2D
{
    public class Connector
    {
        List<PointChain> openChains;
        List<PointChain> closedChains;

        public Connector(int capacity)
        {
            openChains = new List<PointChain>(capacity);
            closedChains = new List<PointChain>(capacity);
        }

        public void Add(Vector2 p0, Vector2 p1)
        {
            for (int i = 0; i < openChains.Count; i++)
            {
                PointChain c = openChains[i];
                if (c.LinkSegment(ref p0, ref p1))
                {
                    if (c.IsClosed)
                    {
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

        public PointChain[] ToArray()
        {
            return closedChains.ToArray();
        }
    }
}
