using UnityEngine;
using System.Collections;
using Pada1.BBCore;
using Pada1.BBCore.Tasks;
using Entities;
using Pada1.BBCore.Framework;
using NavData2d;

namespace BBUnity.Actions
{
    [Action("Navigation/DoPatrol")]
    [Help("Follows a predefined patrol route, starting with the nearest patrol point.")]
    public class DoPatrol : BasePrimitiveAction
    {
        [InParam("AIEntity")]
        [Help("AIEntity to move")]
        public AIEntity entity;
        [InParam("Waypoints")]
        [Help("Waypoints of patrol route")]
        public NavPositionHolder patrolPoints;

        private int currentPatrolPosition;

        public override void OnStart()
        {
            FindNearestWayPoint();
            entity.NavAgent.SetDestination(patrolPoints.handlePositions[currentPatrolPosition].navPosition, new NavAgent.OnPathComputationFinished(OnPathComputationFinished));
        }

        public override TaskStatus OnUpdate()
        {
            if (currentPatrolPosition == -1)
                return TaskStatus.FAILED;

            if (entity.NavAgent.ReachedGoal)
            {
                MoveToNextWaypoint();
            }

            return TaskStatus.RUNNING;
        }

        public override void OnAbort()
        {
            entity.NavAgent.Stop();
        }

        void OnPathComputationFinished(bool pathFound)
        {
            if (!pathFound)
                currentPatrolPosition = -1;
        }

        void FindNearestWayPoint()
        {
            float nearestDist = (patrolPoints.handlePositions[0].navPosition.navPoint - (Vector2)entity.transform.position).sqrMagnitude;
            currentPatrolPosition = 0;
            float tmpDist;
            for (int iPos = 1; iPos < patrolPoints.handlePositions.Length; iPos++)
            {
                tmpDist = (patrolPoints.handlePositions[iPos].navPosition.navPoint - (Vector2)entity.transform.position).sqrMagnitude;
                if (tmpDist < nearestDist)
                {
                    currentPatrolPosition = iPos;
                    nearestDist = tmpDist;
                }
            }
        }

        void MoveToNextWaypoint()
        {
            currentPatrolPosition++;
            if (currentPatrolPosition >= patrolPoints.handlePositions.Length)
                currentPatrolPosition = 0;
            entity.NavAgent.SetDestination(patrolPoints.handlePositions[currentPatrolPosition].navPosition, new NavAgent.OnPathComputationFinished(OnPathComputationFinished));
        }
    }
}
