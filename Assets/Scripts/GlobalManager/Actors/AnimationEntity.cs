﻿using UnityEngine;
using System.Collections;

namespace Entities
{
    public class AnimationEntity : Entity
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

            animator = LoadComponent<Animator>(animator);
            animationHandler = LoadComponent<AnimationHandler>(animationHandler);
        }
#endif

    }
}