using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/*
Author: Oribow
*/
namespace SurfaceTypeUser
{
    [Serializable]
    public class Decal
    {
        [SerializeField]
        public Sprite sprite;
        [SerializeField]
        Vector2 scale;

        public Vector3 Scale
        {
            get
            {
                return new Vector3(scale.x, scale.y, 1);
            }
        }

        public Vector3 GetScale(Matrix4x4 worldToLocal)
        {
            Vector3 size = scale;
            Vector3 min = -size / 2f;
            Vector3 max = size / 2f;

            Vector3[] vts = new Vector3[] {
            new Vector3(min.x, min.y, min.z),
            new Vector3(max.x, min.y, min.z),
            new Vector3(min.x, max.y, min.z),
            new Vector3(max.x, max.y, min.z),

            new Vector3(min.x, min.y, max.z),
            new Vector3(max.x, min.y, max.z),
            new Vector3(min.x, max.y, max.z),
            new Vector3(max.x, max.y, max.z),
            };

            for (int i = 0; i < 8; i++)
            {
                vts[i] = worldToLocal.MultiplyPoint(vts[i]);
            }

            min = max = vts[0];
            foreach (Vector3 v in vts)
            {
                min = Vector3.Min(min, v);
                max = Vector3.Max(max, v);
            }
            return max - min;
        }
    }
}
