using UnityEngine;
using Pada1.BBCore;
using Pada1.BBCore.Framework;
using Pada1.BBCore.Tasks;

namespace BBUnity.Actions
{
    [Action("Transform/FindNearest")]
    [Help("Determines the nearest point and returns it.")]
    public class NearestPosition : BasePrimitiveAction
    {
        [InParam("Start Position")]
        Vector2 startPosition;
        [InParam("Positions")]
        PositionHolder2D positions;

        [OutParam("NearestPosition")]
        Vector2 nearestPosition;

        public override TaskStatus OnUpdate()
        {
            float dist = float.MaxValue;
            float tmpDist;
            foreach (var v in positions.positions)
            {
                tmpDist = (v - startPosition).sqrMagnitude;
                if (tmpDist < dist)
                {
                    nearestPosition = v;
                    dist = tmpDist;
                }
            }
            return TaskStatus.COMPLETED;
        }
    }
}
