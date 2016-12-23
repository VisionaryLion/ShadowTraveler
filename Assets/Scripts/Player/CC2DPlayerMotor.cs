#define DEBUG

using UnityEngine;

namespace CC2D
{
    public class CC2DPlayerMotor : CC2DMotor
    {
        #region Inspector vars
        [Header("External Reference")]
        [SerializeField]
        [Tooltip("Delay in fixed frames before isGrounded changes from true to false.")]
        int transToFallDelay = 20;
        [SerializeField]
        [Tooltip("Determines how long the jump button has to be hold before switching to gliding.")]
        float minGlideButtonHoldTime = 0.1f;

        [Header("Jumping:")]
        [SerializeField]
        float minJumpTime = 0.5f;
        [SerializeField]
        [Tooltip("The y velocity that gets assigned when the analog jump ends early.")]
        float jumpCutVelocity = 4;

        [Header("WallSliding:")]
        [SerializeField]
        [Tooltip("The min. time of user input away from the wall, which is needed for detaching from the wall.")]
        float wallStickiness = 0.1f;

        #endregion

        #region Private

        float _wallDetachingInput; //WallSlide specific

        //Coroutine
        Coroutine _delayedUnGrounding;

        #endregion

        protected override void State_WallSlide()
        {
            if (_cFacingDir * _cMovementInput.horizontalRaw < 0) //They don't share the same sign and horizontalRaw != 0
            {
                _wallDetachingInput += Time.deltaTime;
                if (_wallDetachingInput >= wallStickiness) // Input time exceed wall stickiness. Detach from wall!
                    StartFalling();
            }
            else
                _wallDetachingInput = 0; // No opposite to wall input. Reset wall detaching counter

            if (currentInputEvent as JumpEvent != null)
            {
                _cMovementInput.ConsumeEvent(currentInputEvent);
                StartWallJump();
            }
            if (!actor.CharacterController2D.manuallyCheckForCollisionsH(_cFacingDir * 0.01f)) //The wall suddenly ended in mid air!
                StartFalling();
        }

        protected override void State_Jump()
        {
            base.State_Jump();

            if (Time.time - _stateStartTime >= minJumpTime && !Input.GetButton("Jump"))
            {
                _cVelocity.y = jumpCutVelocity;
                StartFalling();
            }
        }

        protected override void OnBecameGrounded()
        {
            if (_delayedUnGrounding != null)
                StopCoroutine(_delayedUnGrounding);
            base.OnBecameGrounded();
        }

        protected override void OnIsNotGrounded()
        {
            Debug.Assert(_crouchTrigger == 0);

            if (_cMState != MState.Climb)
                _delayedUnGrounding = StartCoroutine(DelayForFixedFrames((object data) => { _isGrounded = false; }, transToFallDelay));
            else
                _isGrounded = false;
            if (_cMState == MState.Walk)
            {
                StartFalling();
            }
            else if (_cMState == MState.Crouched)
            {
                EndCrouch();
                StartFalling();
            }
            FakeTransformParent = null;
        }

#if DEBUG
        void OnGUI()
        {
            if (gameObject.CompareTag("Player"))
            {
                GUILayout.Label("cState = " + _cMState.ToString());
                GUILayout.Label("MoveOutput = " + _cVelocity);
                GUILayout.Label("Real velocity = " + actor.CharacterController2D.velocity);
                GUILayout.Label("isGrounded = " + _isGrounded + "( raw = " + actor.CharacterController2D.isGrounded + ")");
                GUILayout.Label("currentLyTouchingClimbables = " + _climbableTriggerCount);
                GUILayout.Label("isOnSlope = " + actor.CharacterController2D.collisionState.standOnToSteepSlope);
                GUILayout.Label("isFakedParents = " + (_fakeParent != null));
            }
        }
#endif
    }
}
