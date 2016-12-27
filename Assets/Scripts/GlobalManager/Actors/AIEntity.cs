using UnityEngine;
using AI.Brain;
using AI.Sensor;

namespace Entity
{
    public class AIEntity : ActingEntity, IAILogic {

        protected BlackboardHolder blackboardHolder;

        [SerializeField]
        LineOfSight lineOfSight;
        [SerializeField]
        LightSensor lightSensor;
        [SerializeField]
        NavAgent navAgent;

        #region public
        public LineOfSight LineOfSight { get { return lineOfSight; } }
        public LightSensor LightSensor { get { return lightSensor; } }
        public NavAgent NavAgent { get { return navAgent; } }
        #endregion

        protected override void Awake()
        {
            base.Awake();
            blackboardHolder = new BlackboardHolder(this.lineOfSight, this.lightSensor);
        }

        void Start()
        {
            GlobalAILogicScheduler.Instance.AddAILogic(this);
        }

#if UNITY_EDITOR
        public override void Refresh()
        {
            base.Refresh();

            lineOfSight = LoadComponent<LineOfSight>(lineOfSight);
            lightSensor = LoadComponent<LightSensor>(lightSensor);
            navAgent = LoadComponent<NavAgent>(navAgent);
        }
#endif

        public void Think()
        {
            blackboardHolder.Update();

        }
    }
}
