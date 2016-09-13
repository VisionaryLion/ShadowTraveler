using UnityEngine;
using System.Collections;
using Utility;
using System.Collections.Generic;
using Utility.ExtensionMethods;

namespace NavMesh2D.Core
{
    public class WalkSpaceTester
    {
        /*enum EdgeType { NORMAL, NON_CONTRIBUTING, SAME_TRANSITION, DIFFERENT_TRANSITION };
        enum PolygonType { SUBJECT, CLIPPING };*/
        private const float fudgeFactor = 0.0001f;
        private const float maxVertDeviation = 0.01f;

        public static void MarkNotWalkableSegments(MarkableContour sp, List<ExpandedNode> cps, float minWalkableHeight, int markerFlag)
        {
            Bounds inflatedBounds;
            int segmentCount;
            Segment firstExSeg = InflateContour(sp, minWalkableHeight, out inflatedBounds, out segmentCount);
            Segment pSeg = firstExSeg;


            MarkSelfIntersections(firstExSeg, sp, markerFlag, minWalkableHeight);
            foreach (ExpandedNode cp in cps)
            {
                if (cp.contour == sp)
                    continue;
                if (cp.contour.bounds.Intersects(sp.bounds))
                 MarkIntersections(firstExSeg, cp.contour, markerFlag, minWalkableHeight);
            }
        }

        public static void MarkSelfIntersections(MarkableContour sp, float minWalkableHeight, int markerFlag)
        {
            Bounds inflatedBounds;
            int segmentCount;
            Segment firstExSeg = InflateContour(sp, minWalkableHeight, out inflatedBounds, out segmentCount);
            Segment pSeg = firstExSeg;
            MarkSelfIntersections(firstExSeg, sp, markerFlag, minWalkableHeight);
        }

        private static Segment InflateContour(MarkableContour mC, float minWalkableHeight, out Bounds inflatedBounds, out int segmentCount)
        {
            Segment firstExSeg = null, cExSeg, prevSeg;
            Vector2 normal, tangent;
            segmentCount = 0;

            //Calc lastC at the start. This operation will be done later too though, but its done now for code ease.
            tangent = (mC.firstPoint.pointC - mC.firstPoint.pointB).normalized;
            normal = new Vector2(tangent.y, -tangent.x) * minWalkableHeight;
            prevSeg = new Segment();
            prevSeg.b = mC.firstPoint.pointC + normal;
            firstExSeg = prevSeg;
            inflatedBounds = new Bounds(prevSeg.b, Vector2.zero);

            foreach (PointNode pn in mC)
            {
                tangent = (pn.pointC - pn.pointB).normalized;
                normal = new Vector2(tangent.y, -tangent.x) * minWalkableHeight;
                cExSeg = new Segment();
                cExSeg.src = pn;

                if (pn.angle >= Mathf.PI)
                {
                    cExSeg.a = pn.pointB + normal;
                    cExSeg.b = pn.pointC + normal;

                    Vector2[] aproxVerts = AproximateCircle(cExSeg.a, prevSeg.b, pn.pointB, minWalkableHeight, Mathf.PI * 2 - pn.angle);
                    for (int iAVert = 0; iAVert < aproxVerts.Length - 1; iAVert++)
                    {
                        Segment circleExSeg = new Segment();
                        circleExSeg.a = aproxVerts[iAVert];
                        circleExSeg.b = aproxVerts[iAVert + 1];
                        circleExSeg.isCorner = true;
                        circleExSeg.src = pn;
                        prevSeg.next = circleExSeg;
                        prevSeg = circleExSeg;
                        inflatedBounds.Encapsulate(aproxVerts[iAVert]);
                    }
                    segmentCount += aproxVerts.Length - 1;
                }
                else
                {
                    cExSeg.a = pn.pointB + normal;
                    cExSeg.b = pn.pointC + normal;
                }
                segmentCount++;
                inflatedBounds.Encapsulate(cExSeg.b);
                prevSeg.next = cExSeg;
                prevSeg = cExSeg;
            }
            return firstExSeg.next;
        }

        private static Vector2[] AproximateCircle(Vector2 a, Vector2 b, Vector2 m, float radius, float angle)
        {
            Vector2 nA = a - m;
            Vector2 nB = b - m;

            int S = Mathf.Max(Mathf.CeilToInt(angle / (Mathf.Acos(radius / (radius + maxVertDeviation)) * 2)), 1);
            float correctedL = (radius / Mathf.Cos(angle / (S * 2))) - radius;

            Vector2[] result = new Vector2[S + 2];

            float angleStep = angle / S;

            Vector2 cPoint = nA.RotateRad(angleStep / 2).normalized * (radius + correctedL);
            result[0] = a;
            result[1] = cPoint + m;
            result[S + 1] = b;
            for (int i = 2; i < S + 1; i++)
            {
                cPoint = cPoint.RotateRad(angleStep);
                result[i] = cPoint + m;
            }
            return result;
        }

        private static void MarkSelfIntersections(Segment firstExSeg, MarkableContour sp, int markerIndex, float minWalkableHeight)
        {
            Vector2 rectA, rectB, rectC, rectD;
            float doubledAreaOfRect, start = 0, end = 0, dot;
            bool prevWasInside;
            int isInOut;
            Vector2 intersection;
            PointNode pn = null;

            //Circle vars
            Bounds circleBounds;
            float radSquared = minWalkableHeight * minWalkableHeight;
            do
            {
                if (firstExSeg.isCorner)//circle
                {
                    rectA = firstExSeg.a;

                    while (firstExSeg.next.isCorner)
                    {
                        firstExSeg = firstExSeg.next;
                    }

                    rectC = firstExSeg.src.pointB;
                    rectB = firstExSeg.b - rectC;
                    rectA -= rectC;

                    circleBounds = new Bounds(rectC, new Vector3(minWalkableHeight * 2, minWalkableHeight * 2));
                   /* DebugExtension.DebugBounds(circleBounds, Color.yellow);
                    DebugExtension.DebugArrow(rectC, rectB, Color.green);
                    DebugExtension.DebugArrow(rectC, rectA, Color.blue);*/
                    pn = firstExSeg.next.src.Next;
                    while (true)
                    {
                        if (pn.pointC == rectC) // end reached
                        {
                            break;
                        }
                        if (circleBounds.Contains(pn.pointB))
                        {
                            if (IsPointInsideCircleSector(rectB, rectA, radSquared, pn.pointB - rectC))
                            {
                                //Intersection found!
                                /*DebugExtension.DebugArrow(rectC, rectA, Color.gray);
                                DebugExtension.DebugArrow(rectC, rectB, Color.gray);
                                DebugExtension.DebugPoint(pn.pointB, Color.cyan, 2);*/
                                //firstExSeg.next.src.MarkPointNotWalkable(markerIndex);
                                break;
                            }
                        }
                        
                        if (DoesLineIntersectBounds(pn.pointB, pn.pointC, circleBounds))
                        {
                            if (DoesLineIntersectCircleSector(pn.pointB, pn.pointC, radSquared, rectC, rectB, rectA))
                            {
                                DebugExtension.DebugArrow(rectC, pn.pointB - rectC, Color.gray);
                                DebugExtension.DebugArrow(rectC, pn.pointC - rectC, Color.gray);
                               // firstExSeg.next.src.MarkPointNotWalkable(markerIndex);
                                break;
                            }
                        }
                        pn = pn.Next;
                    }
                }
                else
                {
                    rectA = firstExSeg.a;
                    rectB = firstExSeg.b;
                    rectC = firstExSeg.src.pointC;
                    rectD = firstExSeg.src.pointB;
                    doubledAreaOfRect = Mathf.Abs(ExMathf.SignedAreaDoubledRect(rectA, rectB, rectC, rectD)) + fudgeFactor;
                    //if (ShouldDebugThis(firstExSeg, sp, markerIndex, minWalkableHeight))
                    //{
                    Debug.DrawLine(rectA, rectB, Color.cyan);
                     Debug.DrawLine(rectB, rectC, Color.cyan);
                    Debug.DrawLine(rectA, rectD, Color.cyan);
                    // }

                    prevWasInside = true;
                    end = firstExSeg.src.distanceBC;
                    start = firstExSeg.src.distanceBC;
                    pn = firstExSeg.src.Next.Next;

                    while (true)
                    {
                        if (pn == firstExSeg.src)
                        {
                            if (!prevWasInside)
                            {
                                if (HandlePossibleIntersection(rectA, rectB, pn.Previous, out intersection) || HandlePossibleIntersection(rectB, rectC, pn.Previous, out intersection) || HandlePossibleIntersection(rectA, rectD, pn.Previous, out intersection))
                                {
                                    //if (ShouldDebugThis(firstExSeg, sp, markerIndex, minWalkableHeight, pn))
                                    DebugExtension.DebugPoint(intersection);
                                    dot = Vector2.Dot(firstExSeg.src.tangentBC, intersection - rectA);
                                    end = dot;
                                }
                                else
                                    break;
                            }
                            start = 0;
                            firstExSeg.src.AddObstruction(start, end);
                            break;
                        }
                        if (IsPointInsideRect(doubledAreaOfRect, rectA, rectB, rectC, rectD, pn.pointB))
                        {
                            if (!prevWasInside)
                            {
                                if (HandlePossibleIntersection(rectA, rectB, pn.Previous, out intersection) || HandlePossibleIntersection(rectB, rectC, pn.Previous, out intersection) || HandlePossibleIntersection(rectA, rectD, pn.Previous, out intersection))
                                {
                                    //if (ShouldDebugThis(firstExSeg, sp, markerIndex, minWalkableHeight, pn))
                                    DebugExtension.DebugPoint(intersection, Color.yellow);
                                    dot = Vector2.Dot(firstExSeg.src.tangentBC, intersection - rectA);
                                    start = dot;
                                    end = dot;
                                }

                            }
                            //if (ShouldDebugThis(firstExSeg, sp, markerIndex, minWalkableHeight, pn))
                            DebugExtension.DebugPoint(pn.pointB, Color.black);
                            dot = Vector2.Dot(firstExSeg.src.tangentBC, pn.pointB - rectA);
                            start = Mathf.Min(dot, start);
                            end = Mathf.Max(dot, end);
                            prevWasInside = true;
                        }
                        else
                        {
                            isInOut = prevWasInside ? 1 : 0;
                            if (HandlePossibleIntersection(rectA, rectB, pn.Previous, out intersection))
                            {
                                //if (ShouldDebugThis(firstExSeg, sp, markerIndex, minWalkableHeight, pn))
                                DebugExtension.DebugPoint(intersection, Color.red);
                                dot = Vector2.Dot(firstExSeg.src.tangentBC, intersection - rectA);
                                if (isInOut == 0)
                                {
                                    start = dot;
                                    end = dot;
                                }
                                else
                                {
                                    start = Mathf.Min(dot, start);
                                    end = Mathf.Max(dot, end);
                                }
                                isInOut++;
                            }
                            if (HandlePossibleIntersection(rectB, rectC, pn.Previous, out intersection))
                            {
                                //if (ShouldDebugThis(firstExSeg, sp, markerIndex, minWalkableHeight, pn))
                               DebugExtension.DebugPoint(intersection, Color.red);
                                dot = firstExSeg.src.distanceBC;
                                if (isInOut == 0)
                                {
                                    start = dot;
                                    end = dot;
                                }
                                else
                                {
                                    start = Mathf.Min(dot, start);
                                    end = Mathf.Max(dot, end);
                                }
                                isInOut++;
                            }
                            if (HandlePossibleIntersection(rectA, rectD, pn.Previous, out intersection))
                            {
                                //if (ShouldDebugThis(firstExSeg, sp, markerIndex, minWalkableHeight, pn))
                                DebugExtension.DebugPoint(intersection, Color.red);
                                dot = 0;
                                if (isInOut == 0)
                                {
                                    start = dot;
                                    end = dot;
                                }
                                else
                                {
                                    start = Mathf.Min(dot, start);
                                    end = Mathf.Max(dot, end);
                                }
                                isInOut++;
                            }
                            if (prevWasInside)
                            {
                                if (isInOut >= 2)
                                    firstExSeg.src.AddObstruction(start, end);
                                else
                                {
                                    start = 0;
                                    end = 0;
                                }
                                prevWasInside = false;
                            }
                            else
                            {
                                if (isInOut == 1)
                                    prevWasInside = true;
                                else if (isInOut == 2)
                                    firstExSeg.src.AddObstruction(start, end);
                                else
                                {
                                    start = 0;
                                    end = 0;
                                }
                            }
                        }
                        pn = pn.Next;
                    }
                }
            } while ((firstExSeg = firstExSeg.next) != null);
        }

        private static void MarkIntersections(Segment firstExSeg, MarkableContour cp, int markerIndex, float minWalkableHeight)
        {
            Vector2 rectA, rectB, rectC, rectD;
            float doubledAreaOfRect, start = 0, end = 0, dot;
            bool prevWasInside;
            int isInOut;
            Vector2 intersection;

            //Circle vars
            Bounds circleBounds;
            float radSquared = minWalkableHeight * minWalkableHeight;

            DebugExtension.DebugCircle(cp.firstPoint.pointB, Vector3.forward * 2, Color.magenta);
            do
            {
                if (firstExSeg.isCorner)//circle
                {
                    if (!firstExSeg.src.IsPointWalkable(markerIndex))
                        continue;

                    rectA = firstExSeg.a;

                    while (firstExSeg.next.isCorner)
                    {
                        firstExSeg = firstExSeg.next;
                    }

                    rectC = firstExSeg.src.pointB;
                    rectB = firstExSeg.b - rectC;
                    rectA -= rectC;

                    circleBounds = new Bounds(rectC, new Vector3(minWalkableHeight * 2, minWalkableHeight * 2));
                    DebugExtension.DebugBounds(circleBounds, Color.red);
                    foreach (PointNode pn in cp)
                    {
                        if (circleBounds.Contains(pn.pointB))
                        {
                            if (IsPointInsideCircleSector(rectB, rectA, radSquared, pn.pointB - rectC))
                            {
                                //Intersection found!
                                //DebugExtension.DebugBounds(circleBounds, Color.yellow);
                                //DebugExtension.DebugArrow(rectC, rectA, Color.gray);
                                //DebugExtension.DebugArrow(rectC, rectB, Color.gray);
                                //DebugExtension.DebugPoint(pn.pointB, Color.cyan, 2);
                                //firstExSeg.next.src.MarkPointNotWalkable(markerIndex);
                                break;
                            }
                        }
                        if (DoesLineIntersectBounds(pn.pointB, pn.pointC, circleBounds))
                        {
                            if (DoesLineIntersectCircleSector(pn.pointB, pn.pointC, radSquared, rectC, rectB, rectA))
                            {
                                //firstExSeg.next.src.MarkPointNotWalkable(markerIndex);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    rectA = firstExSeg.a;
                    rectB = firstExSeg.b;
                    rectC = firstExSeg.src.pointC;
                    rectD = firstExSeg.src.pointB;
                    doubledAreaOfRect = Mathf.Abs(ExMathf.SignedAreaDoubledRect(rectA, rectB, rectC, rectD)) + fudgeFactor;
                    Debug.DrawLine(rectA, rectB, Color.cyan);
                    Debug.DrawLine(rectB, rectC, Color.cyan);
                    Debug.DrawLine(rectA, rectD, Color.cyan);
                    start = 5;
                    end = 50;
                    prevWasInside = false;//IsPointInsideRect(doubledAreaOfRect, rectA, rectB, rectC, rectD, cp.firstPoint.Previous.pointB);


                    foreach (PointNode pn in cp)
                    {
                        if (IsPointInsideRect(doubledAreaOfRect, rectA, rectB, rectC, rectD, pn.pointB))
                        {
                            if (!prevWasInside)
                            {
                                if (HandlePossibleIntersection(rectA, rectB, pn.Previous, out intersection) || HandlePossibleIntersection(rectB, rectC, pn.Previous, out intersection) || HandlePossibleIntersection(rectA, rectD, pn.Previous, out intersection))
                                {
                                    //DebugExtension.DebugPoint(intersection, Color.yellow);
                                    dot = Vector2.Dot(firstExSeg.src.tangentBC, intersection - rectA);
                                    start = dot;
                                    end = dot;
                                    //DebugExtension.DebugPoint(pn.pointB, Color.black);
                                    dot = Vector2.Dot(firstExSeg.src.tangentBC, pn.pointB - rectA);
                                    start = Mathf.Min(dot, start);
                                    end = Mathf.Max(dot, end);
                                }
                                else
                                {
                                    //DebugExtension.DebugPoint(pn.pointB, Color.black);
                                    dot = Vector2.Dot(firstExSeg.src.tangentBC, pn.pointB - rectA);
                                    start = dot;
                                    end = dot;
                                }
                            }
                            else
                            {
                                //DebugExtension.DebugPoint(pn.pointB, Color.black);
                                dot = Vector2.Dot(firstExSeg.src.tangentBC, pn.pointB - rectA);
                                start = Mathf.Min(dot, start);
                                end = Mathf.Max(dot, end);
                            }
                            prevWasInside = true;
                        }
                        else
                        {
                            isInOut = prevWasInside ? 1 : 0;
                            if (HandlePossibleIntersection(rectA, rectB, pn.Previous, out intersection))
                            {
                                //DebugExtension.DebugPoint(intersection, Color.red);
                                dot = Vector2.Dot(firstExSeg.src.tangentBC, intersection - rectA);
                                if (isInOut == 0)
                                {
                                    start = dot;
                                    end = dot;
                                }
                                else
                                {
                                    start = Mathf.Min(dot, start);
                                    end = Mathf.Max(dot, end);
                                }
                                isInOut++;
                            }
                            if (HandlePossibleIntersection(rectB, rectC, pn.Previous, out intersection))
                            {
                                //DebugExtension.DebugPoint(intersection, Color.red);
                                dot = firstExSeg.src.distanceBC;
                                if (isInOut == 0)
                                {
                                    start = dot;
                                    end = dot;
                                }
                                else
                                {
                                    start = Mathf.Min(dot, start);
                                    end = Mathf.Max(dot, end);
                                }
                                isInOut++;
                            }
                            if (HandlePossibleIntersection(rectA, rectD, pn.Previous, out intersection))
                            {
                                //DebugExtension.DebugPoint(intersection, Color.red);
                                dot = 0;
                                if (isInOut == 0)
                                {
                                    start = dot;
                                    end = dot;
                                }
                                else
                                {
                                    start = Mathf.Min(dot, start);
                                    end = Mathf.Max(dot, end);
                                }
                                isInOut++;
                            }
                            if (prevWasInside)
                            {
                                if (isInOut >= 2)
                                    firstExSeg.src.AddObstruction(start, end);
                                prevWasInside = false;
                            }
                            else
                            {
                                if (isInOut == 1)
                                    prevWasInside = true;
                                else if (isInOut == 2)
                                    firstExSeg.src.AddObstruction(start, end);
                            }
                        }
                    }
                    if (prevWasInside)
                        firstExSeg.src.AddObstruction(start, end);
                }
            } while ((firstExSeg = firstExSeg.next) != null);
        }

        private static bool HandlePossibleIntersection(Vector2 a, Vector2 b, PointNode pn, out Vector2 intersection)
        {
            intersection = Vector2.zero;
            Vector2 ip1;  // intersection points

            if (!FindLineIntersection(a, b, pn, out ip1))
                return false;

            if (a == ip1 || b == ip1)
                return false;

            intersection = ip1;
            return true;
        }

        private static bool IsPointInsideRect(float doubledAreaOfRect, Vector2 rectA, Vector2 rectB, Vector2 rectC, Vector2 rectD, Vector2 point)
        {

            float totalTriArea = Mathf.Abs(ExMathf.SignedAreaDoubledTris(rectA, rectB, point));
            if (totalTriArea > doubledAreaOfRect)
                return false;

            totalTriArea += Mathf.Abs(ExMathf.SignedAreaDoubledTris(rectB, rectC, point));
            if (totalTriArea > doubledAreaOfRect)
                return false;

            totalTriArea += Mathf.Abs(ExMathf.SignedAreaDoubledTris(rectC, rectD, point));
            if (totalTriArea > doubledAreaOfRect)
                return false;

            totalTriArea += Mathf.Abs(ExMathf.SignedAreaDoubledTris(rectD, rectA, point));
            if (totalTriArea > doubledAreaOfRect)
                return false;

            return true;
        }

        private static bool IsPointInsideCircleSector(Vector2 a, Vector2 b, float radSquared, Vector2 point)
        {
            //is within radius:
            return ((-a.x * point.y + a.y * point.x > 0 && -b.x * point.y + b.y * point.x <= 0) && point.x * point.x + point.y * point.y <= radSquared);
        }

        private static bool DoesLineIntersectCircleSector(Vector2 lA, Vector2 lB, float radSquared, Vector2 circleCenter, Vector2 sectorStart, Vector2 sectorEnd)
        {
            //DebugExtension.DebugArrow(lA, lB - lA, Color.yellow);
            Vector2 dir = (lB - lA);
            float distL = dir.magnitude;
            dir /= distL;


            float t = dir.x * (circleCenter.x - lA.x) + dir.y * (circleCenter.y - lA.y);

            Vector2 tangent = t * dir + lA;
            float distToCenter = (tangent - circleCenter).sqrMagnitude;


            if (distToCenter < radSquared)
            {
                //DebugExtension.DebugCircle(circleCenter, Vector3.forward * 2, Color.red);
                //DebugExtension.DebugArrow(sectorStart, sectorEnd - sectorStart, Color.red);
                float dt = Mathf.Sqrt(radSquared - distToCenter);
                float tMinDt = t - dt;
                if (tMinDt > 0 + fudgeFactor || tMinDt < distL -fudgeFactor)
                {
                    Vector2 i1 = tMinDt * dir + lA - circleCenter;
                    //DebugExtension.DebugPoint(i1, Color.clear, 3);
                    if (-sectorStart.x * i1.y + sectorStart.y * i1.x > 0 && -sectorEnd.x * i1.y + sectorEnd.y * i1.x < 0)
                        return true;
                }
                tMinDt = t + dt;
                if (tMinDt > 0 + fudgeFactor || tMinDt < distL - fudgeFactor)
                {
                    if (t <= 0 + fudgeFactor || t >= distL - fudgeFactor)
                        return false;
                    Vector2 i2 = tMinDt * dir + lA - circleCenter;
                    //DebugExtension.DebugPoint(i2, Color.clear, 3);
                    if ((-sectorStart.x * i2.y + sectorStart.y * i2.x > 0 && -sectorEnd.x * i2.y + sectorEnd.y * i2.x < 0))
                        return true;
                }
                return false;
            }
            else if (distToCenter == radSquared)
            {
                tangent -= circleCenter;
                //DebugExtension.DebugCircle(circleCenter, Vector3.forward * 2, Color.red);
                //DebugExtension.DebugPoint(tangent, Color.clear, 3);
                return (-sectorStart.x * tangent.y + sectorStart.y * tangent.x > 0 && -sectorEnd.x * tangent.y + sectorEnd.y * tangent.x < 0);
            }
            else
            {
                return false;
            }
        }

        private static bool FindLineIntersection(Vector2 a, Vector2 b, PointNode pn, out Vector2 pA)
        {
            //Assign the resulting points some dummy values
            pA = Vector2.zero;
            Vector2 se1_Begin = a;//(se1.left) ? se1.p : se1.other.p;
            Vector2 se1_End = b;//(se1.left) ? se1.other.p : se1.p;
            Vector2 se2_Begin = pn.pointB;//(se2.left) ? se2.p : se2.other.p;
            Vector2 se2_End = pn.pointC;// (se2.left) ? se2.other.p : se2.p;

            Vector2 d0 = se1_End - se1_Begin;
            Vector2 d1 = se2_End - se2_Begin;
            Vector2 e = se2_Begin - se1_Begin;

            float sqrEpsilon = 0.0000001f; // 0.001 before

            float kross = d0.x * d1.y - d0.y * d1.x;
            float sqrKross = kross * kross;
            float sqrLen0 = d0.sqrMagnitude;
            float sqrLen1 = d1.sqrMagnitude;

            if (sqrKross > sqrEpsilon * sqrLen0 * sqrLen1)
            {
                // lines of the segments are not parallel
                float s = (e.x * d1.y - e.y * d1.x) / kross;
                if ((s < 0) || (s > 1))
                {
                    return false;
                }
                double t = (e.x * d0.y - e.y * d0.x) / kross;
                if ((t < 0) || (t > 1))
                {
                    return false;
                }
                // intersection of lines is a point an each segment
                pA = se1_Begin + s * d0;
                if (Mathf.Approximately(Vector2.Distance(pA, se1_Begin), 0)) pA = se1_Begin;
                if (Mathf.Approximately(Vector2.Distance(pA, se1_End), 0)) pA = se1_End;
                if (Mathf.Approximately(Vector2.Distance(pA, se2_Begin), 0)) pA = se2_Begin;
                if (Mathf.Approximately(Vector2.Distance(pA, se2_End), 0)) pA = se2_End;
                return true;
            }

            return false;
        }

        private static bool DoesLineIntersectBounds(Vector2 pA, Vector2 pB, Bounds bounds)
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

        private static bool ShouldDebugThis(Segment firstExSeg, MarkableContour sp, int markerIndex, float minWalkableHeight)
        {
            return !sp.isSolid && firstExSeg.DistanceAB > 25;
        }

        private static bool ShouldDebugThis(Segment firstExSeg, MarkableContour sp, int markerIndex, float minWalkableHeight, PointNode pn)
        {
            return ShouldDebugThis(firstExSeg, sp, markerIndex, minWalkableHeight);
        }

        /*
        private static void MarkIntersections(Segment firstExSeg, Bounds spBounds, MarkableContour cp, int markerIndex, int spSegCount)
        {
            //Trivial case: The polygons cannot intersect each other.
            if (!spBounds.Intersects(cp.Bounds))
            {
                return;
            }

            //Init the event queue with the polygon edges
            //TODO: Better approximation then * 3
            HeapPriorityQueue<SweepEvent> eventQueue = new HeapPriorityQueue<SweepEvent>((spSegCount + cp.VertCount) * 5);
            InsertPolygon(eventQueue, firstExSeg, PolygonType.SUBJECT);
            InsertPolygon(eventQueue, cp, PolygonType.CLIPPING);

            SweepRay sweepRay = new SweepRay(20);
            SweepEvent cEvent;
            float minRightBounds = Mathf.Min(spBounds.max.x, cp.Bounds.max.x);

            const float fudgeFactor = 0.00001f;
            while (eventQueue.Count != 0)
            {
                cEvent = eventQueue.Dequeue();
                //Early exit. Are there intersections to be found?
                if (cEvent.p.x > minRightBounds + fudgeFactor)
                {
                    return;
                }

                if (cEvent.left)
                {// the line segment must be inserted into S
                    int pos = sweepRay.Add(cEvent);
                    SweepEvent prev = sweepRay.Previous(pos);
                    if (prev == null)
                        cEvent.inside = cEvent.inOut = false;
                    else if (prev.type != EdgeType.NORMAL)
                    {
                        if (pos - 1 == 0)
                        {
                            cEvent.inside = true;
                            cEvent.inOut = false;
                        }
                        else
                        {
                            SweepEvent sliEvent = sweepRay.Previous(pos - 1);
                            if (prev.pl == cEvent.pl)
                            {
                                cEvent.inOut = !prev.inOut;
                                cEvent.inside = !sliEvent.inOut;
                            }
                            else
                            {
                                cEvent.inOut = !sliEvent.inOut;
                                cEvent.inside = !prev.inOut;
                            }
                        }
                    }
                    else if (cEvent.pl == prev.pl)
                    { // previous line segment in S belongs to the same polygon that "cEvent" belongs to
                        cEvent.inside = prev.inside;
                        cEvent.inOut = !prev.inOut;
                    }
                    else
                    {                          // previous line segment in S belongs to a different polygon that "cEvent" belongs to
                        cEvent.inside = !prev.inOut;
                        cEvent.inOut = prev.inside;
                    }

                    SweepEvent nextEvent = sweepRay.Next(pos);
                    if (nextEvent != null)
                        HandlePossibleIntersection(eventQueue, cEvent, nextEvent, markerIndex);
                    if (prev != null)
                        HandlePossibleIntersection(eventQueue, cEvent, prev, markerIndex);
                }
                else
                {// the line segment must be removed from S
                    int pos = sweepRay.Find(cEvent.other);
                    // delete line segment associated to e from S and check for intersection between the neighbors of "e" in S
                    bool isIntersection = (!cp.IsSolid) ? !cEvent.other.inside : cEvent.other.inside;
                    if (isIntersection && cEvent.pl == PolygonType.SUBJECT)
                    {
                        if (cEvent.isCorner)
                        {
                            cEvent.src.MarkPointNotWalkable(markerIndex);
                        }
                        else
                        {
                            float startDot = Vector2.Dot(cEvent.other.src.tangentBC, cEvent.other.p - cEvent.other.src.pointB);
                            float endDot = Vector2.Dot(cEvent.other.src.tangentBC, cEvent.p - cEvent.other.src.pointB);
                            //DebugExtension.DebugArrow(cEvent.p, cEvent.other.p - cEvent.p, Color.yellow);
                            if(startDot > endDot)
                            cEvent.other.src.AddObstruction(endDot, startDot);
                            else
                                cEvent.other.src.AddObstruction(startDot, endDot);
                        }
                    }
                    SweepEvent next = sweepRay.Next(pos), prev = sweepRay.Previous(pos);
                    sweepRay.RemoveAt(pos);
                    if (next != null && prev != null)
                        HandlePossibleIntersection(eventQueue, prev, next, markerIndex);
                }
            }
        }*/

        /*
        private static void InsertPolygon(HeapPriorityQueue<SweepEvent> eventQueue, MarkableContour c, PolygonType pType)
        {
            SweepEvent se1;
            SweepEvent se2;
            foreach (PointNode s in c)
            {
                if (s.pointB.x < s.pointC.x || (s.pointB.x == s.pointC.x && s.pointB.y < s.pointC.y))
                {
                    se1 = new SweepEvent(s.pointB, true, null, pType);
                    se2 = new SweepEvent(s.pointC, false, null, pType);
                }
                else
                {
                    se1 = new SweepEvent(s.pointB, false, null, pType);
                    se2 = new SweepEvent(s.pointC, true, null, pType);
                }
                se1.other = se2;
                se2.other = se1;
                eventQueue.Enqueue(se1);
                eventQueue.Enqueue(se2);
            }
        }

        private static void InsertPolygon(HeapPriorityQueue<SweepEvent> eventQueue, Segment firstExSeg, PolygonType pType)
        {
            SweepEvent se1;
            SweepEvent se2;
            do
            {
                if (firstExSeg.a.x < firstExSeg.b.x || (firstExSeg.a.x == firstExSeg.b.x && firstExSeg.a.y < firstExSeg.b.y))
                {
                    se1 = new SweepEvent(firstExSeg.a, true, firstExSeg.isCorner, firstExSeg.src, pType);
                    se2 = new SweepEvent(firstExSeg.b, false, firstExSeg.isCorner, firstExSeg.src, pType);
                }
                else
                {
                    se1 = new SweepEvent(firstExSeg.a, false, firstExSeg.isCorner, firstExSeg.src, pType);
                    se2 = new SweepEvent(firstExSeg.b, true, firstExSeg.isCorner, firstExSeg.src, pType);
                }
                se1.other = se2;
                se2.other = se1;
                eventQueue.Enqueue(se1);
                eventQueue.Enqueue(se2);
            } while ((firstExSeg = firstExSeg.next) != null);
        }

        private static int FindIntersection(SweepEvent se1, SweepEvent se2, out Vector2 pA, out Vector2 pB)
        {
            //Assign the resulting points some dummy values
            pA = Vector2.zero;
            pB = Vector2.zero;
            Vector2 se1_Begin = (se1.left) ? se1.p : se1.other.p;
            Vector2 se1_End = (se1.left) ? se1.other.p : se1.p;
            Vector2 se2_Begin = (se2.left) ? se2.p : se2.other.p;
            Vector2 se2_End = (se2.left) ? se2.other.p : se2.p;

            Vector2 d0 = se1_End - se1_Begin;
            Vector2 d1 = se2_End - se2_Begin;
            Vector2 e = se2_Begin - se1_Begin;

            float sqrEpsilon = 0.0000001f; // 0.001 before

            float kross = d0.x * d1.y - d0.y * d1.x;
            float sqrKross = kross * kross;
            float sqrLen0 = d0.sqrMagnitude;
            float sqrLen1 = d1.sqrMagnitude;

            if (sqrKross > sqrEpsilon * sqrLen0 * sqrLen1)
            {
                // lines of the segments are not parallel
                float s = (e.x * d1.y - e.y * d1.x) / kross;
                if ((s < 0) || (s > 1))
                {
                    return 0;
                }
                double t = (e.x * d0.y - e.y * d0.x) / kross;
                if ((t < 0) || (t > 1))
                {
                    return 0;
                }
                // intersection of lines is a point an each segment
                pA = se1_Begin + s * d0;
                if (Mathf.Approximately(Vector2.Distance(pA, se1_Begin), 0)) pA = se1_Begin;
                if (Mathf.Approximately(Vector2.Distance(pA, se1_End), 0)) pA = se1_End;
                if (Mathf.Approximately(Vector2.Distance(pA, se2_Begin), 0)) pA = se2_Begin;
                if (Mathf.Approximately(Vector2.Distance(pA, se2_End), 0)) pA = se2_End;
                return 1;
            }

            // lines of the segments are parallel
            float sqrLenE = e.sqrMagnitude;
            kross = e.x * d0.y - e.y * d0.x;
            sqrKross = kross * kross;
            if (sqrKross > sqrEpsilon * sqrLen0 * sqrLenE)
            {
                // lines of the segment are different
                return 0;
            }

            // Lines of the segments are the same. Need to test for overlap of segments.
            float s0 = (d0.x * e.x + d0.y * e.y) / sqrLen0;  // so = Dot (D0, E) * sqrLen0
            float s1 = s0 + (d0.x * d1.x + d0.y * d1.y) / sqrLen0;  // s1 = s0 + Dot (D0, D1) * sqrLen0
            float smin = Mathf.Min(s0, s1);
            float smax = Mathf.Max(s0, s1);
            float[] w = new float[2];
            int imax = FindIntersection(0.0f, 1.0f, smin, smax, w);

            if (imax > 0)
            {
                pA = se1_Begin + w[0] * d0;
                if (Mathf.Approximately(Vector2.Distance(pA, se1_Begin), 0)) pA = se1_Begin;
                if (Mathf.Approximately(Vector2.Distance(pA, se1_End), 0)) pA = se1_End;
                if (Mathf.Approximately(Vector2.Distance(pA, se2_Begin), 0)) pA = se2_Begin;
                if (Mathf.Approximately(Vector2.Distance(pA, se2_End), 0)) pA = se2_End;
                if (imax > 1)
                {
                    pB = se1_Begin + w[1] * d0;
                }
            }
            return imax;
        }

        private static int FindIntersection(float u0, float u1, float v0, float v1, float[] w)
        {
            if ((u1 < v0) || (u0 > v1))
                return 0;
            if (u1 > v0)
            {
                if (u0 < v1)
                {
                    w[0] = (u0 < v0) ? v0 : u0;
                    w[1] = (u1 > v1) ? v1 : u1;
                    return 2;
                }
                else
                {
                    // u0 == v1
                    w[0] = u0;
                    return 1;
                }
            }
            else
            {
                // u1 == v0
                w[0] = u1;
                return 1;
            }
        }

        private static void HandlePossibleIntersection(HeapPriorityQueue<SweepEvent> eventQueue, SweepEvent e1, SweepEvent e2, int markerIndex)
        {
            if (e1.pl == e2.pl)
                return;
            Vector2 ip1, ip2;  // intersection points
            int nintersections;

            if ((nintersections = FindIntersection(e1, e2, out ip1, out ip2)) == 0)
                return;

            if ((nintersections == 1) && ((e1.p == e2.p) || (e1.other.p == e2.other.p)))
                return; // the line segments intersect at an endpoint of both line segments

            if (nintersections == 2 && e1.pl == e2.pl)
                return; // the line segments overlap, but they belong to the same polygon

            // The line segments associated to e1 and e2 intersect
            if (nintersections == 1)
            {
                if (e1.p != ip1 && e1.other.p != ip1)  // if ip1 is not an endpoint of the line segment associated to e1 then divide "e1"
                    DivideEdge(eventQueue, e1, ip1, markerIndex);
                if (e2.p != ip1 && e2.other.p != ip1)  // if ip1 is not an endpoint of the line segment associated to e2 then divide "e2"
                    DivideEdge(eventQueue, e2, ip1, markerIndex);
                return;
            }

            // The line segments overlap
            List<SweepEvent> sortedEvents = new List<SweepEvent>(2);
            if (e1.p == e2.p)
            {
                sortedEvents.Add(null);
            }
            else if (e1.CompareTo(e2) > 0)
            {
                sortedEvents.Add(e2);
                sortedEvents.Add(e1);
            }
            else
            {
                sortedEvents.Add(e1);
                sortedEvents.Add(e2);
            }

            if (e1.other.p == e2.other.p)
            {
                sortedEvents.Add(null);
            }
            else if (e1.other.CompareTo(e2.other) > 0)
            {
                sortedEvents.Add(e2.other);
                sortedEvents.Add(e1.other);
            }
            else
            {
                sortedEvents.Add(e1.other);
                sortedEvents.Add(e2.other);
            }

            if (sortedEvents.Count == 2)
            { // are both line segments equal?
                e1.type = e1.other.type = EdgeType.NON_CONTRIBUTING;
                e2.type = e2.other.type = (e1.inOut == e2.inOut) ? EdgeType.SAME_TRANSITION : EdgeType.DIFFERENT_TRANSITION;
                return;
            }
            if (sortedEvents.Count == 3)
            { // the line segments share an endpoint
                sortedEvents[1].type = sortedEvents[1].other.type = EdgeType.NON_CONTRIBUTING;
                if (sortedEvents[0] != null)         // is the right endpoint the shared point?
                    sortedEvents[0].other.type = (e1.inOut == e2.inOut) ? EdgeType.SAME_TRANSITION : EdgeType.DIFFERENT_TRANSITION;
                else                                // the shared point is the left endpoint
                    sortedEvents[2].other.type = (e1.inOut == e2.inOut) ? EdgeType.SAME_TRANSITION : EdgeType.DIFFERENT_TRANSITION;
                DivideEdge(eventQueue, sortedEvents[0] != null ? sortedEvents[0] : sortedEvents[2].other, sortedEvents[1].p, markerIndex);
                return;
            }
            if (sortedEvents[0] != sortedEvents[3].other)
            { // no line segment includes totally the other one
                sortedEvents[1].type = EdgeType.NON_CONTRIBUTING;
                sortedEvents[2].type = (e1.inOut == e2.inOut) ? EdgeType.SAME_TRANSITION : EdgeType.DIFFERENT_TRANSITION;
                DivideEdge(eventQueue, sortedEvents[0], sortedEvents[1].p, markerIndex);
                DivideEdge(eventQueue, sortedEvents[1], sortedEvents[2].p, markerIndex);
                return;
            }
            // one line segment includes the other one
            sortedEvents[1].type = sortedEvents[1].other.type = EdgeType.NON_CONTRIBUTING;
            DivideEdge(eventQueue, sortedEvents[0], sortedEvents[1].p, markerIndex);
            sortedEvents[3].other.type = (e1.inOut == e2.inOut) ? EdgeType.SAME_TRANSITION : EdgeType.DIFFERENT_TRANSITION;
            DivideEdge(eventQueue, sortedEvents[3].other, sortedEvents[2].p, markerIndex);
        }

        private static void DivideEdge(HeapPriorityQueue<SweepEvent> eventQueue, SweepEvent e, Vector2 p, int markerIndex)
        {
            // "Right event" of the "left line segment" resulting from dividing e (the line segment associated to e)
            SweepEvent r = new SweepEvent(p, false, e.isCorner, e.src, e.pl, e.type);
            r.other = e;

            // "Left event" of the "right line segment" resulting from dividing e (the line segment associated to e)
            SweepEvent l = new SweepEvent(p, true, e.isCorner, e.src, e.pl, e.other.type);
            l.other = e.other;

            if (l.CompareTo(e.other) > 0)
            { // avoid a rounding error. The left event would be processed after the right event
                Debug.LogWarning("Oops");
                e.other.left = true;
                l.left = false;
            }
            if (e.CompareTo(r) > 0)
            { // avoid a rounding error. The left event would be processed after the right event
                Debug.LogWarning("Oops2");
            }
            e.other.other = l;
            e.other = r;
            eventQueue.Enqueue(r);
            eventQueue.Enqueue(l);
        }*/

        /*
        class SweepEvent : PriorityQueueNode
        {

            public bool left;         // is the point the left endpoint of the segment (p, other.p)?
            public PolygonType pl;    // Polygon to which the associated segment belongs to
            public SweepEvent other; // Event associated to the other endpoint of the segment
                                     *  Does the segment (p, other.p) represent an inside-outside transition in the polygon for a vertical ray from (p.x, -infinite) that crosses the segment? 
            public bool inOut;
            public EdgeType type;
            public bool inside; // Only used in "left" events. Is the segment (p, other.p) inside the other polygon?

            public Vector2 p;// point associated with the event
            public readonly PointNode src;
            public readonly bool isCorner;

            /** Class constructor 
            public SweepEvent(Vector2 point, bool left, PointNode src, PolygonType pTyp, EdgeType t = EdgeType.NORMAL)
            {
                this.p = point;
                this.left = left;
                this.src = src;
                pl = pTyp;
                type = t;
                isCorner = false;
            }

            public SweepEvent(Vector2 point, bool left, bool isCorner, PointNode src, PolygonType pTyp, EdgeType t = EdgeType.NORMAL)
            {
                this.p = point;
                this.left = left;
                this.src = src;
                this.isCorner = isCorner;
                pl = pTyp;
                type = t;
            }

            /** Is the line segment (p, other.p) below point x 
            public bool IsBelow(Vector2 o) { return (left) ? ExMathf.SignedAreaDoubledTris(p, other.p, o) > 0 : ExMathf.SignedAreaDoubledTris(other.p, p, o) > 0; }
            /** Is the line segment (p, other.p) above point x 
            public bool IsAbove(Vector2 o) { return !IsBelow(o); }

            // Return true(1) means that e1 is placed at the event queue after e2, i.e,, e1 is processed by the algorithm after e2
            public override int CompareTo(PriorityQueueNode other)
            {
                if (other.GetType() == typeof(SweepEvent))
                {
                    SweepEvent so = (SweepEvent)other;
                    if (p.x > so.p.x)
                        return 1;
                    if (p.x < so.p.x)
                        return -1;
                    if (p.y > so.p.y)
                        return 1;
                    if (p.y < so.p.y)
                        return -1;
                    if (left != so.left)
                    {
                        if (left)
                            return 1;
                        return -1;
                    }
                    if (IsAbove(so.other.p))
                        return 1;
                    return -1;
                }
                return base.CompareTo(other);

            }

            public static bool operator ==(SweepEvent se1, SweepEvent se2)
            {
                if (object.ReferenceEquals(se1, null))
                    return object.ReferenceEquals(se2, null);
                if (object.ReferenceEquals(se2, null))
                    return object.ReferenceEquals(se1, null);
                return (se1.p == se2.p && se1.left == se2.left && se1.other.p == se2.other.p);
            }

            public static bool operator !=(SweepEvent se1, SweepEvent se2)
            {
                return !(se1 == se2);
            }

            public override string ToString()
            {
                return "SE (p = " + p + ", l = " + left + ", pl = " + pl + ", inOut = " + ((left) ? inOut : other.inOut) + ", inside = " + ((left) ? inside : other.inside) + ", other.p = " + other.p + ")";
            }
        }

        class SweepRay
        {
            List<SweepEvent> s;

            public SweepRay(int capacity)
            {
                s = new List<SweepEvent>(capacity);
            }

            public int Add(SweepEvent e)
            {
                for (int i = 0; i < s.Count; i++)
                {
                    SweepEvent se = s[i];
                    if (IsEventOneMoreImportant(se, e))
                        continue;
                    s.Insert(i, e);
                    return i;
                }
                s.Add(e);
                return s.Count - 1;
            }

            public int Find(SweepEvent e)
            {
                return s.IndexOf(e);
            }

            public void RemoveAt(int index)
            {
                s.RemoveAt(index);
            }

            public SweepEvent Next(int index)
            {
                index++;
                if (index < s.Count)
                    return s[index];
                return null;
            }

            public SweepEvent Previous(int index)
            {
                index--;
                if (index >= 0)
                    return s[index];
                return null;
            }

            public override string ToString()
            {
                string result = "[" + s.Count + "] ";
                foreach (SweepEvent se in s)
                    result += ((se.left) ? "l" : "r") + se.ToString() + ", ";
                return result;
            }

            private bool IsEventOneMoreImportant(SweepEvent se1, SweepEvent se2)
            {
                if (se1 == se2)
                    return false;
                if (ExMathf.SignedAreaDoubledTris(se1.p, se1.other.p, se2.p) != 0 || ExMathf.SignedAreaDoubledTris(se1.p, se1.other.p, se2.other.p) != 0)
                {
                    if (se1.p == se2.p)
                        return se1.IsBelow(se2.other.p);

                    if (se1.CompareTo(se2) > 0)
                        return se2.IsAbove(se1.p);
                    return se1.IsBelow(se2.p);
                }
                if (se1.p == se2.p)
                    return false; //Not sure here. Seems like lines exactly overlap each other. Didnt found the < operator though.
                return se1.CompareTo(se2) > 0;
            }
        }*/

        class Segment
        {
            public float DistanceAB { get { return src.distanceBC; } }
            public Vector2 a;
            public Vector2 b;
            public bool isCorner;
            public Segment next;
            public PointNode src;
        }
    }
}
