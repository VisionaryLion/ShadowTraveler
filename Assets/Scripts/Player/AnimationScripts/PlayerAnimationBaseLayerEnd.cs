using UnityEngine;
using System.Collections;

public class PlayerAnimationBaseLayerEnd : StateMachineBehaviour
{
    public delegate void OnDeathAnimFinished();
    public event OnDeathAnimFinished DeathAnimFinishedHandler;

    public delegate void OnPickUpAnimFinished();
    public event OnPickUpAnimFinished PickUpFinishedHandler;

    void OnDeathAnimationFinished()
    {
        if (DeathAnimFinishedHandler != null)
            DeathAnimFinishedHandler.Invoke();
    }

    void OnPickUpAnimationFinished()
    {
        if (PickUpFinishedHandler != null)
            PickUpFinishedHandler.Invoke();
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsName("Item_Pick_up_Anim"))
            OnPickUpAnimationFinished();
        else if (stateInfo.IsName("Death_Anim"))
            OnDeathAnimationFinished();
    }
}
