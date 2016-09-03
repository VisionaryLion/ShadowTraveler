using UnityEngine;
using System.Collections;

public class AnimationEndHandler : StateMachineBehaviour {

    public delegate void OnAnimationEnded(AnimatorStateInfo stateInfo);
    public event OnAnimationEnded AnimationEndEventHandler;

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (AnimationEndEventHandler != null)
            AnimationEndEventHandler.Invoke(stateInfo);
    }
}
