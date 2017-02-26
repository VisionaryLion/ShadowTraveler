using UnityEngine;
using System.Collections;
using Pada1.BBCore.Framework;
using Pada1.BBCore;
using Pada1.BBCore.Tasks;
using Entities;

namespace BBUnity.Actions
{
    [Action("AIInteraction/Attack")]
    [Help("Attacks the given enemy")]
    public class Attack : BasePrimitiveAction
    {
        [InParam("AIEntity")]
        [Help("AIEntity to move")]
        public AIEntity entity;
        [InParam("Enemy")]
        ActingEntity enemy;
        [InParam("Attack Rad")]
        [Help("Radius around target in which to attack")]
        float attackRad;

        [OutParam("Latest Known Enemy Pos")]
        Vector2 latestEnemyPos;

        bool pathIsFound;
        float squaredRad;

        public override void OnStart()
        {
            entity.NavAgent.SetDestination(enemy.transform.position, new NavAgent.OnPathComputationFinished(OnPathComputationFinished));
            pathIsFound = true;
            squaredRad = attackRad * attackRad;
            latestEnemyPos = enemy.transform.position;
        }

        private void OnPathComputationFinished(bool foundPath)
        {
            pathIsFound = foundPath;
        }

        public override TaskStatus OnUpdate()
        {
            if (!pathIsFound)
                return TaskStatus.FAILED;

            if (squaredRad >= (enemy.transform.position - entity.transform.position).sqrMagnitude)
            {
                Debug.Log("Attack -> DIE YOU MONSTER!");
            }
            else if (latestEnemyPos != (Vector2)enemy.transform.position)
            {
                entity.NavAgent.SetDestination(enemy.transform.position, new NavAgent.OnPathComputationFinished(OnPathComputationFinished));
                latestEnemyPos = enemy.transform.position;
            }
            return TaskStatus.RUNNING;
        }

        public override void OnAbort()
        {
            entity.NavAgent.Stop();
        }
    }
}
