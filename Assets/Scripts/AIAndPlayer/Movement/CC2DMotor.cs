//#define Glide
//#define DEBUG

using UnityEngine;
using Utility.ExtensionMethods;
using System.Collections;
using System.Collections.Generic;
using Actors;

namespace CC2D
{
    [RequireComponent(typeof(CharacterController2D))]
    public class CC2DMotor : MonoBehaviour
    {
        [SerializeField]
        [HideInInspector]
        [AssignActorAutomaticly]
        HumanMovementActor actor;

        #region Inspector vars
        [Header("External Reference")]
        [RemindToConfigureField]
        [SerializeField]
        [Tooltip("Will only be used for flipping the sprite, based on its movement.")]
        Transform spriteRoot;
        [Header("Control easer:")]
        [SerializeField]
        [Tooltip("Will ground the player at start.")]
        bool startWrappedDown = true;
        [SerializeField]
        [Tooltip("Delay in fixed frames before isGrounded changes from true to false.")]
        int transToFallDelay = 20;
        [SerializeField]
        [Tooltip("Max time a jump will be buffered.")]
        float maxJumpExecutionDelay = 0.5f;
        [SerializeField]
        [Tooltip("Determines how long the jump button has to be hold before switching to gliding.")]
        float minGlideButtonHoldTime = 0.1f;

        [Header("Gravity:")]
        [SerializeField]
        float gravityAcceleration = 20;

        [Header("Walk:")]
        [SerializeField]
        float walkHAcc = 5; //horizontal speed
        [SerializeField]
        float walkHFric = 5; //horizontal speed
        [SerializeField]
        float walkHMaxSpeed = 10;
        [SerializeField]
        float steepSlopeGravity = 5;
        [SerializeField]
        [Tooltip("If you jump of a steep slope, you will not be able to move horizontal for a time frame determined by this variable.")]
        float jumpOfSteepSlopeLock = 0.75f;
        [SerializeField]
        [RemindToConfigureField]
        string movingPlatformTag;

        [Header("Fall:")]
        [SerializeField]
        float inAirHAcc = 11; //horizontal speed
        [SerializeField]
        float inAirHFric = 5;
        [SerializeField]
        float inAirHMaxSpeed = 11;
        [SerializeField]
        [Tooltip("Max velocity, that can be reached by falling.")]
        float fallCap = 100;

        [Header("Jumping:")]
        [SerializeField]
        float jumpVAcc = 20;
        [SerializeField]
        float minJumpTime = 0.5f;
        [SerializeField]
        [Tooltip("The y velocity that gets assigned when the analog jump ends early.")]
        float jumpCutVelocity = 4;

        [Header("Gliding:")]
        [SerializeField]
        float glideVVelocity = 3f;
        [SerializeField]
        float glideHAcc = 11; //horizontal speed
        [SerializeField]
        float glideHFric = 5;
        [SerializeField]
        float glideHMaxSpeed = 11;

        [Header("WallSliding:")]
        [RemindToConfigureField]
        [SerializeField]
        LayerMask wallSlideable = 1; //Everything
        [SerializeField]
        float wallSlidingVVelocity = 3f;
        [SerializeField]
        [Tooltip("The min. time of user input away from the wall, which is needed for detaching from the wall.")]
        float wallStickiness = 0.1f;

        [Header("WallJump:")]
        [SerializeField]
        float walljumpVVelocity = 10;
        [SerializeField]
        float walljumpHVelocity = 5;
        [SerializeField]
        float walljumpHFric = 5;
        [SerializeField]
        [Tooltip("How much time the player input be discarded.")]
        float walljumpLockedTime = 1;

        [Header("Climbing:")]
        [RemindToConfigureField]
        [SerializeField]
        string climbableTag = "Climbable";
        [SerializeField]
        float climbingVVelocity = 5;

        [Header("Physics Interaction:")]
        [SerializeField]
        float bounciness = 0;
        [SerializeField]
        [Tooltip("Used to slowly damp impulses from other rigidbodys.")]
        float standartDrag = 0.3f;

        [Header("Crouch:")]
        [SerializeField]
        float crouchHAcc = 3; //horizontal speed
        [SerializeField]
        float crouchHFric = 3; //horizontal speed
        [SerializeField]
        float crouchHMaxSpeed = 5;

        #endregion

        #region Public
        public enum MState
        {
            WallSlide, //Slower fall
            WallJump, //Jump of wall
            Jump,
            Glide, //Slower fall
            Fall,
            Walk,
            Climb, //Move up, down, right, left
            LockedJump, //No horizontal movement input!
            Crouched
        }

        /// <summary>
        /// Property that holds the current controller input. It's asserted, that it's never set to null.
        /// Setting it to zero will result in an error.
        /// </summary>
        public MovementInput CurrentMovementInput { get; set; }

        public void AddVelocity(Vector2 velocity, float damp, Velocity2D.VelocityAllowsThisState velocityAllowsThisState)
        {
            _allExternalVelocitys.Add(new Velocity2D(velocity, damp, velocityAllowsThisState));
        }

        public void AddVelocity(Velocity2D velocity)
        {
            _allExternalVelocitys.Add(velocity);
        }

        public void FreezeAndResetMovement(bool freeze)
        {
            this.IsFroozen = freeze;
            ResetPlayerMovementInput();
        }

        public void FreezeMovement(bool freeze)
        {
            this.IsFroozen = freeze;
        }


        /// <summary>
        /// If assigned to something different from zero, this motor will act as if it were a child of the assigned object.
        /// </summary>
        public Transform FakeTransformParent { get { return _fakeParent; } set { _fakeParent = value; } }

        public Vector2 Velocity { get { return _cVelocity; } }
        public MState MotorState { get { return _cMState; } }
        public MState PrevMotorState { get { return _prevMState; } }

        public void ResetPlayerMovementInput()
        {
            _cVelocity.x = 0;
            _cVelocity.y = Mathf.Min(_cVelocity.y, 0);
        }

        #endregion

        #region Private

        /// <summary>
        /// Current movement state
        /// </summary>
        MState _cMState;
        MState _prevMState;
        /// <summary>
        /// Will change with delay from grounded to not grounded, to help the player.
        /// </summary>
        bool _isGrounded;
        /// <summary>
        /// Current velocity, acceleration output.
        /// </summary>
        Vector2 _cVelocity;
        /// <summary>
        /// Holds the time the current state started (if the state sets this value).
        /// </summary>
        float _stateStartTime;

        float _wallDetachingInput; //WallSlide specific
        int _climbableTriggerCount; //Climbing specific. Counts the amount of triggers we are currently touching.
        int _cFacingDir;
        Vector3 _fakeParentOffset;
        List<Velocity2D> _allExternalVelocitys;
        Vector2 _totalExternalVelocity;
        bool IsFroozen;

        //Coroutine
        Coroutine _delayedUnGrounding;

        //External reference
        Transform _fakeParent;

        #endregion

        void Awake()
        {
            _cFacingDir = 1; // Assume the sprite starts looking at the right side.
            _allExternalVelocitys = new List<Velocity2D>(1);
            if (startWrappedDown)
            {
                actor.CharacterController2D.warpToGrounded();
                StartWalk();
            }
            else
                StartFalling();
        }

        void Start()
        {
            if (CurrentMovementInput == null)
                CurrentMovementInput = new MovementInput();
        }

        void Update()
        {
            HandleFakeParenting();
        }

        void FixedUpdate()
        {
            if (IsFroozen)
                return;
            _prevMState = _cMState;
            //Check, if we are grounded
            if (actor.CharacterController2D.collisionState.wasGroundedLastFrame && !actor.CharacterController2D.isGrounded)
                OnIsNotGrounded();
            else if (actor.CharacterController2D.collisionState.becameGroundedThisFrame)
                OnBecameGrounded();

            switch (_cMState)
            {
                case MState.Walk:
                    //Handle sliding of a top step slope

                    if (actor.CharacterController2D.collisionState.standOnToSteepSlope || actor.CharacterController2D.manuallyCheckForSteepSlopes(-actor.CharacterController2D.collisionState.belowHit.normal.x))
                    {
                        HandleSlope();
                        if (CurrentMovementInput.ShouldJump(maxJumpExecutionDelay))
                        {
                            StartLockedJump();
                            return;
                        }
                    }

                    AccelerateHorizontal(ref walkHAcc, ref walkHFric, ref walkHMaxSpeed);
                    _cVelocity.y = -0.02f; //Small downwards velocity, to keep the CC2D grounded.

                    if (CurrentMovementInput.ShouldJump(maxJumpExecutionDelay))
                        StartJump();
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

                    if (CurrentMovementInput.toggleCrouch)
                        StartCrouch();

                    break;

                case MState.Crouched:
                    AccelerateHorizontal(ref crouchHAcc, ref crouchHFric, ref crouchHMaxSpeed);
                    _cVelocity.y = -0.02f; //Small downwards velocity, to keep the CC2D grounded.
                    ApplyGravity(ref gravityAcceleration, ref fallCap);

                    if (CurrentMovementInput.toggleCrouch)
                        EndCrouch();

                    break;

                case MState.Fall:
                    AccelerateHorizontal(ref inAirHAcc, ref inAirHFric, ref inAirHMaxSpeed);
                    ApplyGravity(ref gravityAcceleration, ref fallCap);

                    //Possible transitions
#if Glide
                    if (!CurrentMovementInput.isJumpConsumed && CurrentMovementInput.jump && Time.time - CurrentMovementInput.timeOfLastJumpStateChange >= minGlideButtonHoldTime) //Should we glide?
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
                    if (_cVelocity.y <= jumpCutVelocity) //Finished jumping, gravity catch us!
                        StartFalling();
                    else if (Time.time - _stateStartTime >= minJumpTime && !CurrentMovementInput.jump)
                    {
                        if (_cVelocity.y > jumpCutVelocity)
                            _cVelocity.y = jumpCutVelocity;
                        StartFalling();
                    }
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
                    if (!CurrentMovementInput.jump)
                        StartFalling();
                    break;
                case MState.WallSlide:
                    if (_cFacingDir * CurrentMovementInput.horizontalRaw < 0) //They don't share the same sign and horizontalRaw != 0
                    {
                        _wallDetachingInput += Time.deltaTime;
                        if (_wallDetachingInput >= wallStickiness) // Input time exceed wall stickiness. Detach from wall!
                            StartFalling();
                    }
                    else
                        _wallDetachingInput = 0; // No opposite to wall input. Reset wall detaching counter
                    if (CurrentMovementInput.ShouldJump(maxJumpExecutionDelay)) //Check for wall jumps
                        StartWallJump();
                    if (!actor.CharacterController2D.manuallyCheckForCollisionsH(_cFacingDir * 0.01f)) //The wall suddenly ended in mid air!
                        StartFalling();
                    break;

                case MState.WallJump:
                    ApplyFrictionHorizontal(ref walljumpHFric);
                    ApplyGravity(ref gravityAcceleration, ref fallCap);

                    //Possible Transitions
                    if (Time.time - _stateStartTime >= walljumpLockedTime)
                        StartFalling();
                    else if (ShouldWallSlide())
                        StartWallSliding();
                    else if (actor.CharacterController2D.collisionState.above) // Probably hit the ceiling. Abort Jump to avoid "hovering at the ceiling"!
                    {
                        _cVelocity.y = 0;
                        StartFalling();
                    }
                    break;

                case MState.Climb:
                    AccelerateHorizontal(ref inAirHAcc, ref inAirHFric, ref inAirHMaxSpeed);
                    _cVelocity.y = CurrentMovementInput.verticalRaw * climbingVVelocity;

                    //Possible transitions
                    //To allow jumping, even when not grounded, but only when some sort of x movement is applied.
                    //For a straight up jump, climbing shouldn't be used!
                    if (CurrentMovementInput.ShouldJump(maxJumpExecutionDelay) && _cVelocity.x != 0)
                        StartJump();
                    break;
            }
            //Solely determined by input
            AdjustFacingDir();
            MoveCC2DByVelocity();
        }

        void OnTriggerExit2D(Collider2D obj)
        {
            if (obj.CompareTag(climbableTag))
            {
                _climbableTriggerCount--;
                if (_climbableTriggerCount == 0) //No more climbable triggers are touching us. Abort climbing.
                {
                    if (_cMState != MState.Jump)
                        StartFalling();
                }
            }
            else if (obj.CompareTag("Crouch"))
            {
                // if we exit a crouch trigger then we do not have to be crouched
                obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y - .01f, obj.transform.localPosition.z);
            }
        }

        void OnTriggerEnter2D(Collider2D obj)
        {
            if (obj.CompareTag(climbableTag))
            {
                if (_cMState != MState.Climb) //If we aren't already climbing, start now!
                {
                    StartClimbing();
                }
                _climbableTriggerCount++;
            }
            else if (obj.CompareTag("Crouch"))
            {
                // if we enter a crouched trigger then we must crouch
                obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y + .01f, obj.transform.localPosition.z);
            }
        }

        void OnCollisionStay2D(Collision2D col)
        {

            Rigidbody2D oRi = col.collider.GetComponent<Rigidbody2D>();
            if (oRi == null)
                return;

            // Calculate relative velocity
            Vector2 rv = _cVelocity - col.relativeVelocity;

            // Calculate relative velocity in terms of the normal direction
            float velAlongNormal = Mathf.Abs(Vector2.Dot(rv, col.contacts[0].normal));


            // Do not resolve if velocities are separating
            //if (velAlongNormal <= 0)
            //    return;

            // Calculate restitution
            float e = Mathf.Min((col.collider.sharedMaterial == null) ? 0 : col.collider.sharedMaterial.bounciness, bounciness);

            // Calculate impulse scalar
            float j = -(1 + e) * velAlongNormal;
            j /= 1 / actor.CharacterController2D.actor.Rigidbody2D.mass + 1 / oRi.mass;

            // Apply impulse
            Vector2 impulse = j * col.contacts[0].normal;
            oRi.AddForceAtPosition(impulse, col.contacts[0].point, ForceMode2D.Force);
            //actor.CharacterController2D.move((-impulse * 1 / actor.CharacterController2D.rigidBody2D.mass) * Time.deltaTime, false);
            //AddVelocity(-impulse * 1 / actor.CharacterController2D.rigidBody2D.mass, standartDrag, (MState mStaet) => { return true; });
        }

        void OnBecameGrounded()
        {
            _isGrounded = true;
            //Stop the delayed ground routine, as we are already grounded.
            if (_delayedUnGrounding != null)
                StopCoroutine(_delayedUnGrounding);
            //Switch to the default grounded mState, except when we are climbing.
            if (_cMState != MState.Climb)
                StartWalk();
        }

        void OnIsNotGrounded()
        {
            if (_cMState != MState.Climb)
                _delayedUnGrounding = StartCoroutine(DelayForFixedFrames((object data) => { _isGrounded = false; }, transToFallDelay));
            else
                _isGrounded = false;
            if (_cMState == MState.Walk)
            {
                _cVelocity.y = 0; //Set it in WALK to something, now reset it.
                StartFalling();
            }
            else if (_cMState == MState.Crouched)
            {
                EndCrouch();
                _cVelocity.y = 0; //Set it in WALK to something, now reset it.
                StartFalling();
            }
            FakeTransformParent = null;
            //frontAnimator.SetBool("IsGrounded", false);
        }

        void FlipFacingDir()
        {
            spriteRoot.localScale = new Vector3(-spriteRoot.localScale.x, spriteRoot.localScale.y, spriteRoot.localScale.z);
            _cFacingDir *= -1;
        }

        void AdjustFacingDir()
        {
            if (_cVelocity.x * _cFacingDir < 0)
                FlipFacingDir();
        }

        void MoveCC2DByVelocity()
        {
            _totalExternalVelocity = CalculateTotalExternalAccerleration();
            if (_fakeParent != null)
            {
                _fakeParentOffset += actor.CharacterController2D.calcMoveVector((_cVelocity + _totalExternalVelocity) * Time.fixedDeltaTime, _cMState == MState.Jump);
            }
            else
                actor.CharacterController2D.move((_cVelocity + _totalExternalVelocity) * Time.fixedDeltaTime, _cMState == MState.Jump);

            //We turned out to be slower then our external velocity demanded us. We presumably hit something, so reset forces.
            if (_totalExternalVelocity.x == 0)
                return;
            if (_totalExternalVelocity.x > 0)
            {
                if (actor.CharacterController2D.collisionState.right)
                {
                    _allExternalVelocitys.Clear();
                    return;
                }
            }
            else
            {
                if (actor.CharacterController2D.collisionState.left)
                {
                    _allExternalVelocitys.Clear();
                    return;
                }
            }
            if (_totalExternalVelocity.y == 0)
                return;
            if (_totalExternalVelocity.y > 0)
            {
                if (actor.CharacterController2D.collisionState.above)
                {
                    _allExternalVelocitys.Clear();
                    return;
                }
            }
            else
            {
                if (actor.CharacterController2D.collisionState.below)
                {
                    _allExternalVelocitys.Clear();
                    return;
                }
            }
        }

        bool ShouldWallSlide()
        {
            if (actor.CharacterController2D.collisionState.right) //we hit a wall in that direction
            {
                if (Mathf.Approximately(actor.CharacterController2D.collisionState.rightHit.normal.y, 0)) // no sliding on overhangs!
                    if (wallSlideable.IsLayerWithinMask(actor.CharacterController2D.collisionState.rightHit.collider.gameObject.layer))
                        return true; //the walls layer is contained in all allowed layers.
            }
            else if (actor.CharacterController2D.collisionState.left) //we hit a wall in that direction
                if (actor.CharacterController2D.collisionState.leftHit.normal.y >= 0) // no sliding on overhangs!
                    if (wallSlideable.IsLayerWithinMask(actor.CharacterController2D.collisionState.leftHit.collider.gameObject.layer))
                        return true; //the walls layer is contained in all allowed layers.
            return false;
        }

        void HandleSlope()
        {
            _cVelocity = new Vector2(actor.CharacterController2D.collisionState.belowHit.normal.y, -actor.CharacterController2D.collisionState.belowHit.normal.x) * steepSlopeGravity * actor.CharacterController2D.collisionState.belowHit.normal.x;
        }

        void HandleFakeParenting()
        {
            if (_fakeParent != null)
            {
                transform.position = _fakeParent.position + _fakeParentOffset;
            }
        }

        void ReCalculateFakeParentOffset()
        {
            if (_fakeParent != null)
            {
                _fakeParentOffset = transform.position - _fakeParent.position;

            }
        }

        void AccelerateHorizontal(ref float acc, ref float fric, ref float cap)
        {
            if (CurrentMovementInput.horizontalRaw != 0)
            {
                if (CurrentMovementInput.horizontalRaw > 0)
                {
                    _cVelocity.x = Mathf.Abs(_cVelocity.x);
                    _cVelocity.x += acc * Time.fixedDeltaTime;
                    _cVelocity.x = Mathf.Min(cap, _cVelocity.x);
                }
                else
                {
                    _cVelocity.x = -Mathf.Abs(_cVelocity.x);
                    _cVelocity.x -= acc * Time.fixedDeltaTime;
                    _cVelocity.x = Mathf.Max(-cap, _cVelocity.x);
                }
            }
            else if (_cVelocity.x != 0) //No Input? Apply friction.
            {
                if (_cVelocity.x < 0)
                {
                    _cVelocity.x += fric * Time.fixedDeltaTime;
                    if (_cVelocity.x > 0)
                        _cVelocity.x = 0;
                }
                else
                {
                    _cVelocity.x -= fric * Time.fixedDeltaTime;
                    if (_cVelocity.x < 0)
                        _cVelocity.x = 0;
                }
            }

        }

        void ApplyGravity(ref float gravity, ref float cap)
        {
            _cVelocity.y -= gravity * Time.fixedDeltaTime;
            Mathf.Max(cap, gravity);
        }

        void ApplyFrictionHorizontal(ref float fric)
        {
            if (_cVelocity.x == 0)
                return;

            if (_cVelocity.x > 0)
            {
                _cVelocity.x -= fric * Time.fixedDeltaTime;
                if (_cVelocity.x < 0)
                    _cVelocity.x = 0;
            }
            else
            {
                _cVelocity.x += fric * Time.fixedDeltaTime;
                if (_cVelocity.x > 0)
                    _cVelocity.x = 0;
            }
        }

        Vector2 CalculateTotalExternalAccerleration()
        {
            Vector2 result = Vector2.zero;
            Velocity2D pForce;
            //Go through all stored forces. Add them to the result and damp them.
            for (int iForce = 0; iForce < _allExternalVelocitys.Count; iForce++)
            {
                pForce = _allExternalVelocitys[iForce];
                result += pForce.Velocity;
                pForce.DampVelocity(Time.fixedDeltaTime);
                if (pForce.IsVelocityZero() || !pForce.velocityAllowsThisState(_cMState))//Force is zero remove it!
                {
                    _allExternalVelocitys.RemoveAt(iForce);
                    iForce--;
                }
            }
            return result;
        }

        #region Methods to start each state with

        void StartFalling()
        {
            _prevMState = _cMState;
            _cMState = MState.Fall;
        }

        void StartWalk()
        {
            _prevMState = _cMState;
            _cMState = MState.Walk;
        }

        void StartJump()
        {
            _isGrounded = false;
            _stateStartTime = Time.time;
            CurrentMovementInput.isJumpConsumed = true;
            _cVelocity.y = jumpVAcc;
            //frontAnimator.SetTrigger("Jump");
            _prevMState = _cMState;
            _cMState = MState.Jump;
        }

        void StartLockedJump()
        {
            _isGrounded = false;
            _stateStartTime = Time.time;
            CurrentMovementInput.isJumpConsumed = true;
            _cVelocity.y = jumpVAcc;
            _prevMState = _cMState;
            _cMState = MState.LockedJump;
        }

        float crouchScaleFactor = 0.50f;  // hack to show player as crouched
        void StartCrouch()
        {
            _prevMState = _cMState;
            if (_cMState != MState.Crouched)
            {
                spriteRoot.localScale = new Vector3(spriteRoot.localScale.x * crouchScaleFactor, spriteRoot.localScale.y * crouchScaleFactor, spriteRoot.localScale.z);
            }
            CurrentMovementInput.toggleCrouch = false;
            _cMState = MState.Crouched;
        }

        void EndCrouch()
        {
            _prevMState = _cMState;
            if (_cMState == MState.Crouched)
            {
                spriteRoot.localScale = new Vector3(spriteRoot.localScale.x / crouchScaleFactor, spriteRoot.localScale.y / crouchScaleFactor, spriteRoot.localScale.z);
            }
            CurrentMovementInput.toggleCrouch = false;
            _cMState = MState.Walk;
        }

        void StartGliding()
        {
            CurrentMovementInput.isJumpConsumed = true;
            _cVelocity.y = -glideVVelocity;
            _prevMState = _cMState;
            _cMState = MState.Glide;
        }

        void StartWallSliding()
        {
            _cVelocity.x = 0; // No side movement in this state!
            _cVelocity.y = -wallSlidingVVelocity;
            _wallDetachingInput = 0;
            _prevMState = _cMState;
            _cMState = MState.WallSlide;
        }

        void StartWallJump()
        {
            CurrentMovementInput.isJumpConsumed = true;
            _cVelocity.x = walljumpHVelocity * -_cFacingDir;
            _cVelocity.y = walljumpVVelocity;
            _stateStartTime = Time.time;
            _prevMState = _cMState;
            _cMState = MState.WallJump;
        }

        void StartClimbing()
        {
            _cMState = MState.Climb;
            _prevMState = _cMState;
        }

        #endregion

        #region Coroutines

        private delegate void DelayedAction(object data);
        /// <summary>
        /// Executes the given "action" a by "delay" specified number of fixed frames later.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="data">The data, that will be supplied to the "action" method.</param>
        /// <param name="delay">Determines how many fixed frames the "action" should be delayed.</param>
        /// <returns></returns>
        IEnumerator DelayForFixedFrames(DelayedAction action, int delay, object data = null)
        {
            int frameCounter = 0;
            while (frameCounter < delay)
            {
                yield return null;
                frameCounter++;
            }
            action(data);
        }

        #endregion

#if DEBUG
        #region Debug

        void OnGUI()
        {
            if (gameObject.CompareTag("Player"))
            {
                GUILayout.Label("cState = " + _cMState.ToString());
                GUILayout.Label("MoveOutput = " + _cVelocity);
                GUILayout.Label("Real velocity = " + actor.CharacterController2D.velocity);
                GUILayout.Label("isGrounded = " + _isGrounded + "( raw = " + actor.CharacterController2D.isGrounded + ")");
                GUILayout.Label("jump = " + CurrentMovementInput.jump + "( time since last jump state change = " + CurrentMovementInput.timeOfLastJumpStateChange + ")");
                GUILayout.Label("currentLyTouchingClimbables = " + _climbableTriggerCount);
                GUILayout.Label("isOnSlope = " + actor.CharacterController2D.collisionState.standOnToSteepSlope);
                GUILayout.Label("currentExternalForceCount = " + _allExternalVelocitys.Count);
                GUILayout.Label("isFakedParents = " + (_fakeParent != null));
            }
        }
        #endregion
#endif
    }

    public class Velocity2D
    {
        public delegate bool VelocityAllowsThisState(CC2DMotor.MState MState);
        public Vector2 Velocity { get { return vel; } }
        public VelocityAllowsThisState velocityAllowsThisState;
        Vector2 vel;
        float damp;


        public Velocity2D(Vector2 velocity, float damp, VelocityAllowsThisState velocityAllowsThisState)
        {
            this.vel = velocity;
            this.damp = damp;
            this.velocityAllowsThisState = velocityAllowsThisState;
        }

        /// <summary>
        /// Will damp the stored force and return it. (Damped force is also saved internally. Another call will return a even minor force!)
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        public void DampVelocity(float deltaTime)
        {
            vel = vel * (1 - deltaTime * damp);
            //velocity += -velocity.normalized * (Mathf.Sqrt(velocity.magnitude) * drag);
        }

        /// <summary>
        /// Returns true, if the force is approximately zero.
        /// </summary>
        /// <returns></returns>
        public bool IsVelocityZero()
        {
            return Mathf.Approximately(vel.x, 0) && Mathf.Approximately(vel.y, 0);
        }
    }
}
