// - AUTHOR : Oribow
using UnityEngine;
using Entity;

namespace SurfaceTypeUser
{
    public class FootstepEmitter : MonoBehaviour
    {
        [SerializeField]
        [AssignEntityAutomaticly]
        [HideInInspector]
        ActingEntity actor;

        [FMODUnity.EventRef]
        [SerializeField]
        string footStepEvent;

        FMOD.Studio.EventInstance footStepInstance;
        AnimationHandler.AnimationEvent _OnFootHitGround;

        void Start()
        {
            _OnFootHitGround = new AnimationHandler.AnimationEvent(OnFootHitGround);
            actor.AnimationHandler.StartListenToAnimationEvent("FootHitGround", _OnFootHitGround);
            footStepInstance = FMODUnity.RuntimeManager.CreateInstance(footStepEvent);
        }

        void OnFootHitGround()
        {
            actor.AnimationHandler.StartListenToAnimationEvent("FootHitGround", _OnFootHitGround);
            footStepInstance.start();
        }

        void Update()
        {
            footStepInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(this.transform, null));
        }

        void OnDestroy()
        {
            footStepInstance.release();
            footStepInstance = null;
        }
    }
}
