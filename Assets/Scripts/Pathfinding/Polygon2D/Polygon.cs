using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

namespace Polygon2D
{
    public class Polygon : IEnumerable<Contour>
    {
        public Bounds Bounds { get { if (!areBoundsValid) CalcBounds(); return bounds; } }
        public int ContourCount { get { return contours.Count; } }
        public bool IsEmpty { get { return contours.Count == 0; } }
        public int TotalVertexCount { get { return totalVertexCount; } }

        private bool areBoundsValid;
        private Bounds bounds;
        private List<Contour> contours;
        private int totalVertexCount;

        public Polygon()
        {
            contours = new List<Contour>(1);
        }

        public Polygon(Polygon other)
        {
            bounds = other.Bounds;
            areBoundsValid = other.areBoundsValid;
            contours = other.contours;
            totalVertexCount = other.totalVertexCount;
        }

        public void AddContour(IEnumerable<Contour> range)
        {
            foreach (Contour c in range)
            {
                AddContour(c);
            }
        }

        public void AddContour(params Contour[] range)
        {
            foreach (Contour c in range)
            {
                AddContour(c);
            }
        }

        private void AddContour(Contour c)
        {
            if (c.VertexCount == 0)// A contour must always hold at least one vertex.
                return;
            contours.Add(c);
            //Recalculate the bounds
            for (int iVert = 0; iVert < c.verticies.Count; iVert++)
            {
                bounds.max = Vector2.Max(bounds.max, c.verticies[iVert]);
                bounds.min = Vector2.Min(bounds.min, c.verticies[iVert]);
            }
            //Updated vertex count
            totalVertexCount += c.VertexCount;
        }

        public void RemoveContourAt(int pos)
        {
            if (pos < 0 || pos >= contours.Count)// A contour must always hold at least one vertex.
                return;
            //Updated vertex count
            totalVertexCount -= contours[pos].VertexCount;

            contours.RemoveAt(pos);
            areBoundsValid = false;

        }

        public void RemoveVertexFromContourAt(int iCont, int iVert)
        {
            if (iCont < 0 || iCont >= contours.Count)// A contour must always hold at least one vertex.
                return;
            Contour c = contours[iCont];
            if (c.VertexCount == 1)
                contours.RemoveAt(iCont);
            else
                c.RemoveVertexAt(iVert);
            areBoundsValid = false;

            //Updated vertex count
            totalVertexCount--;
        }

        public Contour this[int key]
        {
            get { return contours[key]; }
        }

        public IEnumerator<Contour> GetEnumerator()
        {
            return contours.GetEnumerator();
        }

        public void CleanContours()
        {
            for (int i = 0; i < contours.Count; i++)
            {
                Contour c = contours[i];
                totalVertexCount -= c.VertexCount;
                c.RemoveAllPointEdges();
                if (c.VertexCount == 0)
                {
                    contours.RemoveAt(i);
                    i--;
                }
                totalVertexCount += c.VertexCount;
            }
        }

        private void CalcBounds()
        {
            areBoundsValid = true;

            bounds.min = contours[0].verticies[0];
            bounds.max = contours[0].verticies[0];
            for (int iCo = 0; iCo < contours.Count; iCo++)
            {
                for (int iVert = 0; iVert < contours[iCo].verticies.Count; iVert++)
                {
                    bounds.max = Vector2.Max(bounds.max, contours[iCo].verticies[iVert]);
                    bounds.min = Vector2.Min(bounds.min, contours[iCo].verticies[iVert]);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return contours.GetEnumerator();
        }
    }

    public struct Contour : IEnumerable<Vector2>
    {
        public int VertexCount { get { return verticies.Count; } }

        public List<Vector2> verticies;

        public Contour(params Vector2[] verticies)
        {
            this.verticies = new List<Vector2>(verticies);
        }

        public Contour(List<Vector2> verticies)
        {
            this.verticies = new List<Vector2>();
            this.verticies.AddRange(verticies);
        }

        public void InsertVertex(Vector2 v)
        {
            verticies.Add(v);
        }

        public void RemoveVertexAt(int pos)
        {
            if (pos < 0 || pos >= verticies.Count || verticies.Count == 1)// The remove of the last vertex is not allowed!
                return;
            verticies.RemoveAt(pos);
        }

        public Vector2 this[int key]
        {
            get { return verticies[key]; }
        }

        public void RemoveAllPointEdges()
        {
            //Removes: edges with length = 0
            for (int i = 0; i < verticies.Count - 1; i++)
            {
                if (verticies[i] == verticies[i + 1])
                {
                    verticies.RemoveAt(i);
                    i--;
                }
            }
            if (verticies[0] == verticies[verticies.Count - 1])
                verticies.RemoveAt(verticies.Count - 1);
        }

        public IEnumerator<Vector2> GetEnumerator()
        {
            return ((IEnumerable<Vector2>)verticies).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Vector2>)verticies).GetEnumerator();
        }
    }
}
