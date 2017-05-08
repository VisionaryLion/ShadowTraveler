using UnityEngine;
using System.Collections;

namespace Entities
{
    public class AnimationEntity : Entity
    {
        [SerializeField]
        Animator animator;
        [SerializeField]
        AnimationHandler animationHandler;
        [SerializeField]
        Skeleton skeleton;

        public Animator Animator { get { return animator; } }
        public AnimationHandler AnimationHandler { get { return animationHandler; } }
        public Skeleton Skeleton {  get { return skeleton; } }

#if UNITY_EDITOR
        public override void Refresh()
        {
            base.Refresh();

            animator = LoadComponent<Animator>(animator);
            animationHandler = LoadComponent<AnimationHandler>(animationHandler);
            skeleton = LoadComponent<Skeleton>(skeleton);
        }
#endif

    }
}
