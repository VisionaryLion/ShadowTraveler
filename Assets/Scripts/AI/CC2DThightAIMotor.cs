//#define Glide
#define DEBUG

using UnityEngine;
using System.Collections;
using Actors;
using CC2D;
using System.Collections.Generic;
using Utility.ExtensionMethods;



public class CC2DThightAIMotor : CC2DMotor
{
    bool useManualXSpeed;
    float manualXSpeed;

    protected override void FixedUpdate()
    {
        if (IsFroozen)
            return;

        _prevMState = _cMState;
        //Check, if we are grounded
        if (actor.CharacterController2D.collisionState.wasGroundedLastFrame && !actor.CharacterController2D.isGrounded)
            OnIsNotGrounded();
        else if (actor.CharacterController2D.collisionState.becameGroundedThisFrame)
            OnBecameGrounded();

        var currentInputEvent = _cMovementInput.GetNextEvent();

        switch (_cMState)
        {
            case MState.Walk:
                //Handle sliding of a top step slope

                if (actor.CharacterController2D.collisionState.standOnToSteepSlope || actor.CharacterController2D.manuallyCheckForSteepSlopes(-actor.CharacterController2D.collisionState.belowHit.normal.x))
                {
                    HandleSlope();
                    if (currentInputEvent as JumpEvent != null)
                    {
                        _cMovementInput.ConsumeEvent(currentInputEvent);
                        StartLockedJump(jumpOfSteepSlopeLock);
                        return;
                    }
                }

                AccelerateHorizontal(ref walkHAcc, ref walkHFric, ref walkHMaxSpeed);
                _cVelocity.y = -0.02f; //Small downwards velocity, to keep the CC2D grounded.

                if (currentInputEvent as JumpEvent != null)
                {
                    _cMovementInput.ConsumeEvent(currentInputEvent);
                    StartJump();
                }
                else if (actor.CharacterController2D.collisionState.belowHit.collider.CompareTag(movingPlatformTag))
                {
                    if (actor.CharacterController2D.collisionState.belowHit.collider.transform.rotation != Quaternion.identity || actor.CharacterController2D.collisionState.belowHit.collider.transform.localScale != new Vector3(1, 1, 1))
                        FakeTransformParent = actor.CharacterController2D.collisionState.belowHit.collider.transform.parent;
                    else
                        FakeTransformParent = actor.CharacterController2D.collisionState.belowHit.collider.transform;
                    transform.parent = actor.CharacterController2D.collisionState.belowHit.collider.transform.parent;
                    ReCalculateFakeParentOffset();
                }
                else
                {
                    transform.parent = null;
                    FakeTransformParent = null;
                }

                if (currentInputEvent as CrouchEvent != null)
                {
                    _cMovementInput.ConsumeEvent(currentInputEvent);
                    StartCrouch();
                }
                break;

            case MState.Crouched:
                AccelerateHorizontal(ref crouchHAcc, ref crouchHFric, ref crouchHMaxSpeed);
                _cVelocity.y = -0.02f; //Small downwards velocity, to keep the CC2D grounded.
                ApplyGravity(ref gravityAcceleration, ref fallCap);

                if (_crouchTrigger > 0)
                    break;

                if (currentInputEvent as CrouchEvent != null)
                {
                    _cMovementInput.ConsumeEvent(currentInputEvent);
                    EndCrouch();
                    StartWalk();
                }
                else if (currentInputEvent as JumpEvent != null)
                {
                    _cMovementInput.ConsumeEvent(currentInputEvent);
                    EndCrouch();
                    StartJump();
                }

                break;

            case MState.Fall:
                AccelerateHorizontal(ref inAirHAcc, ref inAirHFric, ref inAirHMaxSpeed);
                ApplyGravity(ref gravityAcceleration, ref fallCap);

                //Possible transitions
#if Glide
                    if (!_cMovementInput.isJumpConsumed && _cMovementInput.jump && Time.time - _cMovementInput.timeOfLastJumpStateChange >= minGlideButtonHoldTime) //Should we glide?
                        {
                    StartGliding();
                    frontAnimator.SetBool("IsFalling", false);
                    }
                    else
#endif
                if (ShouldWallSlide())
                {
                    StartWallSliding();

                }
                break;

            case MState.Jump:
                AccelerateHorizontal(ref inAirHAcc, ref inAirHFric, ref inAirHMaxSpeed);
                ApplyGravity(ref gravityAcceleration, ref fallCap);

                //Possible transitions
                if (_cVelocity.y <= 0) //Finished jumping, gravity catch us!
                    StartFalling();
                else if (actor.CharacterController2D.collisionState.above) // Probably hit the ceiling. Abort Jump to avoid "hovering at the ceiling"!
                {
                    _cVelocity.y = 0;
                    StartFalling();
                }
                break;

            case MState.LockedJump:
                ApplyGravity(ref gravityAcceleration, ref fallCap);

                //Possible transitions
                if (Time.time - _stateStartTime >= jumpOfSteepSlopeLock)
                {
                    //Switch seamless to normal jumping!
                    _cMState = MState.Jump;
                }
                else if (actor.CharacterController2D.collisionState.above) // Probably hit the ceiling. Abort Jump to avoid "hovering at the ceiling"!
                {
                    _cVelocity.y = 0;
                    StartFalling();
                }
                break;

            case MState.Glide:
                AccelerateHorizontal(ref glideHAcc, ref glideHFric, ref glideHMaxSpeed);
                if (currentInputEvent as JumpEvent == null)
                {
                    _cMovementInput.ConsumeEvent(currentInputEvent);
                    StartFalling();
                }
                break;
            case MState.WallSlide:
                if (_cFacingDir * _cMovementInput.horizontalRaw < 0) //They don't share the same sign and horizontalRaw != 0
                {
                    StartFalling();
                }

                if (currentInputEvent as JumpEvent != null)
                {
                    _cMovementInput.ConsumeEvent(currentInputEvent);
                    StartWallJump();
                }
                if (!actor.CharacterController2D.manuallyCheckForCollisionsH(_cFacingDir * 0.01f)) //The wall suddenly ended in mid air!
                    StartFalling();
                break;

            case MState.Climb:
                AccelerateHorizontal(ref inAirHAcc, ref inAirHFric, ref inAirHMaxSpeed);
                _cVelocity.y = _cMovementInput.verticalRaw * climbingVVelocity;

                //Possible transitions
                //To allow jumping, even when not grounded, but only when some sort of x movement is applied.
                //For a straight up jump, climbing shouldn't be used!
                if (currentInputEvent as JumpEvent != null && _cVelocity.x != 0)
                {
                    _cMovementInput.ConsumeEvent(currentInputEvent);
                    StartJump();
                }
                break;
        }
        //Solely determined by input
        if (useManualXSpeed)
            _cVelocity.x = manualXSpeed;
        AdjustFacingDirToVelocity();
        MoveCC2DByVelocity();
    }

    public bool ManualJump(float jumpForce)
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

#if DEBUG
    #region Debug

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
    #endregion
#endif
}
