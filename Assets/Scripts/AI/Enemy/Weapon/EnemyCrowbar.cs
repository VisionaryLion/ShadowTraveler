using UnityEngine;
using System.Collections;
using Entities;
using System;
using Combat;

public class EnemyCrowbar : ItemSpecificBase
{
    [SerializeField]
    AreaHarzard2D hitBox;

    AnimationHandler.AnimationEvent callBack;

    protected override void Start()
    {
        base.Start();
        hitBox.hitHandler += HitBox_hitHandler;
        callBack = new AnimationHandler.AnimationEvent(CrowbarSwingFinishedHandler);
        enabled = false;
    }

    private void HitBox_hitHandler(IDamageReciever reciever)
    {
        Debug.Log("Hit = " + reciever.name);
        this.GetComponentInParent<AnimationEntity>().Animator.SetTrigger("Aboard_SwingCrowbar");
        this.GetComponentInParent<AnimationEntity>().AnimationHandler.StopListenToAnimationEnd(callBack);
        CrowbarSwingFinishedHandler();
    }

    public void Swing()
    {
        this.GetComponentInParent<AnimationEntity>().Animator.SetTrigger("SwingCrowbar");
        this.GetComponentInParent<AnimationEntity>().AnimationHandler.StartListenToAnimationEnd("SwingCrowbar_Anim", callBack);
        hitBox.dealDamage = true;
    }

    private void CrowbarSwingFinishedHandler()
    {
        hitBox.dealDamage = false;
    }
}