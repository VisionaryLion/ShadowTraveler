using UnityEngine;
using System.Collections;

namespace Actors
{
    public class ThightAIMovementActor : SimpleMovementActor
    {
        [SerializeField]
        CC2DThightAIMotor cC2DThightAIMotor;
        [SerializeField]
        NavAgent navAgent;

        public CC2DThightAIMotor CC2DThightAIMotor { get { return cC2DThightAIMotor; } }
        public NavAgent NavAgent { get { return navAgent; } }

#if UNITY_EDITOR
        public override void Refresh()
        {
            base.Refresh();

            cC2DThightAIMotor = GetComponentInChildren<CC2DThightAIMotor>();
            navAgent = GetComponentInChildren<NavAgent>();
        }
#endif
    }
}
