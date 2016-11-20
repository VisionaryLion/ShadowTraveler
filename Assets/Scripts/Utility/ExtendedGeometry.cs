using UnityEngine;
using System.Collections;

namespace Utility
{
    public class ExtendedGeometry
    {

        public static float SignedArea(Vector2 a, Vector2 b, Vector2 c)
        {
            return (a.x - c.x) * (b.y - c.y) - (b.x - c.x) * (a.y - c.y);
        }

        /** Signed area of the triangle ( (0,0), p1, p2) */
        public static float SignedArea(Vector2 b, Vector2 c)
        {
            return -c.x * (b.y - c.y) - -c.y * (b.x - c.x);
        }

        /** Sign of triangle (p1, p2, o) */
        public static int Sign(Vector2 a, Vector2 b, Vector2 o)
        {
            float det = (a.x - o.x) * (b.y - o.y) - (b.x - o.x) * (a.y - o.y);
            return (det < 0 ? -1 : (det > 0 ? +1 : 0));
        }

        public static float SignedAreaDoubledTris(Vector2 a, Vector2 b, Vector2 c)
        {
            return (a.x - c.x) * (b.y - c.y) - (b.x - c.x) * (a.y - c.y);
        }

        /** Signed area of the triangle ( (0,0), p1, p2) */
        public static float SignedAreaDoubledTris(Vector2 b, Vector2 c)
        {
            return -c.x * (b.y - c.y) - -c.y * (b.x - c.x);
        }

        /** Sign of triangle (p1, p2, o) */
        public static int SignTris(Vector2 a, Vector2 b, Vector2 o)
        {
            float det = (a.x - o.x) * (b.y - o.y) - (b.x - o.x) * (a.y - o.y);
            return (det < 0 ? -1 : (det > 0 ? +1 : 0));
        }

        public static float SignedAreaDoubledRect(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            return (a.y - c.y) * (d.x - b.x) + (b.y - d.y) * (a.x - c.x);
        }

        public static bool DoesLineIntersectBounds(Vector2 pA, Vector2 pB, Bounds bounds)
        {
            if (pA.x > bounds.max.x && pB.x > bounds.max.x) return false;
            if (pA.x < bounds.min.x && pB.x < bounds.min.x) return false;
            if (pA.y > bounds.max.y && pB.y > bounds.max.y) return false;
            if (pA.y < bounds.min.y && pB.y < bounds.min.y) return false;

            float z = pB.x * pA.y - pA.x * pB.y;
            float x = pB.y - pA.y;
            float y = pA.x - pB.x;

            float sign = Mathf.Sign(bounds.max.x * x + bounds.max.y * y + z);
            return (sign == Mathf.Sign(bounds.min.x * x + bounds.max.y * y + z) && sign == Mathf.Sign(bounds.max.x * x + bounds.max.y * y + z) && sign == Mathf.Sign(bounds.max.x * x + bounds.max.y * y + z));
        }

        
        public static bool IsOnLeftSideOfLine(Vector2 pA, Vector2 pB, Vector2 point)
        {
            return ((pB.x - pA.x) * (point.y - pA.y) - (pB.y - pA.y) * (point.x - pA.x)) > 0;
        }
    }
}
