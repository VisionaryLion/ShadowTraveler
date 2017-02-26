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
        Color pathfindingColor;
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

        public override bool IsTraversable(LightSkin skin, out float traverseCostsMulitplier)
        {
            return skin.IsTraversable(pathfindingColor, out traverseCostsMulitplier);
        }

        public override bool OverlapsSegment(Vector2 segmentA, Vector2 segmentB)
        {
            segmentA = transform.InverseTransformPoint(segmentA);
            segmentB = transform.InverseTransformPoint(segmentB);

            if (!Utility.ExtendedGeometry.DoesLineIntersectBounds(segmentA, segmentB, Bounds))
                return true;

            Vector2 triA = new Vector2(centerOffset.x, centerOffset.y - heightHalfed);
            Vector2 triB = new Vector2(centerOffset.x + widthHalfed, centerOffset.y + heightHalfed);
            Vector2 triC = new Vector2(centerOffset.x - widthHalfed, centerOffset.y + heightHalfed);

            Vector2 inter;
            float maxY = 0;
            if (Utility.ExtendedGeometry.FindLineIntersection(segmentA, segmentB, triA, triB, out inter))
            {
                return true;
            }
            if (Utility.ExtendedGeometry.FindLineIntersection(segmentA, segmentB, triA, triC, out inter))
            {
                return true;
            }
            if (maxY == 0 && Utility.ExtendedGeometry.FindLineIntersection(segmentA, segmentB, triB, triC, out inter))
            {
                return true;
            }
            return false;
        }

        public override Color SampleLightAt(Vector2 pos)
        {
            pos = transform.InverseTransformPoint(pos);
            return SampleColorAt((pos.y - (centerOffset.y - heightHalfed)) / height);
        }

        private Color SampleColorAt(float distToSource)
        {
            if (squaredInterpolation)
                Mathf.Sqrt(distToSource);
            return Color.Lerp(colorA, colorB, distToSource);
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
                    Color.Lerp(colorA, colorB, Mathf.Sqrt(iStep));
                }
                else
                {
                    Color.Lerp(colorA, colorB, iStep);
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
