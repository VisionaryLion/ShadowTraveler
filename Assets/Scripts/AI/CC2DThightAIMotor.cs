using UnityEngine;
using CC2D;



public class CC2DThightAIMotor : CC2DMotor
{
    bool useManualXSpeed;
    float manualXSpeed;

    protected override void EndStateUpdate()
    {
        if (useManualXSpeed)
            _cVelocity.x = manualXSpeed;

        base.EndStateUpdate();
    }

    public bool ManualJump(float jumpForce, float jumpDuration)
    {
        if (_cMState == MState.Walk || _cMState == MState.WallSlide || (_cMState == MState.Crouched && _crouchTrigger > 0)
            || (_cMState == MState.Climb && _cVelocity.x != 0))
        {
            _cVelocity.y = jumpForce;
            _prevMState = _cMState;
            _cMState = MState.Jump;
            return true;
        }
        return false;
    }

    public void SetManualXSpeed(float xVel)
    {
        manualXSpeed = xVel;
        useManualXSpeed = true;
    }

    public void StopUsingManualSpeed()
    {
        useManualXSpeed = false;
    }

    public void EnsureCorrectPosition(float x)
    {
        Vector3 pos = transform.position;
        pos.x = x;
        transform.position = pos;
        actor.CharacterController2D.warpToGrounded();
        _cMState = MState.Walk;
    }

#if DEBUG_THIS
    void OnGUI()
    {
        GUILayout.Label("cState = " + _cMState.ToString());
        GUILayout.Label("MoveOutput = " + _cVelocity);
        GUILayout.Label("Real velocity = " + actor.CharacterController2D.velocity);
        GUILayout.Label("isGrounded = " + _isGrounded + "( raw = " + actor.CharacterController2D.isGrounded + ")");
        GUILayout.Label("currentLyTouchingClimbables = " + _climbableTriggerCount);
        GUILayout.Label("isOnSlope = " + actor.CharacterController2D.collisionState.standOnToSteepSlope);
        GUILayout.Label("isFakedParents = " + (_fakeParent != null));
    }
#endif
}
