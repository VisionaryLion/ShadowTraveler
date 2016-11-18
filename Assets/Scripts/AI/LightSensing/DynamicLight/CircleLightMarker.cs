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
            float dist;
            dist = (pos - centerOffset).magnitude;
            dist /= radius;
            if (squaredInterpolation)
            {
                dist = Mathf.Sqrt(dist);
            }
            return Color.Lerp(centerColor, edgeColor, dist);
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
