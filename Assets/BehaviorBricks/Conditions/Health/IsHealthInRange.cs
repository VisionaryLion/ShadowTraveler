using Pada1.BBCore.Framework;
using Pada1.BBCore;
using Entities;

namespace BehaviorBrick.Conditions
{
    [Condition("Health/IsHealthInRange")]
    [Help("Checks if the health is in a predefined range")]
    public class IsHealthInRange : ConditionBase
    {
        [InParam("HealthEntity")]
        public HealthEntity entity;

        [InParam("Use Percentage")]
        [Help("If true, the range will be interpreted as percentage of the max health.")]
        public bool usePercentage;

        [InParam("Min")]
        [Help("Minimum (Inclusiv)")]
        public float min;

        [InParam("Max")]
        [Help("Maximum (Inclusiv)")]
        public float max;

        public override bool Check()
        {
            if (usePercentage)
            {
                float healthPerc = entity.IHealth.CurrentHealth / entity.IHealth.MaxHealth * 100;
                return healthPerc >= min && healthPerc <= max;
            }
            else
            {
                return entity.IHealth.CurrentHealth >= min && entity.IHealth.CurrentHealth <= max;
            }
        }
    }
}
