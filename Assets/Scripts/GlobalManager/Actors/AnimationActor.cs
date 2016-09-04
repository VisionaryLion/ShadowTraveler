using UnityEngine;
using System.Collections;

namespace Actors
{
    public class AnimationActor : Actor
    {
        [SerializeField]
        Animator animator;
        [SerializeField]
        AnimationHandler animationHandler;

        public Animator Animator { get { return animator; } }
        public AnimationHandler AnimationHandler { get { return animationHandler; } }

#if UNITY_EDITOR
        public override void Refresh()
        {
            base.Refresh();

            animator = GetComponentInChildren<Animator>();
            animationHandler = GetComponentInChildren<AnimationHandler>();
        }
#endif

    }
}
