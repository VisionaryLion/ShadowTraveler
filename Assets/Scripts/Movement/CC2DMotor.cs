using UnityEngine;
using Utility.ExtensionMethods;
using System.Collections;
using System.Collections.Generic;

namespace CC2D
{
    [RequireComponent(typeof(CharacterController2D))]
    [RequireComponent(typeof(Animator))]
    public class CC2DMotor : MonoBehaviour
    {
        #region Inspector vars
        [Header("External Reference")]
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
        AnimationCurve walkHSpeed = AnimationCurve.Linear(0, 0, 1, 10); //horizontal speed
        [SerializeField]
        [Tooltip("If you jump of a steep slope, you will not be able to move horizontal for a time frame determined by this variable.")]
        float jumpOfSteepSlopeLock = 0.75f;

        [Header("Fall:")]
        [SerializeField]
        AnimationCurve inAirHVelocity = AnimationCurve.Linear(0, 0, 1, 11); //horizontal speed
        [SerializeField]
        [Tooltip("Max velocity, that can be reached by falling.")]
        float fallCap = 100;

        [Header("Jumping:")]
        [SerializeField]
        AnimationCurve jumpVVelocity = AnimationCurve.Linear(0, 20, 1, 0);
        [SerializeField]
        float minJumpTime = 0.5f;
        [SerializeField]
        [Tooltip("The y velocity that gets assigned when the analog jump ends early.")]
        float jumpCutVelocity = 4;

        [Header("Gliding:")]
        [SerializeField]
        float glideVVelocity = 3f;

        [Header("WallSliding:")]
        [SerializeField]
        LayerMask wallSlideable = 1; //Everything
        [SerializeField]
        float wallSlidingVVelocity = 3f;
        [SerializeField]
        [Tooltip("The min. time of user input away from the wall, which is needed for detaching from the wall.")]
        float wallStickiness = 0.1f;

        [Header("WallJump:")]
        [SerializeField]
        AnimationCurve walljumpVVelocity = AnimationCurve.Linear(0, 15, 0.5f, 0);
        [SerializeField]
        AnimationCurve walljumpHVelocity = AnimationCurve.Linear(0, 7, 0.5f, 7);

        [Header("Climbing:")]
        [SerializeField]
        string climbableTag = "Climbable";
        [SerializeField]
        float climbingVVelocity = 5;

        [Header("Physics Interaction:")]
        [SerializeField]
        float bounciness;
        [SerializeField]
        [Tooltip("Used to slowly damp impulses from other rigidbodys.")]
        float standartDrag = 0.3f;

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

        /// <summary>
        /// If assigned to something different from zero, this motor will act as if it were a child of the assigned object.
        /// </summary>
        public Transform FakeTransformParent { get { return _fakeParent; } set { _fakeParent = value; } }

        #endregion

        #region Private

        /// <summary>
        /// Current movement state
        /// </summary>
        MState _cMState;
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
        float _animationCurveTime;
        float _wallDetachingInput; //WallSlide specific
        int _climbableTriggerCount; //Climbing specific. Counts the amount of triggers we are currently touching.
        int _cFacingDir;
        Vector3 _fakeParentOffset;
        List<Velocity2D> _allExternalVelocitys;
        Vector2 _totalExternalVelocity;


        //Coroutine
        Coroutine _delayedUnGrounding;

        //External reference
        CharacterController2D _cc2d;
        Animator _animator;
        Transform _fakeParent;
        Collider2D _ownCollider;


        #endregion

        void Awake()
        {
            _cFacingDir = 1; // Assume the sprite starts looking at the right side.
            _animator = GetComponent<Animator>();
            _cc2d = GetComponent<CharacterController2D>();
            CurrentMovementInput = new MovementInput();
            _allExternalVelocitys = new List<Velocity2D>(1);
            _ownCollider = GetComponent<Collider2D>();
            if (startWrappedDown)
            {
                _cc2d.warpToGrounded();
                StartWalk();
            }
            else
                StartFalling();
        }

        void Update()
        {
            HandleFakeParenting();
        }

        void FixedUpdate()
        {
            //Check, if we are grounded
            if (_cc2d.collisionState.wasGroundedLastFrame && !_cc2d.isGrounded)
                OnIsNotGrounded();
            else if (_cc2d.collisionState.becameGroundedThisFrame)
                OnBecameGrounded();

            //If we are not moving, reset the animation curve timer.
            if (CurrentMovementInput.horizontalRaw == 0 || (_cVelocity.x != 0 && (CurrentMovementInput.horizontalRaw > 0) != (_cVelocity.x > 0)))
                _animationCurveTime = Time.time;

            switch (_cMState)
            {
                case MState.Walk:
                    //Handle sliding of a top step slope
                    if (_cc2d.collisionState.standOnToSteepSlope || _cc2d.manuallyCheckForSteepSlopes(-_cc2d.collisionState.belowHit.normal.x))
                    {
                        HandleSlope();
                        if (CurrentMovementInput.ShouldJump(maxJumpExecutionDelay))
                            StartLockedJump();
                    }
                    else //Only x movement when not sliding, or the player could stick to the slope.
                    {
                        _cVelocity.x = CurrentMovementInput.horizontalRaw * walkHSpeed.Evaluate(Time.time - _animationCurveTime);
                        _cVelocity.y = -0.01f; //Small downwards velocity, to keep the CC2D grounded.
                        if (CurrentMovementInput.ShouldJump(maxJumpExecutionDelay))
                            StartJump();
                    }


                    break;

                case MState.Fall:
                    _cVelocity.x = CurrentMovementInput.horizontalRaw * inAirHVelocity.Evaluate(Time.time - _animationCurveTime);
                    if (!CurrentMovementInput.isJumpConsumed && CurrentMovementInput.jump && Time.time - CurrentMovementInput.timeOfLastJumpStateChange >= minGlideButtonHoldTime) //Should we glide?
                        StartGliding();
                    else if (ShouldWallSlide())
                        StartWallSliding();
                    else
                        _cVelocity.y = (-gravityAcceleration * Time.deltaTime) + _cc2d.velocity.y; // Add gravity
                    _cVelocity.y = Mathf.Min(_cVelocity.y, fallCap);
                    break;

                case MState.Jump:
                    _cVelocity.x = CurrentMovementInput.horizontalRaw * inAirHVelocity.Evaluate(Time.time - _animationCurveTime);
                    float deltaTime = Time.time - _stateStartTime;
                    _cVelocity.y = jumpVVelocity.Evaluate(deltaTime);
                    if (deltaTime >= jumpVVelocity[jumpVVelocity.length - 1].time) //Finished jumping, gravity catch us!
                        StartFalling();
                    else if (deltaTime >= minJumpTime && !CurrentMovementInput.jump)
                    {
                        if (_cVelocity.y > jumpCutVelocity)
                            _cVelocity.y = jumpCutVelocity;
                        StartFalling();
                    }
                    else if (_cc2d.collisionState.above) // Probably hit the ceiling. Abort Jump to avoid "hovering at the ceiling"!
                    {
                        _cVelocity.y = 0;
                        StartFalling();
                    }
                    break;

                case MState.LockedJump:
                    deltaTime = Time.time - _stateStartTime;
                    _cVelocity.y = jumpVVelocity.Evaluate(deltaTime);
                    if (deltaTime >= jumpOfSteepSlopeLock)
                    {
                        //Switch seamless to normal jumping!
                        _cMState = MState.Jump; 
                    }
                    else if (deltaTime >= jumpVVelocity[jumpVVelocity.length - 1].time) //Finished jumping, gravity catch us!
                        StartFalling();
                    else if (deltaTime >= minJumpTime && !CurrentMovementInput.jump)
                    {
                        if (_cVelocity.y > jumpCutVelocity)
                            _cVelocity.y = jumpCutVelocity;
                        StartFalling();
                    }
                    else if (_cc2d.collisionState.above) // Probably hit the ceiling. Abort Jump to avoid "hovering at the ceiling"!
                    {
                        _cVelocity.y = 0;
                        StartFalling();
                    }
                    break;

                case MState.Glide:
                    _cVelocity.x = CurrentMovementInput.horizontalRaw * inAirHVelocity.Evaluate(Time.time - _animationCurveTime);
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
                    if (!_cc2d.manuallyCheckForCollisionsH(_cFacingDir * 0.01f)) //The wall suddenly ended in mid air!
                        StartFalling();
                    break;
                case MState.WallJump:
                    deltaTime = Time.time - _stateStartTime;
                    if (deltaTime >= jumpVVelocity[walljumpHVelocity.length - 1].time)
                        StartFalling();
                    //Essentially block the user input and fully drive by animation curves.
                    //_wallDetachingInput stores the original facing dir in this context!
                    _cVelocity.x = walljumpHVelocity.Evaluate(deltaTime) * -_wallDetachingInput;
                    _cVelocity.y = walljumpVVelocity.Evaluate(deltaTime);

                    break;
                case MState.Climb:
                    _cVelocity.x = CurrentMovementInput.horizontalRaw * inAirHVelocity.Evaluate(Time.time - _animationCurveTime);
                    _cVelocity.y = CurrentMovementInput.verticalRaw * climbingVVelocity;
                    //To allow jumping, even when not grounded, but only when some sort of x movement is applied.
                    //For a straight up jump, climbing should be used!
                    if (CurrentMovementInput.ShouldJump(maxJumpExecutionDelay) && _cVelocity.x != 0)
                        StartJump();
                    break;
            }
            //Solely determined by input
            UpdateAnimatorVars();
            AdjustFacingDir();

            MoveCC2DByVelocity();
            ReCalculateFakeParentOffset();
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
        }

        void OnTriggerEnter2D(Collider2D obj)
        {
            if (obj.CompareTag(climbableTag))
            {
                if (_cMState != MState.Climb) //If we aren't already climbing, start now!
                    StartClimbing();
                _climbableTriggerCount++;
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
            j /= 1 / _cc2d.rigidBody2D.mass + 1 / oRi.mass;

            // Apply impulse
            Vector2 impulse = j * col.contacts[0].normal;
            oRi.AddForceAtPosition(impulse, col.contacts[0].point, ForceMode2D.Force);
            //_cc2d.move((-impulse * 1 / _cc2d.rigidBody2D.mass) * Time.deltaTime, false);
            //AddVelocity(-impulse * 1 / _cc2d.rigidBody2D.mass, standartDrag, (MState mStaet) => { return true; });
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
        }

        void UpdateAnimatorVars()
        {
            _animator.SetFloat("VelocityX", _cc2d.velocity.x);
            _animator.SetFloat("VelocityY", _cc2d.velocity.y);
            _animator.SetBool("IsGrounded", _cc2d.isGrounded);
            _animator.SetFloat("AbsX", Mathf.Abs(_cc2d.velocity.x));
            _animator.SetFloat("AbsY", Mathf.Abs(_cc2d.velocity.y));
            _animator.SetBool("IsOnWall", _cMState == MState.WallSlide);
            _animator.SetBool("HasMoveInput", _cVelocity != Vector2.zero);
            _animator.SetBool("IsOnLadder", _cMState == MState.Climb);
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
            _cc2d.move((_cVelocity + _totalExternalVelocity) * Time.fixedDeltaTime, _cMState == MState.Jump);

            Vector2 deltaVelocity = _cc2d.velocity - _cVelocity;
            //We turned out to be slower then our external velocity demanded us. We presumably hit something, so reset forces.
            if (_totalExternalVelocity.x == 0)
                return;
            if (_totalExternalVelocity.x > 0)
            {
                if (_cc2d.collisionState.right)
                {
                    _allExternalVelocitys.Clear();
                    return;
                }
            }
            else
            {
                if (_cc2d.collisionState.left)
                {
                    _allExternalVelocitys.Clear();
                    return;
                }
            }
            if (_totalExternalVelocity.y == 0)
                return;
            if (_totalExternalVelocity.y > 0)
            {
                if (_cc2d.collisionState.above)
                {
                    _allExternalVelocitys.Clear();
                    return;
                }
            }
            else
            {
                if (_cc2d.collisionState.below)
                {
                    _allExternalVelocitys.Clear();
                    return;
                }
            }
        }

        bool ShouldWallSlide()
        {
            if (CurrentMovementInput.horizontalRaw > 0) //Moving right
            {
                if (_cc2d.collisionState.right) //we hit a wall in that direction
                    if (Mathf.Approximately(_cc2d.collisionState.rightHit.normal.y, 0)) // no sliding on overhangs!
                        if (wallSlideable.IsLayerWithinMask(_cc2d.collisionState.rightHit.collider.gameObject.layer))
                            return true; //the walls layer is contained in all allowed layers.
            }
            else //Moving left
            {
                if (_cc2d.collisionState.left) //we hit a wall in that direction
                    if (_cc2d.collisionState.leftHit.normal.y >= 0) // no sliding on overhangs!
                        if (wallSlideable.IsLayerWithinMask(_cc2d.collisionState.leftHit.collider.gameObject.layer))
                            return true; //the walls layer is contained in all allowed layers.
            }
            return false;
        }

        void HandleSlope()
        {
            _cVelocity = new Vector2(_cc2d.collisionState.belowHit.normal.y, -_cc2d.collisionState.belowHit.normal.x) * gravityAcceleration * _cc2d.collisionState.belowHit.normal.x;
        }

        void HandleFakeParenting()
        {
            if (_fakeParent != null)
            {
                transform.position = _fakeParentOffset + _fakeParent.position;
            }
        }

        void ReCalculateFakeParentOffset()
        {
            if (_fakeParent != null)
            {
                _fakeParentOffset = transform.position - _fakeParent.position;
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
            _cMState = MState.Fall;
        }

        void StartWalk()
        {
            _cMState = MState.Walk;
        }

        void StartJump()
        {
            _isGrounded = false;
            _stateStartTime = Time.time;
            CurrentMovementInput.isJumpConsumed = true;
            _cMState = MState.Jump;
        }

        void StartLockedJump()
        {
            _isGrounded = false;
            _stateStartTime = Time.time;
            CurrentMovementInput.isJumpConsumed = true;
            _cMState = MState.LockedJump;
        }

        void StartGliding()
        {
            CurrentMovementInput.isJumpConsumed = true;
            _cVelocity.y = -glideVVelocity;
            _cMState = MState.Glide;
        }

        void StartWallSliding()
        {
            _cVelocity.x = 0; // No side movement in this state!
            _cVelocity.y = -wallSlidingVVelocity;
            _wallDetachingInput = 0;
            _cMState = MState.WallSlide;
        }

        void StartWallJump()
        {
            _wallDetachingInput = _cFacingDir; //Use this unrelated (I know not good, but think of the memory!) for storing the fall facing dir.
            _stateStartTime = Time.time;
            _cMState = MState.WallJump;
        }

        void StartClimbing()
        {
            _cMState = MState.Climb;
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

        #region Debug

        void OnGUI()
        {
            GUILayout.Label("cState = " + _cMState.ToString());
            GUILayout.Label("MoveOutput = " + _cVelocity);
            GUILayout.Label("Real velocity = " + _cc2d.velocity);
            GUILayout.Label("isGrounded = " + _isGrounded + "( raw = " + _cc2d.isGrounded + ")");
            GUILayout.Label("jump = " + CurrentMovementInput.jump + "( time since last jump state change = " + CurrentMovementInput.timeOfLastJumpStateChange + ")");
            GUILayout.Label("currentLyTouchingClimbables = " + _climbableTriggerCount);
            GUILayout.Label("isOnSlope = " + _cc2d.collisionState.standOnToSteepSlope);
            GUILayout.Label("currentExternalForceCount = " + _allExternalVelocitys.Count);
            GUILayout.Label("isFakedParents = " + (_fakeParent != null));
        }
        #endregion
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
