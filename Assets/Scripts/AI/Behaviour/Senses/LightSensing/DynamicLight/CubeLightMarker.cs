#define DEBUG_SHOW_GIZMOS

using UnityEngine;
using System.Collections;
using System;

namespace LightSensing
{
    public class CubeLightMarker : LightMarker
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
        Bounds chachedBounds;

        public override Bounds Bounds
        {
            get
            {
                if (transform.hasChanged)
                {
                    Vector2 pointA = transform.TransformPoint(new Vector2(centerOffset.x - widthHalfed, centerOffset.y - heightHalfed));
                    Vector2 pointB = transform.TransformPoint(new Vector2(centerOffset.x + widthHalfed, centerOffset.y - heightHalfed));
                    Vector2 pointC = transform.TransformPoint(new Vector2(centerOffset.x + widthHalfed, centerOffset.y + heightHalfed));
                    Vector2 pointD = transform.TransformPoint(new Vector2(centerOffset.x - widthHalfed, centerOffset.y + heightHalfed));
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
            return pos.x >= centerOffset.x - widthHalfed && pos.x <= centerOffset.x + widthHalfed && pos.y >= centerOffset.y - heightHalfed && pos.y <= centerOffset.y + heightHalfed;
        }

        public override Color SampleColorAt(Vector2 pos)
        {
            pos = transform.InverseTransformPoint(pos);
            return SampleColorAt(pos.y - (centerOffset.y - heightHalfed));
        }

        public Color SampleColorAt(float distanceFromSource)
        {
            distanceFromSource /= height;
            if (squaredInterpolation)
                Mathf.Sqrt(distanceFromSource);
            return Color.Lerp(colorA, colorB, distanceFromSource);
        }

        public override bool IsTraversable(LightSkin skin, Vector2 pointA, Vector2 pointB, out float traverseCostsMulitplier)
        {
            pointA = transform.InverseTransformPoint(pointA);
            pointB = transform.InverseTransformPoint(pointB);
            traverseCostsMulitplier = 1;

            if (!Utility.ExtendedGeometry.DoesLineIntersectBounds(pointA, pointB, Bounds))
                return true;

            float maxY = Mathf.Max(pointA.y, pointB.y) - (centerOffset.y - heightHalfed);
            return skin.IsTraversable(SampleColorAt(maxY), out traverseCostsMulitplier);
        }

        void Awake()
        {
            widthHalfed = width / 2;
            heightHalfed = height / 2;
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
                from = transform.TransformPoint(new Vector3(centerOffset.x - widthHalfed, y, 0));
                to = transform.TransformPoint(new Vector3(centerOffset.x + widthHalfed, y, 0));
                Gizmos.DrawLine(from, to);
            }
            from = transform.TransformPoint(new Vector3(centerOffset.x - widthHalfed, centerOffset.y + heightHalfed, 0));
            to = transform.TransformPoint(new Vector3(centerOffset.x + widthHalfed, centerOffset.y + heightHalfed, 0));
            Gizmos.DrawLine(from, to);
        }
#endif
    }
}
