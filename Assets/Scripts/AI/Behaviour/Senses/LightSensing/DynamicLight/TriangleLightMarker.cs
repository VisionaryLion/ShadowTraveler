#define DEBUG_SHOW_GIZMOS

using UnityEngine;
using System.Collections;
using System;

namespace LightSensing
{
    public class TriangleLightMarker : LightMarker
    {
        [SerializeField]
        float width;
        [SerializeField]
        float height;
        [SerializeField]
        Vector2 centerOffset;

        [SerializeField]
        Color colorA;
        [SerializeField]
        Color colorB;
        [SerializeField]
        bool squaredInterpolation;

        float widthHalfed;
        float heightHalfed;
        float widthPerHeight;
        Bounds chachedBounds;

        public override Bounds Bounds
        {
            get
            {
                if (transform.hasChanged)
                {
                    Vector2 pointA = transform.TransformPoint(new Vector2(centerOffset.x, centerOffset.y - heightHalfed));
                    Vector2 pointB = transform.TransformPoint(new Vector2(centerOffset.x + widthHalfed, centerOffset.y + heightHalfed));
                    Vector2 pointC = transform.TransformPoint(new Vector2(centerOffset.x - widthHalfed, centerOffset.y + heightHalfed));
                    chachedBounds = new Bounds(pointA, Vector3.zero);
                    chachedBounds.Encapsulate(pointB);
                    chachedBounds.Encapsulate(pointC);
                    transform.hasChanged = false;
                }
                return chachedBounds;
            }
        }

        public override Vector2 Center
        {
            get
            {
                return transform.TransformPoint(centerOffset);
            }
        }

        public override bool IsPointInsideMarker(Vector2 pos)
        {
            return IsPointInsideMarkerLocal(transform.InverseTransformPoint(pos));
        }

        public bool IsPointInsideMarkerLocal(Vector2 pos)
        {
            pos -= centerOffset;
            float y = (pos.y + heightHalfed);
            return pos.x >= -widthPerHeight * y && pos.x <= widthPerHeight * y && pos.y >= -heightHalfed && pos.y <= heightHalfed;
        }

        public override Color SampleColorAt(Vector2 pos)
        {
            pos = transform.InverseTransformPoint(pos);
            return SampleColorAt((pos.y - (centerOffset.y - heightHalfed)) / height);
        }

        public Color SampleColorAt(float distToSource)
        {
            if (squaredInterpolation)
                Mathf.Sqrt(distToSource);
            return Color.Lerp(colorA, colorB, distToSource);
        }

        public override bool IsTraversable(LightSkin skin, Vector2 pointA, Vector2 pointB, out float traverseCostsMulitplier)
        {
            pointA = transform.InverseTransformPoint(pointA);
            pointB = transform.InverseTransformPoint(pointB);
            traverseCostsMulitplier = 1;

            if (!Utility.ExtendedGeometry.DoesLineIntersectBounds(pointA, pointB, Bounds))
                return true;

            Vector2 triA = new Vector2(centerOffset.x, centerOffset.y - heightHalfed);
            Vector2 triB = new Vector2(centerOffset.x + widthHalfed, centerOffset.y + heightHalfed);
            Vector2 triC = new Vector2(centerOffset.x - widthHalfed, centerOffset.y + heightHalfed);

            Vector2 inter;
            float maxY = 0;

            if (IsPointInsideMarkerLocal(pointA))
            {
                maxY = (pointA - centerOffset).y + heightHalfed;
            }
            if (IsPointInsideMarkerLocal(pointB))
            {
                if (maxY != 0)
                {
                    maxY = Mathf.Max((pointB - centerOffset).y + heightHalfed, maxY);
                    return skin.IsTraversable(SampleColorAt(maxY), out traverseCostsMulitplier);
                }
            }
            if (Utility.ExtendedGeometry.FindLineIntersection(pointA, pointB, triA, triB, out inter))
            {
                maxY = Mathf.Max((inter - centerOffset).y + heightHalfed, maxY);
            }
            if (Utility.ExtendedGeometry.FindLineIntersection(pointA, pointB, triA, triC, out inter))
            {
                maxY = Mathf.Max((inter - centerOffset).y + heightHalfed, maxY);
            }
            if (maxY == 0 && Utility.ExtendedGeometry.FindLineIntersection(pointA, pointB, triB, triC, out inter))
            {
                maxY = (inter - centerOffset).y + heightHalfed;
            }
            if (maxY == 0)
            {
                return true;
            }

            return skin.IsTraversable(SampleColorAt(maxY), out traverseCostsMulitplier);
        }

        void Awake()
        {
            widthHalfed = width / 2;
            heightHalfed = height / 2;
            widthPerHeight = widthHalfed / height;
            transform.hasChanged = true;
        }

#if DEBUG_SHOW_GIZMOS
        void OnDrawGizmos()
        {
            Awake();
            float stepSize = 0.5f / height;
            Vector2 from;
            Vector2 to;

            for (float iStep = 0; iStep <= 1; iStep += stepSize)
            {
                float y = (centerOffset.y - heightHalfed) + iStep * height;
                if (squaredInterpolation)
                {
                    Gizmos.color = Color.Lerp(colorA, colorB, Mathf.Sqrt(iStep));
                }
                else
                {
                    Gizmos.color = Color.Lerp(colorA, colorB, iStep);
                }
                from = transform.TransformPoint(new Vector3(centerOffset.x - widthPerHeight * iStep * height, y, 0));
                to = transform.TransformPoint(new Vector3(centerOffset.x + widthPerHeight * iStep * height, y, 0));
                Gizmos.DrawLine(from, to);
            }
            Gizmos.color = colorB;
            from = transform.TransformPoint(centerOffset + new Vector2(-widthHalfed, heightHalfed));
            to = transform.TransformPoint(centerOffset + new Vector2(widthHalfed, heightHalfed));
            Gizmos.DrawLine(from, to);
        }
#endif
    }
}
