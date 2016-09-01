using UnityEngine;
using System.Collections;
using Actors;
using System;
using Combat;

public class SwingCrowbar : MonoBehaviour, IEquipment
{
    [SerializeField]
    AreaHarzard2D hitBox;

    PlayerActor actor;

    void Start()
    {
        actor = ActorDatabase.GetInstance().FindFirst<PlayerActor>();
        hitBox.hitHandler += HitBox_hitHandler;
    }

    private void HitBox_hitHandler(IDamageReciever reciever)
    {
        if (!actor.PlayerLimitationHandler.AreAnimationTriggerLocked())
        {
            actor.CC2DMotor.frontAnimator.SetTrigger("Aboard_SwingCrowbar");
            PlayerAnimationEventGrabberFront_CrowbarSwingFinishedHandler();
        }
    }

    public void OnEquiped()
    {
        enabled = true;
    }

    public void OnUnequiped()
    {
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1") && !hitBox.dealDamage && !actor.PlayerLimitationHandler.AreAnimationTriggerLocked())
        {
            actor.CC2DMotor.frontAnimator.SetTrigger("SwingCrowbar");
            actor.PlayerAnimationUpperBodyEnd.CrowbarSwingFinishedHandler += PlayerAnimationEventGrabberFront_CrowbarSwingFinishedHandler;
            hitBox.dealDamage = true;

        }
    }

    private void PlayerAnimationEventGrabberFront_CrowbarSwingFinishedHandler()
    {
        actor.PlayerAnimationUpperBodyEnd.CrowbarSwingFinishedHandler -= PlayerAnimationEventGrabberFront_CrowbarSwingFinishedHandler;
        hitBox.dealDamage = false;
    }
}
