using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NavMesh2D.Core
{
    [Serializable]
    public class Contour : IEnumerable<Vector2d>
    {
        public int VertexCount { get { return verticies.Count; } }
        public Boundsd Bounds { get { if (!areBoundsValid) CalcBounds(); return bounds; } }
        public bool IsEmpty { get { return verticies.Count == 0; } }
        public List<Vector2d> verticies;

        [SerializeField]
        private Boundsd bounds;
        [SerializeField]
        private bool areBoundsValid;

        public Contour(params Vector2d[] verticies)
        {
            this.verticies = new List<Vector2d>(verticies);
            CalcBounds();
        }

        public Contour(List<Vector2d> verticies)
        {
            this.verticies = new List<Vector2d>();
            this.verticies.AddRange(verticies);
            CalcBounds();
        }

        public void AddVertex(Vector2d v)
        {
            verticies.Add(v);
            bounds.max = Vector2d.Max(bounds.max, v);
            bounds.min = Vector2d.Min(bounds.min, v);
        }

        public void RemoveVertexAt(int pos)
        {
            if (pos < 0 || pos >= verticies.Count || verticies.Count == 3)// The remove of the last vertex is not allowed!
                throw new Exception("Tried to remove vertex failed. Remove index = " + pos + ", VertexCount = " + verticies.Count);
            verticies.RemoveAt(pos);
            areBoundsValid = false;
        }

        public Vector2d this[int key]
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

        public void Optimize(float nodeMergeDist, float maxEdgeDeviation)
        {
            Vector2d prevVert = verticies[verticies.Count - 1];
            Vector2d prevPrevVert = verticies[verticies.Count - 2];
            double srqMergeDist = nodeMergeDist * nodeMergeDist;
            for (int i = 0; i < verticies.Count && verticies.Count > 3; i++)
            {
                if ((prevVert - verticies[i]).sqrMagnitude <= srqMergeDist)
                {
                    //Stage for remove
                    RemoveVertexAt(i);
                    i--;
                }
                else
                {
                    Vector2d nA = prevPrevVert - prevVert;
                    Vector2d nB = verticies[i] - prevVert;
                    double angle = Vector2d.Angle(nA, nB);

                    if (angle >= 180 - maxEdgeDeviation)
                    {
                        //Stage for remove
                        RemoveVertexAt((verticies.Count + (i - 1)) % verticies.Count);
                        if (i > 0)
                            i--;
                        prevVert = verticies[i];
                    }
                    else
                    {
                        prevPrevVert = prevVert;
                        prevVert = verticies[i];
                    }
                }
            }
        }

        public bool IsSolid()
        {
            return CalcArea() >= 0;
        }

        public double CalcArea()
        {
            double area = 0;
            int j = verticies.Count - 1;

            for (int i = 0; i < verticies.Count; i++)
            {
                area = area + (verticies[j].x + verticies[i].x) * (verticies[j].y - verticies[i].y);
                j = i;
            }
            return area / 2 * -1;
        }

        public void VisualDebug(Color color)
        {
            Vector2d prev = verticies[verticies.Count - 1];
            foreach (Vector2d vert in verticies)
            {
                Debug.DrawLine((Vector2)prev, (Vector2)vert, color);
                prev = vert;
            }
        }

        public IEnumerator<Vector2d> GetEnumerator()
        {
            return ((IEnumerable<Vector2d>)verticies).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Vector2d>)verticies).GetEnumerator();
        }

        private void CalcBounds()
        {
            areBoundsValid = true;
            bounds.min = verticies[0];
            bounds.max = verticies[0];
            for (int iVert = 1; iVert < verticies.Count; iVert++)
            {
                bounds.max = Vector2d.Max(bounds.max, verticies[iVert]);
                bounds.min = Vector2d.Min(bounds.min, verticies[iVert]);
            }
        }
    }
}
