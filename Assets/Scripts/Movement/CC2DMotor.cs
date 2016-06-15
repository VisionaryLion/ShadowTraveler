using UnityEngine;
using System.Collections;

namespace CC2D
{
    [RequireComponent(typeof(CharacterController2D))]
    public class CC2DMotor : MonoBehaviour
    {
        #region Inspector vars

        [Header("Control easer:")]
        [SerializeField]
        int transToFallDelay = 2; //Delay in fixed frames before "_isGrounded" changes from true to false.
        [SerializeField]
        float maxJumpExecutionDelay = 4; //Max time a jump will be buffered.

        [Header("Gravity:")]
        [SerializeField]
        float gravityAcceleration = 9.81f;

        [Header("Walk:")]
        [SerializeField]
        AnimationCurve walkHSpeed = AnimationCurve.Linear(0, 0, 1, 10); //horizontal speed
        [SerializeField]
        float inMotionThreshold = 0.01f; //Is used to determine, if the cc2d is actually moving.

        [Header("Fall:")]
        [SerializeField]
        AnimationCurve inAirHVelocity = AnimationCurve.Linear(0, 0, 1, 10); //horizontal speed
        [SerializeField]
        float fallCap = 100; //max velocity

        [Header("Jumping:")]
        [SerializeField]
        AnimationCurve jumpVelocity = AnimationCurve.Linear(0, 0, 1, 10);
        [SerializeField]
        float minJumpTime = 0.5f;

        [Header("Gravity:")]
        [SerializeField]
        float glideVelocity = 3f;

        #endregion

        #region Public
        /// <summary>
        /// Property that holds the current controller input. It's asserted, that it's never set to null.
        /// Setting it to zero will result in an error.
        /// </summary>
        public MovementInput CurrentMovementInput { get; set; }

        #endregion

        #region Private

        enum MState
        {
            WallSlide, //Slower fall
            WallJump, //Jump of wall
           /**/ Jump,
            Glide, //Slower fall
           /**/ Fall,
           /**/ Walk,
            Climb, //Move up, down, right, left
            PhysicalWalk, //Simulate momentum
        }

        CharacterController2D _cc2d;
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
        float _bufferedTimeOfJumpInput;

        //Coroutines
        Coroutine _delayedUnGrounding;
        Coroutine _exeAnalogJump;

        #endregion

        void Awake()
        {
            _cc2d = GetComponent<CharacterController2D>();
            CurrentMovementInput = new MovementInput();
            StartFalling();
        }

        void FixedUpdate()
        {
            //Check, if we are grounded
            if (_isGrounded && _cc2d.collisionState.wasGroundedLastFrame && !_cc2d.isGrounded)
                OnIsNotGrounded();
            else if (_cc2d.collisionState.becameGroundedThisFrame)
                OnBecameGrounded();

            //If we are not moving, reset the animation curve timer.
            if (CurrentMovementInput.horizontalRaw == 0 || (_cVelocity.x != 0 && (CurrentMovementInput.horizontalRaw > 0) != (_cVelocity.x > 0)))
                _animationCurveTime = Time.time;

            //Check, if we should jump
            if (_isGrounded && (CurrentMovementInput.jump || Time.time - CurrentMovementInput.timeOfJumpButtonDown <= maxJumpExecutionDelay))
                StartJump();

            switch (_cMState)
            {
                case MState.Walk:
                    _cVelocity.x = CurrentMovementInput.horizontalRaw * walkHSpeed.Evaluate(Time.time - _animationCurveTime);
                    _cVelocity.y = -1; //Small downwards velocity, to keep the CC2D grounded.
                    break;

                case MState.Fall:
                    _cVelocity.x = CurrentMovementInput.horizontalRaw * inAirHVelocity.Evaluate(Time.time - _animationCurveTime);
                    if(CurrentMovementInput.jump) //Should we glide?
                        _cVelocity.y = -glideVelocity;
                    else
                    _cVelocity.y = (-gravityAcceleration * Time.deltaTime) + _cc2d.velocity.y; // Add gravity
                    _cVelocity.y = Mathf.Min(_cVelocity.y, fallCap);
                    break;

                case MState.Jump:
                    _cVelocity.x = CurrentMovementInput.horizontalRaw * inAirHVelocity.Evaluate(Time.time - _animationCurveTime);
                    float deltaTime = Time.time - _stateStartTime;
                    _cVelocity.y = jumpVelocity.Evaluate(deltaTime);
                    if (deltaTime >= jumpVelocity[jumpVelocity.length - 1].time) //Finished jumping, gravity catch us!
                        StartFalling();
                    else if (deltaTime >= minJumpTime && (!CurrentMovementInput.jump || CurrentMovementInput.timeOfJumpButtonDown > _stateStartTime))
                        StartFalling();
                    else if (_cc2d.collisionState.above) // Probably hit the ceiling. Abort Jump to avoid "hovering at the ceiling"!
                        StartFalling();
                    break;
            }
            MoveCC2DByVelocity();
        }

        void OnBecameGrounded()
        {
            _isGrounded = true;
            //Stop the delayed ground routine, as we are already grounded.
            if (_delayedUnGrounding != null)
                StopCoroutine(_delayedUnGrounding);
            //Stop analog jump, as we aren't in air anymore.
            if (_exeAnalogJump != null)
                StopCoroutine(_exeAnalogJump);
            //Switch to the default grounded mState
            StartWalk();
        }

        void OnIsNotGrounded()
        {
            _delayedUnGrounding = StartCoroutine(DelayForFixedFrames((object data) => { _isGrounded = false; }, transToFallDelay));
            if (_cMState == MState.Walk)
                StartFalling();
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
            _cMState = MState.Jump;
        }

        #endregion

        #region Utility methods

        void MoveCC2DByVelocity()
        {
            _cc2d.move(_cVelocity * Time.fixedDeltaTime, _cMState == MState.Jump);
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
            GUILayout.Label("isGrounded = " + _isGrounded + "( raw = " + _cc2d.isGrounded + ")");
            GUILayout.Label("jump = " + CurrentMovementInput.jump + "( time since last jump button down = " + CurrentMovementInput.timeOfJumpButtonDown + ")");
            //GUILayout.Label("shouldJump = " + _isJumpButtonPressed + "( raw = " + Input.GetButton("Jump") + ")");
        }
        #endregion
    }
}
