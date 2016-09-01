using UnityEngine;
using System.Collections;
using Actors;

public enum PlayerLimitation { NoLimitation, BlockMovement, Freeze, BlockNonMovement, ResetMovementInput, BlockEquipmentUse }

public class PlayerLimitationHandler
{
    PlayerActor actor;
    PlayerLimitation[] currentLimitations;
    UnityEventHog eventHog;
    bool lockAnimationTrigger = false;

    public PlayerLimitationHandler(PlayerActor actor, UnityEventHog eventHog)
    {
        this.actor = actor;
        this.eventHog = eventHog;
        SetLimitation(PlayerLimitation.NoLimitation);
    }

    public void SetLimitation(params PlayerLimitation[] limit)
    {
        ApplyLimitation(limit);
    }

    public void SetLimitation(PlayerLimitation[] limit, float duration, PlayerLimitation[] fallback)
    {
        SetLimitation(limit);
        eventHog.DelayActionBySeconds(new UnityEventHog.DelayedAction(SetLimitation), duration, fallback);
    }

    public void SetLimitation(float duration, params PlayerLimitation[] limit)
    {
        PlayerLimitation[] prevLimit = currentLimitations;
        SetLimitation(limit);
        eventHog.DelayActionBySeconds(new UnityEventHog.DelayedAction(SetLimitation), duration, prevLimit);
    }

    public void SetLimitationAfter(float time, params PlayerLimitation[] limit)
    {
        eventHog.DelayActionBySeconds(new UnityEventHog.DelayedAction(SetLimitation), time, limit);
    }

    public bool AreAnimationTriggerLocked()
    {
        return lockAnimationTrigger;
    }

    public void LockAnimaionTrigger(bool lockTrigger)
    {
        lockAnimationTrigger = lockTrigger;
    }

    void SetLimitation(object limit)
    {
        SetLimitation((PlayerLimitation[])limit);
    }

    void ApplyLimitation(PlayerLimitation[] newLimit)
    {
        currentLimitations = newLimit;
        foreach (PlayerLimitation l in newLimit)
        {
            switch (l)
            {
                case PlayerLimitation.NoLimitation:
                    actor.HumanInput.SetAllowInput(true);
                    actor.PlayerEquipmentManager.allowInput = true;
                    actor.CC2DMotor.IsFroozen = false;
                    break;
                case PlayerLimitation.BlockMovement:
                    actor.HumanInput.SetAllowInput(false);
                    actor.PlayerEquipmentManager.allowInput = false;
                    break;
                case PlayerLimitation.Freeze:
                    actor.CC2DMotor.IsFroozen = true;
                    break;
                case PlayerLimitation.ResetMovementInput:
                    actor.CC2DMotor.ResetPlayerMovementInput();
                    break;
                case PlayerLimitation.BlockNonMovement:
                    actor.PlayerEquipmentManager.allowInput = false;
                    break;
                case PlayerLimitation.BlockEquipmentUse:
                    actor.PlayerEquipmentManager.allowInput = false;
                    break;
            }
        }
    }   
}
