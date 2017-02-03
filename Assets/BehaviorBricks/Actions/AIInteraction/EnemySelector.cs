using Pada1.BBCore;
using Pada1.BBCore.Framework;
using Entities;
using Pada1.BBCore.Tasks;

namespace BBUnity.Actions
{
    [Action("AIInteraction/EnemySelector")]
    [Help("Selects an npc")]
    public class EnemySelector : BasePrimitiveAction
    {
        [InParam("SelectableEnemies")]
        [Help("Npcs to select from")]
        public BehaviorBrick.Conditions.IsNPCInSight.ResizableList<BehaviorBrick.Conditions.IsNPCInSight.VisibleNPCs> enemies;

        [InParam("Select Weakest")]
        [Help("Determines, if the enemy with the highest chance of defeat is selected, or the one with the lowest chance.")]
        bool selectWeakest;

        [InParam("Health Multiplier")]
        float healthMult;

        [InParam("Distance Multiplier")]
        float distMult;

        [OutParam("SelectedEnemy")]
        ActingEntity selectedEnemy;

        public override void OnStart()
        {
            if (selectWeakest)
            {
                float prob = float.MaxValue;
                float tmpProb;
                foreach (var e in enemies)
                {
                    tmpProb = ComputeNotKillProbabilty(e);
                    if (tmpProb < prob)
                    {
                        prob = tmpProb;
                        selectedEnemy = e.otherEntity;
                    }
                }
            }
            else
            {
                float prob = -1;
                float tmpProb;
                foreach (var e in enemies)
                {
                    tmpProb = ComputeNotKillProbabilty(e);
                    if (tmpProb > prob)
                    {
                        prob = tmpProb;
                        selectedEnemy = e.otherEntity;
                    }
                }

            }
        }

        float ComputeNotKillProbabilty(BehaviorBrick.Conditions.IsNPCInSight.VisibleNPCs e)
        {
            return e.otherEntity.IHealth.CurrentHealth * healthMult + e.distance * distMult;
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.COMPLETED;
        }
    }
}

