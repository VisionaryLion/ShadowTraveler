using UnityEngine;
using System.Collections;

public class PlayerAnimationUpperBodyEnd : StateMachineBehaviour
{
    public delegate void OnEquipAnimFinished();
    public event OnEquipAnimFinished EquipFinishedHandler;

    public delegate void OnCrowbarSwingAnimFinished();
    public event OnCrowbarSwingAnimFinished CrowbarSwingFinishedHandler;

    void OnEquipAnimationFinished()
    {
        if (EquipFinishedHandler != null)
            EquipFinishedHandler.Invoke();
    }

    void OnCrowbarSwingAnimationFinished()
    {
        if (CrowbarSwingFinishedHandler != null)
            CrowbarSwingFinishedHandler.Invoke();
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsName("SwingCrowbar_Anim"))
            OnCrowbarSwingAnimationFinished();
        else if (stateInfo.IsName("Equip_Item_Anim"))
            OnEquipAnimationFinished();
    }
}
