using UnityEngine;
using System.Collections;
using Actors;
using System;
using Combat;

public class SwingCrowbar : MonoBehaviour, IEquipment
{
    [SerializeField]
    AreaHarzard2D hitBox;

    AnimationActor actor;
    AnimationHandler.AnimationEvent callBack;

    void Start()
    {
        hitBox.hitHandler += HitBox_hitHandler;
        callBack = new AnimationHandler.AnimationEvent(CrowbarSwingFinishedHandler);
    }

    private void HitBox_hitHandler(IDamageReciever reciever)
    {
        Debug.Log("Hit = "+reciever.name);
        actor.Animator.SetTrigger("Aboard_SwingCrowbar");
        actor.AnimationHandler.StopListenToAnimationEnd(callBack);
        CrowbarSwingFinishedHandler();
    }

    public void OnEquiped()
    {
        enabled = true;
        actor = GetComponentInParent<AnimationActor>();
    }

    public void OnUnequiped()
    {
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1") && !hitBox.dealDamage && actor.AnimationHandler.CanAquireAnyStateTransitionPriority(1, 0) && !actor.Animator.GetCurrentAnimatorStateInfo(1).IsName("SwingCrowbar_Anim"))
        {
            actor.Animator.SetTrigger("SwingCrowbar");
            actor.AnimationHandler.StartListenToAnimationEnd("SwingCrowbar_Anim", callBack);
            hitBox.dealDamage = true;

        }
    }

    private void CrowbarSwingFinishedHandler()
    {
        hitBox.dealDamage = false;
    }
}
