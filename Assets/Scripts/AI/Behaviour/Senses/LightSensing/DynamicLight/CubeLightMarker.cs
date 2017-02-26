#define DEBUG_SHOW_GIZMOS

using UnityEngine;
using System.Collections;
using System;

namespace LightSensing
{
    public class CubeLightMarker : LightMarker
    {
        [SerializeField, Range(0.1f, 100)]
        float width;
        [SerializeField, Range(0.1f, 100)]
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
        Bounds chachedBounds;

        public override Bounds Bounds
        {
            get
            {
                if (transform.hasChanged)
                {
                    Vector2 pointA = transform.TransformPoint(new Vector2(centerOffset.x - widthHalfed, centerOffset.y));
                    Vector2 pointB = transform.TransformPoint(new Vector2(centerOffset.x + widthHalfed, centerOffset.y));
                    Vector2 pointC = transform.TransformPoint(new Vector2(centerOffset.x + widthHalfed, centerOffset.y + height));
                    Vector2 pointD = transform.TransformPoint(new Vector2(centerOffset.x - widthHalfed, centerOffset.y + height));
                    chachedBounds = new Bounds(pointA, Vector3.zero);
                    chachedBounds.Encapsulate(pointB);
                    chachedBounds.Encapsulate(pointC);
                    chachedBounds.Encapsulate(pointD);
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
            pos = transform.InverseTransformPoint(pos);
            return pos.x >= centerOffset.x - widthHalfed && pos.x <= centerOffset.x + widthHalfed && pos.y >= centerOffset.y && pos.y <= centerOffset.y + height;
        }

        public override bool IsTraversable(LightSkin skin, out float traverseCostsMulitplier)
        {
            return skin.IsTraversable(pathfindingColor, out traverseCostsMulitplier);
        }

        public override bool OverlapsSegment(Vector2 segmentA, Vector2 segmentB)
        {
            segmentA = transform.InverseTransformPoint(segmentA);
            segmentB = transform.InverseTransformPoint(segmentB);

            return Utility.ExtendedGeometry.DoesLineIntersectBounds(segmentA, segmentB, Bounds);
        }

        public override Color SampleLightAt(Vector2 pos)
        {
            pos = transform.InverseTransformPoint(pos);
            return SampleColorAt(pos.y - centerOffset.y);
        }

        private Color SampleColorAt(float distanceFromSource)
        {
            distanceFromSource /= height;
            if (squaredInterpolation)
                Mathf.Sqrt(distanceFromSource);
            return Color.Lerp(colorA, colorB, distanceFromSource);
        }

        void Awake()
        {
            widthHalfed = width / 2;
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
                float y = (centerOffset.y) + iStep * height;
                if (squaredInterpolation)
                {
                    Gizmos.color = Color.Lerp(colorA, colorB, Mathf.Sqrt(iStep)); ;
                }
                else
                {
                    Gizmos.color = Color.Lerp(colorA, colorB, iStep);
                }
                from = transform.TransformPoint(new Vector3(centerOffset.x - widthHalfed, y, 0));
                to = transform.TransformPoint(new Vector3(centerOffset.x + widthHalfed, y, 0));
                Gizmos.DrawLine(from, to);
            }
            from = transform.TransformPoint(new Vector3(centerOffset.x - widthHalfed, centerOffset.y + height, 0));
            to = transform.TransformPoint(new Vector3(centerOffset.x + widthHalfed, centerOffset.y + height, 0));
            Gizmos.DrawLine(from, to);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(centerOffset, 0.1f);
        }
#endif
    }
}
