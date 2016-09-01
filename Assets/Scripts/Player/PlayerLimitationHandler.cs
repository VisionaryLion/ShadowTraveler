using UnityEngine;
using System.Collections;
using Actors;

public class PlayerLimitationHandler
{

    public enum PlayerLimition { NoLimitation, BlockPlayerInput, Freeze, BlockInputAndFreeze, BlockNonMovement }

    PlayerActor actor;
    PlayerLimition limitation;
    UnityEventHog eventHog;

    public PlayerLimitationHandler(PlayerActor actor, UnityEventHog eventHog)
    {
        this.actor = actor;
        this.eventHog = eventHog;
        SetLimitation(PlayerLimition.NoLimitation);
    }

    public void SetLimitation(PlayerLimition limit)
    {
        limitation = limit;

        if (limitation != PlayerLimition.NoLimitation)
            ApplyLimitation(PlayerLimition.NoLimitation);

        ApplyLimitation(limit);
    }

    public void SetLimitation(PlayerLimition limit, float duration, PlayerLimition fallback)
    {
        SetLimitation(limit);
        eventHog.DelayActionBySeconds(new UnityEventHog.DelayedAction(SetLimitation), duration, fallback);
    }

    public void SetLimitation(PlayerLimition limit, float duration)
    {
        PlayerLimition prevLimit = limitation;
        SetLimitation(limit);
        eventHog.DelayActionBySeconds(new UnityEventHog.DelayedAction(SetLimitation), duration, prevLimit);
    }

    public void SetLimitationAfter(PlayerLimition limit, float time)
    {
        eventHog.DelayActionBySeconds(new UnityEventHog.DelayedAction(SetLimitation), time, limit);
    }

    void SetLimitation(object limit)
    {
        SetLimitation((PlayerLimition)limit);
    }

    void ApplyLimitation(PlayerLimition newLimit)
    {
        limitation = newLimit;
        switch (limitation)
        {
            case PlayerLimition.NoLimitation:
                actor.HumanInput.SetAllowInput(true);
                actor.PlayerEquipmentManager.allowInput = true;
                actor.CC2DMotor.IsFroozen = false;
                break;
            case PlayerLimition.BlockPlayerInput:
                actor.HumanInput.SetAllowInput(false);
                actor.PlayerEquipmentManager.allowInput = false;
                actor.CC2DMotor.ResetPlayerMovementInput();
                break;
            case PlayerLimition.Freeze:
                actor.CC2DMotor.IsFroozen = true;
                break;
            case PlayerLimition.BlockInputAndFreeze:
                actor.HumanInput.SetAllowInput(false);
                actor.PlayerEquipmentManager.allowInput = false;
                actor.CC2DMotor.IsFroozen = true;
                actor.CC2DMotor.ResetPlayerMovementInput();
                break;
            case PlayerLimition.BlockNonMovement:
                actor.PlayerEquipmentManager.allowInput = false;
                break;
        }
    }   
}
