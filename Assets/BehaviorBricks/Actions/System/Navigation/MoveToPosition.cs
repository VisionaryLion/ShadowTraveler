using Pada1.BBCore.Tasks;
using Pada1.BBCore;
using UnityEngine;
using Pada1.BBCore.Framework;
using Entities;
using System;

namespace BBUnity.Actions
{
    [Action("Navigation/MoveToPosition")]
    [Help("Moves the game object to a given position by using his NavAgent")]
    public class MoveToPosition : BasePrimitiveAction
    {
        [InParam("AIEntity")]
        [Help("AIEntity to move")]
        public AIEntity entity;
        [InParam("target")]
        [Help("Target position where the game object will be moved")]
        public Vector2 target;

        bool isAllOk;

        public override void OnStart()
        {
            entity.NavAgent.SetDestination(target, new NavAgent.OnPathComputationFinished(OnPathComputationFinished));
            isAllOk = true;
        }

        private void OnPathComputationFinished(bool foundPath)
        {
            if (!foundPath)
                isAllOk = false;
        }

        public override TaskStatus OnUpdate()
        {
            if (!isAllOk)
                return TaskStatus.FAILED;
            if (entity.NavAgent.ReachedGoal)
                return TaskStatus.COMPLETED;

            return TaskStatus.RUNNING;
        }

        public override void OnAbort()
        {
            entity.NavAgent.Stop();
        }
    }
}
