using UnityEngine;
using System.Collections;
using Entities;
using System;
using Combat;

public class SwingCrowbar : ItemSpecificBase
{
    [SerializeField]
    AreaHarzard2D hitBox;
        
    AnimationHandler.AnimationEvent callBack;

    protected override void Start ()
    {
        base.Start();
        hitBox.hitHandler += HitBox_hitHandler;
        callBack = new AnimationHandler.AnimationEvent(CrowbarSwingFinishedHandler);
        enabled = false;
    }

    private void HitBox_hitHandler(IDamageReciever reciever)
    {
        Debug.Log("Hit = "+reciever.name);
        equipedEntity.Animator.SetTrigger("Aboard_SwingCrowbar");
        equipedEntity.AnimationHandler.StopListenToAnimationEnd(callBack);
        CrowbarSwingFinishedHandler();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOnPlayer())
            return;

        if (Input.GetButton("Fire1") && !hitBox.dealDamage && equipedEntity.AnimationHandler.CanAquireAnyStateTransitionPriority(1, 0) && !equipedEntity.Animator.GetCurrentAnimatorStateInfo(1).IsName("SwingCrowbar_Anim"))
        {
            equipedEntity.Animator.SetTrigger("SwingCrowbar");
            equipedEntity.AnimationHandler.StartListenToAnimationEnd("SwingCrowbar_Anim", callBack);
            hitBox.dealDamage = true;
        }
    }

    private void CrowbarSwingFinishedHandler()
    {
        hitBox.dealDamage = false;
    }
}
