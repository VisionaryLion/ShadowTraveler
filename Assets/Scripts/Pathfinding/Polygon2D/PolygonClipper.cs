using UnityEngine;
using Utility;
using System.Collections;
namespace Polygon2D
{
    public class PolygonClipper
    {
        public enum BoolOpType { INTERSECTION, UNION, DIFFERENCE, XOR };

        enum EdgeType { NORMAL, NON_CONTRIBUTING, SAME_TRANSITION, DIFFERENT_TRANSITION };
        enum PolygonType { SUBJECT, CLIPPING };

        public static Polygon Compute(ref Polygon sp, ref Polygon cp, BoolOpType op)
        {
            //Trivial case: At least one polygon is empty
            if (sp.IsEmpty || cp.IsEmpty)
            {
                if (op == BoolOpType.DIFFERENCE)
                    return sp;
                if (op == BoolOpType.UNION)
                    return (sp.IsEmpty) ? cp : sp;
                //Return null for INTERSECTION and XOR operations;
                return new Polygon();
            }

            //Trivial case: The polygons cannot intersect each other.
            if (!sp.Bounds.Intersects(cp.Bounds))
            {
                if (op == BoolOpType.DIFFERENCE)
                    return sp;
                if (op == BoolOpType.UNION)
                {
                    Polygon result = new Polygon(sp);
                    result.AddContours(cp);
                    return result;
                }
                //Return null for INTERSECTION and XOR operations;
                return new Polygon();
            }
        }

        class SweepEvent
        {
            Vector2 p;           // point associated with the event
            bool left;         // is the point the left endpoint of the segment (p, other->p)?
            PolygonType pl;    // Polygon to which the associated segment belongs to
            SweepEvent other; // Event associated to the other endpoint of the segment
                              /**  Does the segment (p, other->p) represent an inside-outside transition in the polygon for a vertical ray from (p.x, -infinite) that crosses the segment? */
            bool inOut;
            EdgeType type;
            bool inside; // Only used in "left" events. Is the segment (p, other->p) inside the other polygon?
            int posS; // Only used in "left" events. Position of the event (line segment) in S

            /** Class constructor */
            public SweepEvent(Vector2 point, bool left, PolygonType pTyp, SweepEvent other, EdgeType t = EdgeType.NORMAL)
            {
                p = point;
                this.left = left;
                pl = pTyp;
                this.other = other;
                type = t;
            }

            /** Is the line segment (p, other->p) below point x */
            public bool IsBelow(Vector2 o) { return (left) ? ExtendedMathf.SignedArea(p, other.p, o) > 0 : ExtendedMathf.SignedArea(other.p, p, o) > 0; }
            /** Is the line segment (p, other->p) above point x */
            public bool IsAbove(Vector2 o) { return !IsBelow(p); }
        }
    }
}
