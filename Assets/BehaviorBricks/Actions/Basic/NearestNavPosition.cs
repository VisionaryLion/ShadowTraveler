using UnityEngine;
using Pada1.BBCore;
using Pada1.BBCore.Framework;
using Pada1.BBCore.Tasks;
using NavData2d;

namespace BBUnity.Actions
{
    [Action("Transform/FindNearestNavPosition")]
    [Help("Determines the nearest point and returns it.")]
    public class NearestNavPosition : BasePrimitiveAction
    {
        [InParam("Start Position")]
        Vector2 startPosition;
        [InParam("Positions")]
        NavPositionHolder positions;

        [OutParam("NearestPosition")]
        Vector2 nearestPosition;

        public override TaskStatus OnUpdate()
        {
            float dist = float.MaxValue;
            float tmpDist;
            foreach (var v in positions.handlePositions)
            {
                tmpDist = (v.navPosition.navPoint - startPosition).sqrMagnitude;
                if (tmpDist < dist)
                {
                    nearestPosition = v.navPosition.navPoint;
                    dist = tmpDist;
                }
            }
            return TaskStatus.COMPLETED;
        }
    }
}
