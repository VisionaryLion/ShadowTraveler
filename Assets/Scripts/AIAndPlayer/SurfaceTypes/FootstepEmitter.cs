// - AUTHOR : Oribow
using UnityEngine;
using Entities;

namespace SurfaceTypeUser
{
    public class FootstepEmitter : MonoBehaviour
    {
        [SerializeField]
        [AssignEntityAutomaticly]
        [HideInInspector]
        ActingEntity actor;
       
        [SerializeField]
        string footStepEvent;

        AnimationHandler.AnimationEvent _OnFootHitGround;

        void Start()
        {
            _OnFootHitGround = new AnimationHandler.AnimationEvent(OnFootHitGround);
            actor.AnimationHandler.StartListenToAnimationEvent("FootHitGround", _OnFootHitGround);

        }

        void OnFootHitGround()
        {
            actor.AnimationHandler.StartListenToAnimationEvent("FootHitGround", _OnFootHitGround);
        }

        void Update()
        {
            
        }

        void OnDestroy()
        {
            
        }
    }
}
