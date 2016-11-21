#define DEBUG_SHOW_GIZMOS

using UnityEngine;
using System.Collections;
using System;

namespace LightSensing
{
    public class CircleLightMarker : LightMarker
    {
        [SerializeField]
        float radius;
        [SerializeField]
        Vector2 centerOffset;

        [SerializeField]
        Color centerColor;
        [SerializeField]
        Color edgeColor;
        [SerializeField]
        bool squaredInterpolation;

        float sqrRadius;

        Bounds chachedBounds;

        public override Bounds Bounds
        {
            get
            {
                if (transform.hasChanged)
                {
                    chachedBounds = new Bounds(transform.position, new Vector3(radius * transform.lossyScale.x, radius * transform.lossyScale.y));
                    transform.hasChanged = false;
                }
                return chachedBounds;
            }
        }

        public override bool IsPointInsideMarker(Vector2 pos)
        {
            pos = transform.InverseTransformPoint(pos);
            float dx = Mathf.Abs(pos.x - centerOffset.x);
            float dy = Mathf.Abs(pos.y - centerOffset.y);

            if (dx > radius)
                return false;
            if (dy > radius)
                return false;
            if (dx + dy <= radius)
                return true;
            if (dx * dx + dy * dy <= sqrRadius)
                return true;
            return false;
        }

        public override Color SampleColorAt(Vector2 pos)
        {
            pos = transform.InverseTransformPoint(pos);
            float dist = (pos - centerOffset).magnitude;
            return SampleColorAt(dist);
        }

        public Color SampleColorAt(float distanceFromCenter)
        {
            distanceFromCenter /= radius;
            if (squaredInterpolation)
            {
                distanceFromCenter = Mathf.Sqrt(distanceFromCenter);
            }
            return Color.Lerp(centerColor, edgeColor, distanceFromCenter);
        }

        const float LineCircle_FudgeFactor = 0.00001f;
        public override bool IsTraversable(LightSkin skin, Vector2 pointA, Vector2 pointB, out float traverseCostsMulitplier)
        {
            traverseCostsMulitplier = 1;
            if (!Utility.ExtendedGeometry.DoesLineIntersectBounds(pointA, pointB, Bounds))
                return true;

            pointA = transform.InverseTransformPoint(pointA);
            pointB = transform.InverseTransformPoint(pointB);

            Vector2 dir = (pointB - pointA).normalized;
            float t = dir.x * (centerOffset.x - pointA.x) + dir.y * (centerOffset.y - pointA.y);
            Vector2 tangent = t * dir + pointA;
            float distToCenter = (tangent - centerOffset).magnitude;

            if (distToCenter <= radius)
            {
                return skin.IsTraverable(SampleColorAt(distToCenter), out traverseCostsMulitplier);
            }
            else
            {
                return true;
            }
        }

        void Awake()
        {
            radius = Mathf.Max(0.001f, radius);
            sqrRadius = radius * radius;
            transform.hasChanged = true;
        }

#if DEBUG_SHOW_GIZMOS
        void OnDrawGizmos()
        {
            Awake();
            if (!squaredInterpolation)
            {
                float stepSize = 0.5f / radius;
                for (float iRad = 0; iRad <= 1; iRad += stepSize)
                {
                    DebugExtension.DrawCircle(centerOffset, Vector3.forward, Color.Lerp(centerColor, edgeColor, iRad), transform.localToWorldMatrix, iRad * radius);
                }
            }
            else
            {
                float stepSize = 0.5f / radius;
                for (float iRad = 0; iRad <= 1; iRad += stepSize)
                {
                    DebugExtension.DrawCircle(centerOffset, Vector3.forward, Color.Lerp(centerColor, edgeColor, Mathf.Sqrt(iRad)), transform.localToWorldMatrix, iRad * radius);
                }
            }
            DebugExtension.DrawCircle(centerOffset, Vector3.forward, edgeColor, transform.localToWorldMatrix, radius);
            DebugExtension.DrawPoint(transform.TransformPoint(centerOffset), centerColor);
        }
#endif
    }
}
