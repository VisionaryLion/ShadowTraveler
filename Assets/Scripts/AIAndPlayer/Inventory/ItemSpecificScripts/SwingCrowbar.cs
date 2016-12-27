using UnityEngine;
using System.Collections;
using Entities;
using System;
using Combat;

public class SwingCrowbar : MonoBehaviour
{
    [SerializeField, AssignEntityAutomaticly, HideInInspector]
    TwoHandItemEntity entity;
    [SerializeField]
    AreaHarzard2D hitBox;
        
    AnimationEntity actor;
    AnimationHandler.AnimationEvent callBack;

    void Start ()
    {
        entity.EquipedHandler += Entity_EquipedHandler;
        entity.UnequipedHandler += Entity_UnequipedHandler;

        hitBox.hitHandler += HitBox_hitHandler;
        callBack = new AnimationHandler.AnimationEvent(CrowbarSwingFinishedHandler);
        enabled = false;
    }

    private void Entity_UnequipedHandler()
    {
        enabled = false;
    }

    private void Entity_EquipedHandler(ActingEquipmentEntity equiper)
    {
        enabled = true;
        actor = GetComponentInParent<AnimationEntity>();
    }

    private void HitBox_hitHandler(IDamageReciever reciever)
    {
        Debug.Log("Hit = "+reciever.name);
        actor.Animator.SetTrigger("Aboard_SwingCrowbar");
        actor.AnimationHandler.StopListenToAnimationEnd(callBack);
        CrowbarSwingFinishedHandler();
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
