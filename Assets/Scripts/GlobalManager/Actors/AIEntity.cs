using UnityEngine;

namespace Entities
{
    public class AIEntity : ActingEntity {

        [SerializeField]
        NavAgent navAgent;
        [SerializeField]
        BehaviorExecutor behaviorExecutor;

        #region public
        public NavAgent NavAgent { get { return navAgent; } }
        public BehaviorExecutor BehaviorExecutor { get { return behaviorExecutor; } }
        #endregion

#if UNITY_EDITOR
        public override void Refresh()
        {
            base.Refresh();

            navAgent = LoadComponent<NavAgent>(navAgent);
            behaviorExecutor = LoadComponent<BehaviorExecutor>(behaviorExecutor);
        }
#endif
    }
}
