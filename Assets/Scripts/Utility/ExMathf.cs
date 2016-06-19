using UnityEngine;
using System.Collections;

namespace Utility
{
    public class ExMathf
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
    }
}
