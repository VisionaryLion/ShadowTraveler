using UnityEngine;
using System.Collections;
using LightSensing;

namespace Actors
{
    public class StateMaschineAIActor : BasicEntityActor
    {
        [SerializeField]
        SimpleStateMaschineBehaviour simpleStateMaschineBehaviour;
        [SerializeField]
        LightSensor lightSensor;
        [SerializeField]
        NavAgent navAgent;
        [SerializeField]
        LineOfSight lineOfSight;

        public SimpleStateMaschineBehaviour SimpleStateMaschineBehaviour { get { return simpleStateMaschineBehaviour; } }
        public LightSensor LightSensor { get { return lightSensor; } }
        public NavAgent NavAgent { get { return navAgent; } }
        public LineOfSight LineOfSight { get { return lineOfSight; } }

#if UNITY_EDITOR
        public override void Refresh()
        {
            base.Refresh();
            simpleStateMaschineBehaviour = LoadComponent<SimpleStateMaschineBehaviour>(simpleStateMaschineBehaviour);
            lightSensor = LoadComponent<LightSensor>(lightSensor);
            navAgent = LoadComponent<NavAgent>(navAgent);
            lineOfSight = LoadComponent<LineOfSight>(lineOfSight);
        }
#endif
    }
}
